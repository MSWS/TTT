using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class BodySpawner(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }
  public void Start() { }

  [GameEventHandler]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo _) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS })
      return HookResult.Continue;
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;
    player.SetColor(Color.FromArgb(0, 0, 0, 0));

    var ragdollBody = makeGameRagdoll(player);
    var body = new CS2Body(provider, ragdollBody, converter.GetPlayer(player));

    if (ev.Attacker != null && ev.Attacker.IsValid)
      body.WithKiller(converter.GetPlayer(ev.Attacker));

    body.WithWeapon(new BaseWeapon(ev.Weapon));

    var bodyCreatedEvent = new BodyCreateEvent(body);
    bus.Dispatch(bodyCreatedEvent);

    if (bodyCreatedEvent.IsCanceled) ragdollBody.AcceptInput("Kill");
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
    var pawn    = playerController.Pawn.Value;

    if (ragdoll == null || !ragdoll.IsValid || playerController == null)
      throw new ArgumentNullException(nameof(ragdoll));

    if (pawn == null || !pawn.IsValid)
      throw new ArgumentException("Pawn is not valid",
        nameof(playerController));

    var origin   = pawn.AbsOrigin.Clone();
    var rotation = pawn.AbsRotation.Clone();

    if (origin == null)
      throw new ArgumentException("Pawn AbsOrigin is null",
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