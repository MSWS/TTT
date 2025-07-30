using TTT.Api.Player;

namespace TTT.Api;

public interface IAction {
  IPlayer Player { get; }
  IPlayer? Other { get; }
  string Id { get; }
  string Verb { get; }
  string Details { get; }

  public string Format() {
    return Other is not null ?
      $"{Player} {Verb} {Other} {Details}" :
      $"{Player} {Verb} {Details}";
  }
}