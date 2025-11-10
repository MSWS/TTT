using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.API.Items;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Tripwire;

public class TripwireMovementListener(IServiceProvider provider)
  : BaseListener(provider), IPluginModule, ITripwireActivator {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly Dictionary<string, TripwireInstance> killedWithTripwire =
    new();

  private readonly ITripwireTracker? tripwireTracker =
    provider.GetService<ITripwireTracker>();

  private TripwireConfig config
    => Provider.GetService<IStorage<TripwireConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TripwireConfig();

  public void Start(BasePlugin? plugin) {
    if (tripwireTracker == null) return;
    plugin?.AddTimer(0.2f, checkTripwires, TimerFlags.REPEAT);
  }

  public void ActivateTripwire(TripwireInstance instance) {
    tripwireTracker?.RemoveTripwire(instance);
    instance.TripwireProp.EmitSound("Flashbang.ExplodeDistant");

    foreach (var player in Finder.GetOnline()) {
      if (!dealTripwireDamage(instance, player, out var gamePlayer)) continue;
      gamePlayer.EmitSound("Player.BurnDamage");
    }
  }

  private void checkTripwires() {
    if (tripwireTracker == null) return;
    foreach (var wire in new List<TripwireInstance>(tripwireTracker
     .ActiveTripwires)) {
      var ray = TraceRay.TraceShape(wire.StartPos, wire.EndPos, Contents.Player,
        wire.TripwireProp.Handle);
      if (!ray.DidHit() || !ray.HitPlayer(out var player)) continue;

      if (!config.FriendlyFireTriggers && player != null) {
        var apiPlayer = converter.GetPlayer(player);
        var role      = Roles.GetRoles(apiPlayer);
        if (role.Any(r => r is TraitorRole)) continue;
      }

      ActivateTripwire(wire);
    }
  }

  private float getDamage(float distance) {
    return config.ExplosionPower
      * MathF.Pow(MathF.E, -distance * config.FalloffDelay);
  }

  private int getDamage(CCSPlayerController gamePlayer, IOnlinePlayer player,
    Vector tripwire) {
    var origin = gamePlayer.Pawn.Value?.AbsOrigin;
    if (origin == null) return 0;
    var distance = tripwire.Distance(origin);
    var damage   = getDamage(distance);
    if (Roles.GetRoles(player).Any(r => r is TraitorRole))
      damage *= config.FriendlyFireMultiplier;

    return (int)Math.Floor(damage);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    killedWithTripwire.Clear();
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRagdollSpawn(BodyCreateEvent ev) {
    if (!killedWithTripwire.TryGetValue(ev.Body.Id, out var info)) return;
    if (ev.Body.Killer != null && ev.Body.Killer.Id != ev.Body.OfPlayer.Id)
      return;
    ev.Body.Killer = info.owner;
  }

  private bool dealTripwireDamage(TripwireInstance instance,
    IOnlinePlayer player,
    [NotNullWhen(true)] out CCSPlayerController? gamePlayer) {
    gamePlayer = null;
    if (!player.IsAlive) return false;

    gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return false;
    var damage = getDamage(gamePlayer, player, instance.StartPos);

    if (damage < 1) return false;

    Event ev;
    if (player.Health - damage <= 0) {
      killedWithTripwire[player.Id] = instance;
      ev = new PlayerDeathEvent(player).WithKiller(instance.owner)
       .WithWeapon("[Tripwire]");
    } else {
      ev = new PlayerDamagedEvent(player, instance.owner, damage) {
        Weapon = "[Tripwire]"
      };
    }

    Bus.Dispatch(ev);

    player.Health -= damage;
    return true;
  }
}