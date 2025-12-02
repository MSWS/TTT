using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using SpecialRoundAPI;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.PoisonShots;

public class PoisonShotsListener(IServiceProvider provider)
  : BaseListener(provider), IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly Dictionary<string, IPlayer> killedWithPoison = new();

  private readonly Dictionary<IPlayer, int> poisonShots = new();

  private readonly List<IDisposable> poisonTimers = [];

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private PoisonShotsConfig config
    => Provider.GetService<IStorage<PoisonShotsConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PoisonShotsConfig();

  public override void Dispose() {
    base.Dispose();
    foreach (var timer in poisonTimers) timer.Dispose();
  }

  public void Start(BasePlugin? plugin) {
    base.Start();
    plugin?.HookUserMessage(452, onWeaponSound);
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnFire(EventWeaponFire ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    if (!Tag.PISTOLS.Contains(ev.Weapon)) return HookResult.Continue;
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
    if (ev.Weapon == null || !Tag.PISTOLS.Contains(ev.Weapon)) return;
    Messenger.Message(ev.Attacker,
      Locale[PoisonShotMsgs.SHOP_ITEM_POISON_HIT(ev.Player)]);
    addPoisonEffect(ev.Player, ev.Attacker);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    foreach (var timer in poisonTimers) timer.Dispose();
    poisonTimers.Clear();
    poisonShots.Clear();
    killedWithPoison.Clear();
  }

  [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
  private void addPoisonEffect(IPlayer player, IPlayer shooter) {
    IDisposable? timer = null;

    var effect = new PoisonEffect(player, shooter);
    timer = scheduler.SchedulePeriodic(config.PoisonConfig.TimeBetweenDamage, ()
      => {
      Server.NextWorldUpdate(() => {
        if (tickPoison(effect) || timer == null) return;
        timer.Dispose();
        poisonTimers.Remove(timer);
      });
    });

    poisonTimers.Add(timer);
  }

  private bool tickPoison(PoisonEffect effect) {
    if (effect.Player is not IOnlinePlayer online) return false;
    if (!online.IsAlive) return false;

    var dmgEvent = new PlayerDamagedEvent(online,
      effect.Shooter as IOnlinePlayer, online.Health,
      online.Health - config.PoisonConfig.DamagePerTick) {
      Weapon = $"[{Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS]}]"
    };

    bus.Dispatch(dmgEvent);

    if (dmgEvent.IsCanceled) return true;

    if (online.Health - config.PoisonConfig.DamagePerTick <= 0) {
      killedWithPoison[online.Id] = effect.Shooter;
      var deathEvent = new PlayerDeathEvent(online)
       .WithKiller(effect.Shooter as IOnlinePlayer)
       .WithWeapon($"[{Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS]}]");
      bus.Dispatch(deathEvent);
    }

    effect.Ticks++;
    effect.DamageGiven += config.PoisonConfig.DamagePerTick;

    var gamePlayer = converter.GetPlayer(online);
    gamePlayer?.ColorScreen(config.PoisonColor, 0.2f, 0.3f);
    if (gamePlayer != null)
      gamePlayer.DealPoisonDamage(config.PoisonConfig.DamagePerTick);

    return effect.DamageGiven < config.PoisonConfig.TotalDamage;
  }

  /// <summary>
  ///   Uses a poison shot for the player. Returns the remaining shots, -1 if none
  ///   are available.
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

  private HookResult onWeaponSound(UserMessage msg) {
    var defIndex = msg.ReadUInt("item_def_index");

    if (!WeaponSoundIndex.PISTOLS.Contains(defIndex))
      return HookResult.Continue;
    var splits = msg.DebugString.Split("\n");
    if (splits.Length < 5) return HookResult.Continue;
    var angleLines = msg.DebugString.Split("\n")[1..4]
     .Select(s => s.Trim())
     .ToList();
    if (!angleLines[0].Contains('x') || !angleLines[1].Contains('y')
      || !angleLines[2].Contains('z'))
      return HookResult.Continue;
    var x      = float.Parse(angleLines[0].Split(' ')[1]);
    var y      = float.Parse(angleLines[1].Split(' ')[1]);
    var z      = float.Parse(angleLines[2].Split(' ')[1]);
    var vec    = new Vector(x, y, z);
    var player = findPlayerByCoord(vec);

    if (player == null) return HookResult.Continue;
    if (converter.GetPlayer(player) is not IOnlinePlayer apiPlayer)
      return HookResult.Continue;

    if (poisonShots.TryGetValue(apiPlayer, out var shots) && shots > 0) {
      msg.Recipients.Clear();
      return HookResult.Handled;
    }

    return HookResult.Continue;
  }

  private CCSPlayerController? findPlayerByCoord(Vector vec) {
    foreach (var pl in Utilities.GetPlayers()) {
      var origin = pl.GetEyePosition();
      if (origin == null) continue;
      var dist = vec.DistanceSquared(origin);
      if (dist < 1) return pl;
    }

    return null;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRagdollSpawn(BodyCreateEvent ev) {
    if (!killedWithPoison.TryGetValue(ev.Body.OfPlayer.Id, out var shooter))
      return;
    if (ev.Body.Killer != null && ev.Body.Killer.Id != ev.Body.OfPlayer.Id)
      return;
    ev.Body.Killer = shooter as IOnlinePlayer;
  }

  private class PoisonEffect(IPlayer player, IPlayer shooter) {
    public IPlayer Player { get; } = player;
    public IPlayer Shooter { get; } = shooter;
    public int Ticks { get; set; }
    public int DamageGiven { get; set; }
  }
}