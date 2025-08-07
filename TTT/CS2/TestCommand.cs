using System.Globalization;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2;

public class TestCommand(IServiceProvider provider) : ICommand, IPluginModule {
  public void Dispose() { }

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  public string Name => "test";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.CheckTransmit>(
        checkTransmit);
  }

  private void checkTransmit(CCheckTransmitInfoList infoList) {
    foreach (var (info, player) in infoList) {
      // info.TransmitEntities.Remove();
    }
  }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) {
      info.ReplySync("Unknown command");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    Server.NextWorldUpdate(() => {
      var gameExecutor = converter.GetPlayer(executor);
      switch (info.Args[1].ToLower()) {
        case "body":
          info.ReplySync("Spawning body");
          if (gameExecutor == null || !gameExecutor.IsValid) return;
          if (gameExecutor.AbsOrigin == null) return;
          var ragdoll = CreateRagdoll(gameExecutor);
          break;
        case "alive":
          info.ReplySync("marking everyone alive");
          foreach (var gp in finder.GetOnline()
           .Select(p => converter.GetPlayer(p))
           .OfType<CCSPlayerController>()) {
            gp.PawnIsAlive = true;
            Utilities.SetStateChanged(gp, "CCSPlayerController",
              "m_bPawnIsAlive");
          }

          break;
        case "dead":
          info.ReplySync("marking everyone dead");

          foreach (var gp in finder.GetOnline()
           .Select(p => converter.GetPlayer(p))
           .OfType<CCSPlayerController>()) {
            gp.PawnIsAlive = false;
            // Utilities.SetStateChanged(gp, "CCSPlayerController",
            //   "m_bPawnIsAlive");
          }

          break;
        case "gettarget":
          var target = gameExecutor?.PlayerPawn.Value?.Target;
          info.ReplySync(target ?? "null");
          info.ReplySync(gameExecutor?.PlayerPawn.Value?.LookTargetPosition
           .ToString() ?? "null");
          break;
        case "getdeath":
          info.ReplySync("DeathInfoTime: "
            + gameExecutor?.PlayerPawn.Value?.DeathInfoTime.ToString(CultureInfo
             .CurrentCulture));
          info.ReplySync("DeathTime: "
            + gameExecutor?.PlayerPawn.Value?.DeathTime.ToString(CultureInfo
             .CurrentCulture));
          break;
      }
    });

    return Task.FromResult(CommandResult.SUCCESS);
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
    playerOrigin.Z += 20;
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
    ragdoll.Entity!.Name        = "player_body__" + playerController.Index;
    ragdoll.Teleport(playerOrigin,
      playerController.PlayerPawn!.Value!.AbsRotation, new Vector(0, 0, 0));
    // ragdoll.AcceptInput("FollowEntity", playerController.PlayerPawn.Value,
    //   ragdoll, "!activator");
    // ragdoll.AcceptInput("EnableMotion");
    Server.NextFrame(() => {
      if (!ragdoll.IsValid) return;
      ragdoll.AcceptInput("ClearParent", null, ragdoll, "", 0);
      ragdoll.MoveType = MoveType_t.MOVETYPE_VPHYSICS;
      ragdoll.Teleport(playerOrigin,
        playerController.PlayerPawn!.Value!.AbsRotation, new Vector(0, 0, 0));
      Server.NextFrame(() => {
        if (!ragdoll.IsValid) return;
        ragdoll.Teleport(playerOrigin,
          playerController.PlayerPawn!.Value!.AbsRotation, new Vector(0, 0, 0));
        Server.NextFrame(() => {
          if (!ragdoll.IsValid) return;
          ragdoll.Teleport(playerOrigin,
            playerController.PlayerPawn!.Value!.AbsRotation,
            new Vector(0, 0, 0));
        });
      });
    });
    return ragdoll;
  }
}