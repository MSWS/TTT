using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Body;

namespace TTT.Game.Actions;

public class IdentifyBodyAction(BodyIdentifyEvent ev) : IAction {
  public IPlayer Player { get; } = ev.Identifier;
  public IPlayer? Other { get; } = ev.Body.OfPlayer;
  public string Id { get; } = "basegame.action.identify_body";
  public string Verb { get; } = "identified the body of";
  public string Details { get; } = "";
}