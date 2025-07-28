namespace TTT.Api;

public interface IOnlinePlayer : IPlayer {
  ICollection<IRole> Roles { get; }
}