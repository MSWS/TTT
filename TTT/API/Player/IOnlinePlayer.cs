namespace TTT.API.Player;

public interface IOnlinePlayer : IPlayer {
  // [Obsolete(
  //   "Roles are now managed via IRoleAssigner. Use IRoleAssigner.GetRoles(IPlayer) instead.")]
  // ICollection<IRole> Roles { get; }

  public int Health { get; set; }
  public int MaxHealth { get; set; }
  public int Armor { get; set; }
  public bool IsAlive { get; set; }
}