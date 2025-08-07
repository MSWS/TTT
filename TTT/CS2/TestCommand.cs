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
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }

  public string Name => "test";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) {
      info.ReplySync("Spawning body");
      Server.NextWorldUpdate(() => {
        var gamePlayer = converter.GetPlayer(executor);
        if (gamePlayer == null || !gamePlayer.IsValid) return;
        if (gamePlayer.AbsOrigin == null) return;
        var ragdoll = CreateRagdoll(gamePlayer);
      });
    }

    return Task.FromResult(CommandResult.SUCCESS);
  }

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
      ragdoll.AcceptInput("ClearParent", null, ragdoll);
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