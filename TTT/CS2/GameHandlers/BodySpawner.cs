using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;

namespace TTT.CS2.GameHandlers;

public class BodySpawner(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly PropMover mover = provider.GetRequiredService<PropMover>();

  public void Dispose() { }
  public string Name => nameof(BodySpawner);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  [GameEventHandler]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;
    player.SetColor(Color.FromArgb(0, 0, 0, 0));

    var ragdollBody = makeGameRagdoll(player);
    var body        = new CS2Body(ragdollBody, converter.GetPlayer(player));

    if (ev.Attacker != null && ev.Attacker.IsValid)
      body.WithKiller(converter.GetPlayer(ev.Attacker));

    body.WithWeapon(ev.Weapon);

    var bodyCreatedEvent = new BodyCreateEvent(body);
    bus.Dispatch(bodyCreatedEvent);

    if (bodyCreatedEvent.IsCanceled) {
      ragdollBody.AcceptInput("Kill");
      return HookResult.Continue;
    }

    mover.MapEntities.Add(ragdollBody);
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnStart(EventRoundStart ev, GameEventInfo _) {
    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers())
        player.SetColor(Color.FromArgb(254, 255, 255, 255));
    });
    return HookResult.Continue;
  }

  private CRagdollProp makeGameRagdoll(CCSPlayerController playerController) {
    var ragdoll = Utilities.CreateEntityByName<CRagdollProp>("prop_ragdoll");
    var pawn    = playerController.PlayerPawn.Value;

    if (ragdoll == null || !ragdoll.IsValid || playerController == null)
      throw new ArgumentNullException(nameof(ragdoll));

    if (pawn == null || !pawn.IsValid)
      throw new ArgumentException("PlayerPawn is not valid",
        nameof(playerController));

    var origin   = pawn.AbsOrigin.Clone();
    var rotation = pawn.AbsRotation.Clone();

    if (origin == null)
      throw new ArgumentException("PlayerPawn AbsOrigin is null",
        nameof(playerController));

    origin.Z      += 30;
    ragdoll.Speed =  0;

    ragdoll.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags =
      (uint)(ragdoll.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags
        & ~(1 << 2));
    ragdoll.SetModel(pawn.CBodyComponent!.SceneNode!.GetSkeletonInstance()
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
    ragdoll.Teleport(origin, rotation, Vector.Zero);
    Server.NextWorldUpdate(()
      => correctRagdoll(ragdoll, origin, rotation ?? QAngle.Zero, true));

    // TODO: See if we need to do this 4 times
    // for (var i = 0; i < 4; i++) {
    //   var j = i;
    //   Server.RunOnTick(Server.TickCount + i + 1,
    //     () => correctRagdoll(ragdoll, origin, rotation ?? QAngle.Zero, j == 0));
    // }

    return ragdoll;
  }

  private void correctRagdoll(CRagdollProp ragdoll, Vector origin,
    QAngle rotation, bool init = false) {
    if (!ragdoll.IsValid) return;
    if (init) {
      ragdoll.AcceptInput("ClearParent", null, ragdoll);
      ragdoll.MoveType = MoveType_t.MOVETYPE_FLY;
    }

    ragdoll.Teleport(origin, rotation, Vector.Zero);
  }
}