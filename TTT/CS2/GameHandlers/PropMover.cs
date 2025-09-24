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
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;
using TTT.CS2.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.GameHandlers;

public class PropMover(IServiceProvider provider) : IPluginModule {
  // TODO: Make this configurable
  public static readonly float MAX_DISTANCE = 200;
  public static readonly float MIN_HOLDING_DISTANCE = 80;
  public static readonly float MAX_HOLDING_DISTANCE = 150;
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly Dictionary<CCSPlayerController, MovementInfo>
    playersPressingE = new();

  private static QAngle DEAD_ANGLE = new(90, 45, 90);

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

    var target = player.GetGameTraceByEyePosition(TraceMask.MaskSolid,
      Contents.NoDraw, player);
    if (target == null) return;

    var endPos = new Vector(target.Value.EndPos.X, target.Value.EndPos.Y,
      target.Value.EndPos.Z);

    CBaseEntity? hitEntity = null;
    target.Value.HitEntityByDesignerName(out hitEntity, "prop_ragdoll");

    if (hitEntity == null || !hitEntity.IsValid)
      target.Value.HitEntityByDesignerName(out hitEntity,
        "prop_physics_multiplayer");

    var playerDist = target.Value.Distance();
    if (playerDist > MAX_DISTANCE) return;
    if (hitEntity == null) return;

    var apiPlayer   = converter.GetPlayer(player);
    var pickupEvent = new PropPickupEvent(apiPlayer, hitEntity);
    bus.Dispatch(pickupEvent);
    if (pickupEvent.IsCanceled) return;

    playersPressingE[player] = new MovementInfo(playerDist, pickupEvent.Prop) {
      Beam = createBeam(playerPos, pickupEvent.Prop.AbsOrigin ?? Vector.Zero)
    };
    pickupEvent.Prop.AcceptInput("DisableMotion");
  }

  private void refreshBodies() {
    foreach (var (player, info) in playersPressingE) refreshBody(player, info);
  }

  private void refreshBody(CCSPlayerController player, MovementInfo info) {
    var ent = info.Ragdoll;
    if (!player.IsValid || !ent.IsValid) {
      playersPressingE.Remove(player);
      return;
    }

    var playerPawn = player.PlayerPawn.Value;
    if (playerPawn == null || !playerPawn.IsValid) return;
    var playerOrigin = player.GetEyePosition();
    if (playerOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    var raytrace = player.GetGameTraceByEyePosition(TraceMask.MaskSolid,
      Contents.NoDraw, player);

    if (ent.AbsOrigin == null || raytrace == null) return;

    var isOnSelf =
      raytrace.Value.HitEntityByDesignerName(out CBaseEntity? hitEnt,
        ent.DesignerName);

    var endPos = raytrace.Value.EndPos.toVector();

    if (isOnSelf || raytrace.Value.Distance() > MAX_HOLDING_DISTANCE) {
      endPos = playerOrigin
        + playerPawn.EyeAngles.ToForward() * MAX_HOLDING_DISTANCE;
    }

    if (ent.DesignerName == "prop_physics_multiplayer") {
      ent.Teleport(endPos, QAngle.Zero, Vector.Zero);
      return;
    }

    var deadRot = DEAD_ANGLE.Clone();

    var rotDeg = (Server.CurrentTime * 64f) % 360;
    var rotRad = (rotDeg + 0) * (MathF.PI / 180);
    deadRot.Y += rotDeg;

    var xOff  = MathF.Cos(rotRad) * 32;
    var yOff  = MathF.Sin(rotRad) * 32;
    var xBias = MathF.Cos(rotRad + MathF.PI / 2) * 16;
    var yBias = MathF.Sin(rotRad + MathF.PI / 2) * 16;
    endPos.X += xBias;
    endPos.Y += yBias;

    endPos -= new Vector(xOff, yOff, 0);

    ent.Teleport(endPos, deadRot, Vector.Zero);
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