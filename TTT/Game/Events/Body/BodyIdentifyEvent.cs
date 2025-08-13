using TTT.API.Events;
using TTT.API.Player;

namespace TTT.Game.Events.Body;

public class BodyIdentifyEvent(IBody body, IOnlinePlayer identifier)
  : BodyEvent(body), ICancelableEvent {
  public override string Id => "base.game.body.identify";
  public IOnlinePlayer Identifier { get; } = identifier;
  public bool IsCanceled { get; set; }
}