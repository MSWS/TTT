using System.Drawing;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Traitor;
using ShopAPI.Events;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.PoisonShots;

public class PoisonShotsListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly Dictionary<IPlayer, int> poisonShots = new();

  private readonly PoisonShotsConfig config =
    provider.GetRequiredService<PoisonShotsConfig>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly List<IDisposable> poisonTimers = [];

  [UsedImplicitly]
  [EventHandler]
  public void OnPurchase(PlayerPurchaseItemEvent ev) {
    if (ev.Item is not PoisonShotsItem) return;
    poisonShots.TryAdd(ev.Player, 0);
    poisonShots[ev.Player] += config.TotalShots;
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnFire(EventWeaponFire ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid);
    if (!poisonShots.TryGetValue(player, out var shot) || shot <= 0)
      return HookResult.Continue;
    Server.NextWorldUpdate(() => poisonShots[player]--);
    return HookResult.Continue;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (ev.Attacker == null) return;
    if (!poisonShots.TryGetValue(ev.Attacker, out var shot) || shot <= 0)
      return;
    poisonShots[ev.Attacker]--;
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
      if (!tickPoison(effect)) timer?.Dispose();
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
    gamePlayer?.ColorScreen(Color.Purple, 0.2f, 0.3f);

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
}