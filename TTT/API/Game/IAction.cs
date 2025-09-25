using TTT.API.Player;
using TTT.API.Role;

namespace TTT.API.Game;

public interface IAction {
  IPlayer Player { get; }
  IPlayer? Other { get; }
  IRole? PlayerRole { get; }
  IRole? OtherRole { get; }
  string Id { get; }
  string Verb { get; }
  string Details { get; }

  public string Format() {
    var pRole = PlayerRole != null ? $" [{PlayerRole.Name.First()}]" : "";
    var oRole = OtherRole != null ? $" [{OtherRole.Name.First()}]" : "";
    return Other is not null ?
      $"{Player}{pRole} {Verb} {Other}{oRole} {Details}" :
      $"{Player}{pRole} {Verb} {Details}";
  }
}