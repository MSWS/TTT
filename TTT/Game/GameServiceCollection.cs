using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;

namespace TTT.Game;

public static class GameServiceCollection {
  public static void AddGameServices(this IServiceCollection collection) {
    collection.AddScoped<IEventBus, EventBus>();
    collection.AddScoped<IGameManager, GameManager>();
  }
}