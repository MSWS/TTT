namespace TTT.API;

public interface IGameManager {
  IGame? ActiveGame { get; }
  IGame? CreateGame();
}