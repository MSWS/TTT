using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.CS2.Events;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.GameHandlers;

public class PropMover(IServiceProvider provider) : IPluginModule {
  // TODO: Make this configurable
  public static readonly float MIN_LOOK_ACCURACY = 2000f;
  public static readonly float MAX_DISTANCE = 100f;
  public static readonly float MIN_HOLDING_DISTANCE = 100f;
  public static readonly float MAX_HOLDING_DISTANCE = 10000f;
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public readonly HashSet<CBaseEntity> MapEntities = [];
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly Dictionary<CCSPlayerController, MovementInfo>
    playersPressingE = new();

  public void Dispose() { }


  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.AddTimer(Server.TickInterval, refreshLines, TimerFlags.REPEAT);
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnTick>(
      refreshBodies);
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnPlayerButtonsChanged>(
        buttonsChanged);

    if (!hotReload) return;
    OnRoundStart(null!, null!);
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    var entities =
      Utilities.GetAllEntities().Where(ent => ent.IsValid).ToList();
    foreach (var propMultiplayer in from ent in entities
      where ent.DesignerName.Equals("prop_physics_multiplayer")
      select new CPhysicsPropMultiplayer(ent.Handle))
      MapEntities.Add(propMultiplayer);
    foreach (var propMultiplayer in from ent in entities
      where ent.DesignerName.Equals("prop_ragdoll")
      select new CRagdollProp(ent.Handle))
      MapEntities.Add(propMultiplayer);

    return HookResult.Continue;
  }

  private void buttonsChanged(CCSPlayerController player, PlayerButtons pressed,
    PlayerButtons released) {
    if (playersPressingE.TryGetValue(player, out var e)) {
      if (!released.HasFlag(PlayerButtons.Use)) return;
      playersPressingE.Remove(player);
      if (!e.Ragdoll.IsValid) return;
      e.Ragdoll.AcceptInput("EnableMotion");
      if (e.Beam != null && e.Beam.IsValid) e.Beam.AcceptInput("Kill");
      return;
    }

    var playerPos = player.PlayerPawn.Value?.AbsOrigin;
    if (playerPos == null) return;

    if (!pressed.HasFlag(PlayerButtons.Use)) return;

    var target = RayTrace.FindRayTraceIntersection(player);
    if (target == null) return;

    CBaseEntity? foundEntity = null;
    MapEntities.RemoveWhere(ent => !ent.IsValid);
    var closestDist = double.MaxValue;
    foreach (var ent in MapEntities) {
      if (!ent.IsValid) continue;
      var rayPointDist =
        ent.AbsOrigin?.DistanceSquared(target) ?? double.MaxValue;
      if (rayPointDist >= MIN_LOOK_ACCURACY || rayPointDist >= closestDist)
        continue;

      closestDist = rayPointDist;
      foundEntity = ent;
    }

    var playerDist = playerPos.Distance(target);
    if (playerDist > MAX_DISTANCE) return;
    if (foundEntity == null) return;

    var apiPlayer   = converter.GetPlayer(player);
    var pickupEvent = new PropPickupEvent(apiPlayer, foundEntity);
    bus.Dispatch(pickupEvent);
    if (pickupEvent.IsCanceled) return;

    playersPressingE[player] = new MovementInfo(playerDist, pickupEvent.Prop) {
      Beam = createBeam(playerPos, pickupEvent.Prop.AbsOrigin ?? Vector.Zero)
    };
    pickupEvent.Prop.AcceptInput("DisableMotion");
  }

  private void refreshBodies() {
    foreach (var (player, info) in playersPressingE)
      refreshBodies(player, info);
  }

  private void refreshBodies(CCSPlayerController player, MovementInfo info) {
    var ent = info.Ragdoll;
    if (!player.IsValid || !ent.IsValid) {
      playersPressingE.Remove(player);
      return;
    }

    var playerPawn = player.PlayerPawn.Value;
    if (playerPawn == null || !playerPawn.IsValid) return;
    var playerOrigin = playerPawn.AbsOrigin;
    if (playerOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    playerOrigin   =  playerOrigin.Clone()!;
    playerOrigin.Z += 64;

    var eyeAngles = playerPawn!.EyeAngles;

    var targetVector = playerOrigin + eyeAngles.Clone()!.ToForward()
      * Math.Clamp(info.Distance, MIN_HOLDING_DISTANCE, MAX_HOLDING_DISTANCE);

    targetVector.Z = Math.Max(targetVector.Z, playerOrigin.Z - 48);

    if (ent.AbsOrigin == null) return;
    var lerpedVector = ent.AbsOrigin.Lerp(targetVector, 0.3f);

    ent.Teleport(lerpedVector, QAngle.Zero, Vector.Zero);
  }

  private void refreshLines() {
    foreach (var (player, info) in playersPressingE) refreshLines(player, info);
  }

  private void refreshLines(CCSPlayerController player, MovementInfo info) {
    var ent = info.Ragdoll;
    if (!player.IsValid || !ent.IsValid) {
      playersPressingE.Remove(player);
      return;
    }

    var playerPawn = player.PlayerPawn.Value;
    if (playerPawn == null || !playerPawn.IsValid) return;
    var playerOrigin = playerPawn.AbsOrigin;
    if (playerOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    playerOrigin   =  playerOrigin.Clone()!;
    playerOrigin.Z += 64;

    var eyeAngles = playerPawn!.EyeAngles;

    var targetVector = playerOrigin + eyeAngles.Clone()!.ToForward()
      * Math.Clamp(info.Distance, MIN_HOLDING_DISTANCE, MAX_HOLDING_DISTANCE);

    targetVector.Z = Math.Max(targetVector.Z, playerOrigin.Z - 48);

    if (ent.AbsOrigin == null) return;
    var lerpedVector = ent.AbsOrigin.Lerp(targetVector, 0.3f);

    if (info.Beam != null && info.Beam.IsValid) {
      info.Beam.AcceptInput("Kill");
      info.Beam = createBeam(playerOrigin.With(z: playerOrigin.Z - 16),
        lerpedVector);
    }

    playersPressingE[player] = info;
  }

  private CEnvBeam? createBeam(Vector start, Vector end) {
    var beam = Utilities.CreateEntityByName<CEnvBeam>("env_beam");
    if (beam == null) return null;
    beam.RenderMode = RenderMode_t.kRenderTransColor;
    beam.Width      = 0.5f;
    beam.Render     = Color.White;
    beam.EndPos.X   = end.X;
    beam.EndPos.Y   = end.Y;
    beam.EndPos.Z   = end.Z;
    beam.Teleport(start);
    return beam;
  }
}