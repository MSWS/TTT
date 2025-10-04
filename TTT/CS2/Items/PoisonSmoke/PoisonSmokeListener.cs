using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Items.PoisonSmoke;

public class PoisonSmokeListener(IServiceProvider provider) : IPluginModule {
  private readonly PoisonSmokeConfig config =
    provider.GetService<IStorage<PoisonSmokeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PoisonSmokeConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly List<IDisposable> poisonSmokes = [];

  private readonly IRoleAssigner roleAssigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() {
    foreach (var timer in poisonSmokes) timer.Dispose();

    poisonSmokes.Clear();
  }

  public void Start() { }

  [GameEventHandler]
  public HookResult OnSmokeGrenade(EventSmokegrenadeDetonate ev,
    GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid) as IOnlinePlayer;
    if (player == null) return HookResult.Continue;
    if (!shop.HasItem<PoisonSmokeItem>(player)) return HookResult.Continue;

    shop.RemoveItem<PoisonSmokeItem>(player);

    var projectile =
      Utilities.GetEntityFromIndex<CSmokeGrenadeProjectile>(ev.Entityid);
    if (projectile == null || !projectile.IsValid) return HookResult.Continue;
    startPoisonEffect(projectile);
    return HookResult.Continue;
  }

  [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
  private void startPoisonEffect(CSmokeGrenadeProjectile projectile) {
    IDisposable? timer = null;

    var effect = new PoisonEffect(projectile);

    timer = scheduler.SchedulePeriodic(config.PoisonConfig.TimeBetweenDamage, ()
      => {
      Server.NextWorldUpdate(() => {
        if (tickPoisonEffect(effect) || timer == null) return;
        timer.Dispose();
        poisonSmokes.Remove(timer);
      });
    });

    poisonSmokes.Add(timer);
  }

  private bool tickPoisonEffect(PoisonEffect effect) {
    if (!effect.Projectile.IsValid) return false;
    effect.Ticks++;

    var players = finder.GetOnline()
     .Where(player => player.IsAlive && roleAssigner.GetRoles(player)
       .Any(role => role is InnocentRole or DetectiveRole));

    var gamePlayers = players.Select(p => converter.GetPlayer(p))
     .Where(p => p != null && p.Pawn.Value != null && p.Pawn.Value.IsValid)
     .Select(p => (p!, p?.Pawn.Value?.AbsOrigin.Clone()!));

    gamePlayers = gamePlayers.Where(t
      => t.Item2.Distance(effect.Origin) <= config.SmokeRadius);

    foreach (var player in gamePlayers.Select(p => p.Item1)) {
      if (effect.DamageGiven >= config.PoisonConfig.TotalDamage) continue;
      player.AddHealth(-config.PoisonConfig.DamagePerTick);
      player.ExecuteClientCommand("play " + config.PoisonConfig.PoisonSound);
      effect.DamageGiven += config.PoisonConfig.DamagePerTick;
    }

    return effect.DamageGiven < config.PoisonConfig.TotalDamage;
  }

  private class PoisonEffect(CSmokeGrenadeProjectile projectile) {
    public int Ticks { get; set; }
    public int DamageGiven { get; set; }
    public Vector Origin { get; } = projectile.AbsOrigin.Clone()!;
    public CSmokeGrenadeProjectile Projectile { get; } = projectile;
  }
}