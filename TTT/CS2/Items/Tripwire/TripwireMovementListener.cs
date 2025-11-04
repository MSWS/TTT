using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;
using TTT.CS2.Utils;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;

namespace TTT.CS2.Items.Tripwire;

public class TripwireMovementListener(IServiceProvider provider)
  : IPluginModule, IListener {
  private readonly TripwireItem? item = provider.GetService<TripwireItem>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    if (item == null) return;
    plugin?.AddTimer(0.1f, checkTripwires, TimerFlags.REPEAT);
  }

  private readonly Dictionary<string, TripwireItem.TripwireInstance>
    killedWithTripwire = new();

  private void checkTripwires() {
    if (item == null) return;
    var toRemove = new List<TripwireItem.TripwireInstance>();
    foreach (var wire in item.ActiveTripwires) {
      var ray = TraceRay.TraceShape(wire.StartPos, wire.EndPos, Contents.Player,
        wire.TripwireProp.Handle);
      if (!ray.DidHit() || !ray.HitPlayer(out _)) continue;

      toRemove.Add(wire);

      wire.TripwireProp.EmitSound("Flashbang.ExplodeDistant");
      doExplosion(wire);
    }

    foreach (var wire in toRemove) {
      item.ActiveTripwires.Remove(wire);
      wire.Beam.Remove();
      wire.TripwireProp.Remove();
    }
  }

  private void doExplosion(TripwireItem.TripwireInstance instance) {
    foreach (var player in finder.GetOnline()) {
      if (!player.IsAlive) continue;
      var gamePlayer = converter.GetPlayer(player);
      if (gamePlayer == null || gamePlayer.Pawn.Value == null) continue;
      if (gamePlayer.Pawn.Value.AbsOrigin == null) continue;

      var distance =
        instance.StartPos.Distance(gamePlayer.Pawn.Value.AbsOrigin);
      var damage = (int)Math.Round(getDamage(distance));
      if (damage < 1) continue;

      if (player.Health - damage <= 0) {
        killedWithTripwire[player.Id] = instance;
        var death = new PlayerDeathEvent(player).WithKiller(instance.owner)
         .WithWeapon("[Tripwire]");
        bus.Dispatch(death);
      } else {
        var damaged =
          new PlayerDamagedEvent(player, instance.owner, damage) {
            Weapon = "[Tripwire]"
          };
        bus.Dispatch(damaged);
      }

      player.Health -= damage;
      gamePlayer.EmitSound("Player.BurnDamage");
    }
  }

  private static readonly float fallofDelay = 0.01f;

  private float getDamage(float distance) {
    return 1000.0f * MathF.Pow(MathF.E, -distance * fallofDelay);
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
}