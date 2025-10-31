using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Items.PoisonSmoke;

public class PoisonSmokeListener(IServiceProvider provider)
  : BaseListener(provider), IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ISet<string> killedWithPoison = new HashSet<string>();

  private readonly List<IDisposable> poisonSmokes = [];

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private PoisonSmokeConfig config
    => Provider.GetService<IStorage<PoisonSmokeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PoisonSmokeConfig();

  public override void Dispose() {
    base.Dispose();
    foreach (var timer in poisonSmokes) timer.Dispose();

    poisonSmokes.Clear();
    killedWithPoison.Clear();
  }


  [UsedImplicitly]
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
    startPoisonEffect(projectile, player);
    return HookResult.Continue;
  }

  [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
  private void startPoisonEffect(CSmokeGrenadeProjectile projectile,
    IOnlinePlayer thrower) {
    IDisposable? timer = null;

    var effect = new PoisonEffect(projectile, thrower);

    timer = Scheduler.SchedulePeriodic(config.PoisonConfig.TimeBetweenDamage, ()
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

    var players = Finder.GetOnline()
     .Where(player => player.IsAlive && Roles.GetRoles(player)
       .Any(role => role is InnocentRole or DetectiveRole));

    var gamePlayers = players.Select(p => (p, converter.GetPlayer(p)))
     .Where(p => p.Item2 != null && p.Item2.Pawn.Value != null
        && p.Item2.Pawn.Value.IsValid)
     .Select(p => (p!, p.Item2?.Pawn.Value?.AbsOrigin.Clone()!));

    gamePlayers = gamePlayers.Where(t
      => t.Item2.Distance(effect.Origin) <= config.SmokeRadius);

    foreach (var (apiPlayer, gamePlayer) in gamePlayers.Select(p => p.Item1)) {
      if (effect.DamageGiven >= config.PoisonConfig.TotalDamage) continue;
      if (gamePlayer.GetHealth() - config.PoisonConfig.DamagePerTick <= 0) {
        killedWithPoison.Add(apiPlayer.Id);
        var playerDeathEvent = new PlayerDeathEvent(apiPlayer)
         .WithKiller(effect.Attacker as IOnlinePlayer)
         .WithWeapon("[Poison Smoke]");
        Bus.Dispatch(playerDeathEvent);

        gamePlayer.SetHealth(0);
        continue;
      }

      var dmgEvent = new PlayerDamagedEvent(apiPlayer,
        effect.Attacker as IOnlinePlayer, config.PoisonConfig.DamagePerTick) {
        Weapon = "[Poison Smoke]"
      };

      Bus.Dispatch(dmgEvent);

      gamePlayer.DealPoisonDamage(config.PoisonConfig.DamagePerTick);
      effect.DamageGiven += config.PoisonConfig.DamagePerTick;
    }

    return effect.DamageGiven < config.PoisonConfig.TotalDamage;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    killedWithPoison.Clear();
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRagdollSpawn(BodyCreateEvent ev) {
    if (!killedWithPoison.Contains(ev.Body.OfPlayer.Id)) return;
    if (ev.Body.Killer == null || ev.Body.Killer.Id == ev.Body.OfPlayer.Id)
      ev.IsCanceled = true;
  }

  private class PoisonEffect(CSmokeGrenadeProjectile projectile,
    IOnlinePlayer attacker) {
    public int Ticks { get; set; }
    public int DamageGiven { get; set; }
    public Vector Origin { get; } = projectile.AbsOrigin.Clone()!;
    public CSmokeGrenadeProjectile Projectile { get; } = projectile;
    public IPlayer Attacker { get; } = attacker;
  }
}