using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace TTT.Game;

public class GameManager(IServiceProvider provider) : IGameManager {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  public IGame? ActiveGame { get; private set; }

  public IGame CreateGame() {
    ActiveGame = new RoundBasedGame(provider);

    var ev = new GameInitEvent(ActiveGame);
    bus.Dispatch(ev);
    
    return ActiveGame;
  }
}