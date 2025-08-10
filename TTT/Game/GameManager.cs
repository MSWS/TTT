using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace TTT.Game;

public class GameManager(IServiceProvider provider) : IGameManager {
  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();
  protected readonly IServiceProvider Provider = provider;
  public IGame? ActiveGame { get; set; }

  public virtual IGame CreateGame() {
    ActiveGame = new RoundBasedGame(Provider);

    var ev = new GameInitEvent(ActiveGame);
    Bus.Dispatch(ev);

    return ActiveGame;
  }
}