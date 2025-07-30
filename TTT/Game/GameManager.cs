using TTT.Api;

namespace TTT.Game;

public class GameManager(IServiceProvider provider) : IGameManager {
  public IGame? ActiveGame { get; private set; }

  public IGame? CreateGame() {
    return ActiveGame = new RoundBasedGame(provider);
  }
}