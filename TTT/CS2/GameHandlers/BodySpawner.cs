using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class BodySpawner(IServiceProvider provider) : BaseListener(provider) {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  [UsedImplicitly]
  [EventHandler]
  public void OnLeave(PlayerLeaveEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    spawnRagdoll(ev.Player);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnDeath(PlayerDeathEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    spawnRagdoll(ev.Player, ev.Killer, ev.Weapon);
  }

  private void spawnRagdoll(IPlayer apiPlayer, IOnlinePlayer? killer = null,
    string? weapon = null) {
    var player = converter.GetPlayer(apiPlayer);
    if (player == null || !player.IsValid) return;
    player.SetColor(Color.FromArgb(0, 255, 255, 255));

    var ragdollBody = makeGameRagdoll(player);
    var body        = new CS2Body(ragdollBody, converter.GetPlayer(player));

    if (killer != null) body.WithKiller(killer);

    body.WithWeapon(new BaseWeapon(weapon ?? "unknown"));

    var bodyCreatedEvent = new BodyCreateEvent(body);
    bus.Dispatch(bodyCreatedEvent);

    if (bodyCreatedEvent.IsCanceled) { ragdollBody.AcceptInput("Kill"); }
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnStart(EventRoundStart ev, GameEventInfo _) {
    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers())
        player.SetColor(Color.White);
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

    if (pawn.AbsOrigin == null || pawn.AbsRotation == null)
      throw new ArgumentException("Pawn AbsOrigin or AbsRotation is null",
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