using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Events;
using TTT.API.Game;
using TTT.CS2.API;
using TTT.Game;
using TTT.Game.Events.Body;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;

namespace TTT.CS2.Listeners;

public class BodyTracker(IServiceProvider provider)
  : BaseListener(provider), IBodyTracker {
  public IDictionary<IBody, CRagdollProp> Bodies => bodyCache;
  private readonly Dictionary<IBody, CRagdollProp> bodyCache = new();

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

    bodyCache[ev.Body] = entity;
  }
}