namespace TTT.Api.Player;

public interface IOnlinePlayer : IPlayer {
  ICollection<IRole> Roles { get; }
}