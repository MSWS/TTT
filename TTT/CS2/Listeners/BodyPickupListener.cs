using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Events;
using TTT.CS2.Extensions;
using TTT.Game;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.CS2.Listeners;

public class BodyPickupListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly Dictionary<CBaseEntity, IBody> bodyCache = new();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [EventHandler]
  public void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    bodyCache.Clear();
  }

  [EventHandler]
  public void OnBodyCreate(BodyCreateEvent ev) {
    if (!int.TryParse(ev.Body.Id, out var index))
      throw new ArgumentException(
        $"Body ID '{ev.Body.Id}' is not a valid entity index.");

    var entity = Utilities.GetEntityFromIndex<CRagdollProp>(index);

    if (entity == null || !entity.IsValid)
      throw new InvalidOperationException(
        $"Could not find valid entity for body ID '{ev.Body.Id}'.");

    bodyCache[entity] = ev.Body;
  }

  [EventHandler(Priority = Priority.HIGH)]
  public void OnPropPickup(PropPickupEvent ev) {
    if (!bodyCache.TryGetValue(ev.Prop, out var body)) return;
    if (body.IsIdentified) return;
    if (ev.Player is not IOnlinePlayer online)
      throw new InvalidOperationException("Player is not an online player.");

    var identifyEvent = new BodyIdentifyEvent(body, online);

    Bus.Dispatch(identifyEvent);
    if (identifyEvent.IsCanceled) return;

    body.IsIdentified = true;
    var role = Roles.GetRoles(body.OfPlayer);
    if (role.Count == 0) return;
    var primaryRole = role.First();

    Messenger.MessageAll(
      Locale[GameMsgs.BODY_IDENTIFIED(online, body.OfPlayer, primaryRole)]);

    var gameBody =
      Utilities.GetEntityFromIndex<CRagdollProp>(int.Parse(body.Id));
    if (gameBody != null && gameBody.IsValid)
      gameBody.SetColor(role.First().Color);

    var onlinePlayer = converter.GetPlayer(body.OfPlayer);
    if (onlinePlayer == null || !onlinePlayer.IsValid) return;

    if (primaryRole is InnocentRole)
      onlinePlayer.SwitchTeam(CsTeam.CounterTerrorist);

    onlinePlayer.PawnIsAlive = false;
    onlinePlayer.SetClan(primaryRole.Name);
    Utilities.SetStateChanged(onlinePlayer, "CCSPlayerController",
      "m_bPawnIsAlive");
  }
}