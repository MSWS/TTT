namespace TTT.Api;

public interface IAction {
  IPlayer Player { get; }
  IPlayer? Target { get; }
  string Id { get; }
  string Verb { get; }
  string Details { get; }

  string Format() {
    return Target is not null ?
      $"{Player} {Verb} {Target} {Details}" :
      $"{Player} {Verb} {Details}";
  }
}