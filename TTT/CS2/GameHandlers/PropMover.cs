using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.CS2.Extensions;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.GameHandlers;

public class PropMover(IServiceProvider provider) : IPluginModule {
  private static readonly Vector ZERO_VECTOR = new(0, 0, 0);
  private static readonly QAngle ZERO_QANGLE = new(0, 0, 0);

  private readonly string[] interactEntities = [
    "prop_physics_multiplayer", "prop_ragdoll"
  ];

  private readonly HashSet<CBaseEntity> mapEntities = new();
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly Dictionary<CCSPlayerController, MovementInfo>
    playersPressingE = new();

  public void Dispose() { }

  public string Name => nameof(PropMover);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnTick>(
      onTick);
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnPlayerButtonsChanged>(
        buttonsChanged);

    if (!hotReload) return;
    OnRoundStart(null!, null!);
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    var entities = Utilities.GetAllEntities().ToList();
    foreach (var ent in entities) {
      if (!interactEntities.Contains(ent.DesignerName)) continue;
      msg.Debug($"Added {ent.DesignerName} to interactable entities");
      if (ent.DesignerName.Equals("prop_physics_multiplayer")) {
        var propMultiplayer = new CPhysicsPropMultiplayer(ent.Handle);
        msg.DebugAnnounce(
          $"Added {propMultiplayer.DesignerName} to map entities {propMultiplayer.AbsOrigin}");
        mapEntities.Add(propMultiplayer);
        continue;
      }

      msg.Debug(
        $"Skipping {ent.DesignerName} as it is not a valid interactable entity");
    }

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo _1) {
    var user = ev.Userid;
    if (user == null || !user.IsValid) return HookResult.Continue;
    if (user.PlayerPawn.Value == null) return HookResult.Continue;
    var ragdoll = CreateRagdoll(user);
    mapEntities.Add(ragdoll);
    return HookResult.Continue;
  }

  private void buttonsChanged(CCSPlayerController player, PlayerButtons pressed,
    PlayerButtons released) {
    if (playersPressingE.TryGetValue(player, out var e)) {
      if (!released.HasFlag(PlayerButtons.Use)) return;
      playersPressingE.Remove(player);
      if (!e.Ragdoll.IsValid) return;
      e.Ragdoll.AcceptInput("EnableMotion");
      return;
    }

    var playerPos = player.PlayerPawn.Value?.AbsOrigin;
    if (playerPos == null) return;

    if (pressed.HasFlag(PlayerButtons.Use)) {
      msg.DebugAnnounce("Player pressed 'E' key");
      var target = RayTrace.FindRayTraceIntersection(player);
      if (target == null) {
        msg.DebugInform("No target found for player");
        return;
      }

      msg.DebugInform(target.ToString());

      CBaseEntity? foundEntity = null;
      var          dist        = double.MaxValue;
      foreach (var ent in mapEntities) {
        if (!ent.IsValid) {
          msg.Debug($"Entity {ent.DesignerName} is not valid, skipping");
          mapEntities.Remove(ent);
          continue;
        }

        var entPos = ent.AbsOrigin;
        if (entPos == null) continue;
        dist = entPos.DistanceSquared(target);
        msg.DebugInform($"Distance from player to {ent.DesignerName}: {dist}");
        if (dist < 500) {
          foundEntity = ent;
          break;
        }
      }

      var playerDist = playerPos.Distance(target);

      if (foundEntity == null) {
        msg.DebugInform("No interactable entity found within range");
        return;
      }

      msg.DebugAnnounce($"Moving entity: {foundEntity.DesignerName}");
      playersPressingE[player] = new MovementInfo {
        Distance = playerDist, Ragdoll = foundEntity
      };
      foundEntity.AcceptInput("DisableMotion");
    }
  }

  private void onTick() {
    foreach (var (player, info) in playersPressingE) onTick(player, info);
  }

  private void onTick(CCSPlayerController player, MovementInfo info) {
    var ent = info.Ragdoll;
    if (!player.IsValid || !ent.IsValid) {
      playersPressingE.Remove(player);
      return;
    }

    var playerPawn = player.PlayerPawn.Value;
    if (playerPawn == null || !playerPawn.IsValid) return;
    var playerOrigin = playerPawn?.AbsOrigin;
    if (playerOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    playerOrigin   = new Vector(playerOrigin.X, playerOrigin.Y, playerOrigin.Z);
    playerOrigin.Z += 64;

    var entOrigin = ent.AbsOrigin;
    if (entOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    var eyeAngles = playerPawn!.EyeAngles;

    var forward = new Vector(
      (float)Math.Cos(eyeAngles.Y * Math.PI / 180)
      * (float)Math.Cos(eyeAngles.X * Math.PI / 180),
      (float)Math.Sin(eyeAngles.Y * Math.PI / 180)
      * (float)Math.Cos(eyeAngles.X * Math.PI / 180),
      (float)-Math.Sin(eyeAngles.X * Math.PI / 180));
    var addedDist = playerOrigin
      + forward * Math.Clamp((float)info.Distance, 100, 100 * 100);

    var targetRay = RayTrace.FindRayTraceIntersection(player);
    if (targetRay != null) {
      msg.DebugInform($"Target ray found at {targetRay}");
      if (targetRay.LengthSqr() < Math.Pow(info.Distance, 2)) {
        msg.Debug("Target ray is within distance, moving to target ray");
        addedDist = playerOrigin + targetRay;
      }
    }

    ent.Teleport(addedDist, ZERO_QANGLE, ZERO_VECTOR);
  }

  public CRagdollProp CreateRagdoll(CCSPlayerController playerController) {
    var ragdoll = Utilities.CreateEntityByName<CRagdollProp>("prop_ragdoll");

    if (ragdoll == null || !ragdoll.IsValid || playerController == null)
      throw new ArgumentNullException(nameof(ragdoll));
    var playerOrigin = new Vector {
      X = playerController.PlayerPawn?.Value?.AbsOrigin!.X ?? 0,
      Y = playerController.PlayerPawn?.Value?.AbsOrigin!.Y ?? 0,
      Z = playerController.PlayerPawn?.Value?.AbsOrigin!.Z ?? 0
    };
    playerOrigin.Z += 30;
    ragdoll.Speed  =  0;
    //ragdoll.RagdollClientSide = false;
    ragdoll.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags =
      (uint)(ragdoll.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags
        & ~(1 << 2));
    ragdoll.SetModel(
      playerController.PlayerPawn!.Value!.CBodyComponent!.SceneNode!
       .GetSkeletonInstance()
       .ModelState.ModelName);
    ragdoll.DispatchSpawn();
    ragdoll.Collision.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DEBRIS;
    ragdoll.Collision.CollisionAttribute.CollisionGroup =
      (byte)CollisionGroup.COLLISION_GROUP_DEBRIS;
    ragdoll.Collision.SolidType = SolidType_t.SOLID_VPHYSICS;
    ragdoll.MoveType            = MoveType_t.MOVETYPE_VPHYSICS;
    Utilities.SetStateChanged(ragdoll, "CBaseEntity", "m_MoveType");
    ragdoll.Entity!.Name = "player_body__" + playerController.Index;
    ragdoll.Teleport(playerOrigin,
      playerController.PlayerPawn!.Value!.AbsRotation, ZERO_VECTOR);
    // ragdoll.AcceptInput("FollowEntity", playerController.PlayerPawn.Value,
    //   ragdoll, "!activator");
    // ragdoll.AcceptInput("EnableMotion");
    Server.NextFrame(() => {
      if (!ragdoll.IsValid) return;
      ragdoll.AcceptInput("ClearParent", null, ragdoll);
      ragdoll.MoveType = MoveType_t.MOVETYPE_FLY;
      ragdoll.Teleport(playerOrigin,
        playerController.PlayerPawn!.Value!.AbsRotation, ZERO_VECTOR);
      Server.NextFrame(() => {
        if (!ragdoll.IsValid) return;
        ragdoll.Teleport(playerOrigin,
          playerController.PlayerPawn!.Value!.AbsRotation, ZERO_VECTOR);
        Server.NextFrame(() => {
          if (!ragdoll.IsValid) return;
          ragdoll.Teleport(playerOrigin,
            playerController.PlayerPawn!.Value!.AbsRotation, ZERO_VECTOR);
        });
      });
    });
    return ragdoll;
  }
}