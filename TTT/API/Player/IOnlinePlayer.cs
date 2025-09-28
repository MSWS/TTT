namespace TTT.API.Player;

public interface IOnlinePlayer : IPlayer {
  public int Health { get; set; }
  public int MaxHealth { get; set; }
  public int Armor { get; set; }
  public bool IsAlive { get; set; }
}