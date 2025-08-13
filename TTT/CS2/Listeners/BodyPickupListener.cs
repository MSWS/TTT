using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
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
using TTT.Locale;

namespace TTT.CS2.Listeners;

public class BodyPickupListener(IServiceProvider provider) : IListener {
  private readonly Dictionary<CBaseEntity, IBody> bodyCache = new();
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Dispose() { bus.UnregisterListener(this); }

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
    var prop = ev.Prop as CRagdollProp;
    if (prop == null || !prop.IsValid) return;

    if (!bodyCache.TryGetValue(prop, out var body)) return;
    if (body.IsIdentified) return;

    if (ev.Player is not IOnlinePlayer online)
      throw new InvalidOperationException("Player is not an online player.");

    var identifyEvent = new BodyIdentifyEvent(body, online);

    bus.Dispatch(identifyEvent);
    if (identifyEvent.IsCanceled) return;

    body.IsIdentified = true;
    var role = roles.GetRoles(body.OfPlayer);
    if (role.Count == 0) return;
    var primaryRole = role.First();

    prop.SetColor(primaryRole.Color);
    msg.MessageAll(
      locale[GameMsgs.BODY_IDENTIFIED(online, body.OfPlayer, primaryRole)]);
  }
}