namespace TTT.API.Game;

public interface IGameManager {
  IGame? ActiveGame { get; }
  IGame? CreateGame();
}