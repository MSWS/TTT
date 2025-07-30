namespace TTT.Api;

public interface IGameManager {
  IGame? ActiveGame { get; }
  IGame? CreateGame();
}