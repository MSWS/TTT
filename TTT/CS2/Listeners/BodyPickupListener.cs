using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.API;
using TTT.CS2.Events;
using TTT.CS2.Extensions;
using TTT.Game.Events.Body;
using TTT.Game.lang;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class BodyPickupListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IAliveSpoofer? spoofer =
    provider.GetService<IAliveSpoofer>();

  [UsedImplicitly]
  [EventHandler]
  public void OnPropPickup(PropPickupEvent ev) {
    if (!bodies.TryLookup(ev.Prop.Index.ToString(), out var body)) return;
    if (body == null || body.IsIdentified) return;
    if (ev.Player is not IOnlinePlayer online)
      throw new InvalidOperationException("Player is not an online player.");

    var identifyEvent = new BodyIdentifyEvent(body, online);

    Bus.Dispatch(identifyEvent);
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnIdentify(BodyIdentifyEvent ev) {
    ev.Body.IsIdentified = true;

    var role = Roles.GetRoles(ev.Body.OfPlayer);
    if (role.Count == 0) return;

    var primary = role.First();

    Messenger.MessageAll(Locale[
      GameMsgs.BODY_IDENTIFIED(ev.Identifier, ev.Body.OfPlayer, primary)]);

    if (!bodies.Bodies.TryGetValue(ev.Body, out var ragdoll)) return;

    if (ragdoll.IsValid) ragdoll.SetColor(primary.Color);

    var online = converter.GetPlayer(ev.Body.OfPlayer);
    if (online is not { IsValid: true }) return;

    if (primary is InnocentRole) online.SwitchTeam(CsTeam.CounterTerrorist);

    spoofer?.UnspoofAlive(online);
    online.PawnIsAlive = false;
    online.SetClan(primary.Name);
    Utilities.SetStateChanged(online, "CCSPlayerController", "m_bPawnIsAlive");
  }
}