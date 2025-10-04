using System.Drawing;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using ShopAPI.Events;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.PoisonShots;

public class PoisonShotsListener(IServiceProvider provider)
  : BaseListener(provider), IPluginModule {
  private readonly Dictionary<IPlayer, int> poisonShots = new();

  private readonly PoisonShotsConfig config =
    provider.GetService<IStorage<PoisonShotsConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PoisonShotsConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly List<IDisposable> poisonTimers = [];

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnFire(EventWeaponFire ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    if (!Tag.GUNS.Contains(ev.Weapon)) return HookResult.Continue;
    if (converter.GetPlayer(ev.Userid) is not IOnlinePlayer player)
      return HookResult.Continue;
    var remainingShots = usePoisonShot(player);
    if (remainingShots == 0)
      Messenger.Message(player, Locale[PoisonShotMsgs.SHOP_ITEM_POISON_OUT]);

    return HookResult.Continue;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (ev.Attacker == null) return;
    if (!poisonShots.TryGetValue(ev.Attacker, out var shot) || shot <= 0)
      return;
    Messenger.Message(ev.Attacker,
      Locale[PoisonShotMsgs.SHOP_ITEM_POISON_HIT(ev.Player)]);
    addPoisonEffect(ev.Player);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    foreach (var timer in poisonTimers) timer.Dispose();
    poisonTimers.Clear();
  }

  private void addPoisonEffect(IPlayer player) {
    IDisposable? timer = null;

    var effect = new PoisonEffect(player);
    timer = scheduler.SchedulePeriodic(config.TimeBetweenDamage, () => {
      // ReSharper disable once AccessToModifiedClosure
      Server.NextWorldUpdate(() => {
        if (!tickPoison(effect)) timer?.Dispose();
      });
    });

    poisonTimers.Add(timer);
  }

  private bool tickPoison(PoisonEffect effect) {
    if (effect.Player is not IOnlinePlayer online) return false;
    if (!online.IsAlive) return false;
    online.Health -= config.DamagePerTick;
    effect.Ticks++;
    effect.DamageGiven += config.DamagePerTick;

    var gamePlayer = converter.GetPlayer(online);
    gamePlayer?.ColorScreen(config.PoisonColor, 0.2f, 0.3f);
    gamePlayer?.ExecuteClientCommand("play " + config.PoisonSound);

    return effect.DamageGiven < config.TotalDamage;
  }

  public override void Dispose() {
    base.Dispose();
    foreach (var timer in poisonTimers) timer.Dispose();
  }

  private class PoisonEffect(IPlayer player) {
    public IPlayer Player { get; init; } = player;
    public int Ticks { get; set; }
    public int DamageGiven { get; set; }
  }

  /// <summary>
  /// Uses a poison shot for the player. Returns the remaining shots, -1 if none
  /// are available.
  /// </summary>
  /// <param name="player"></param>
  /// <returns></returns>
  private int usePoisonShot(IOnlinePlayer player) {
    if (!poisonShots.TryGetValue(player, out var shot) || shot <= 0) {
      if (!shop.HasItem<PoisonShotsItem>(player)) return -1;
      poisonShots[player] = config.TotalShots;
    }

    poisonShots[player]--;
    if (poisonShots[player] > 0) return poisonShots[player];

    poisonShots.Remove(player);
    shop.RemoveItem<PoisonShotsItem>(player);
    return 0;
  }
}