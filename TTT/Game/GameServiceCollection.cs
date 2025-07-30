using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;

namespace TTT.Game;

public static class GameServiceCollection {
  public static void AddGameServices(this IServiceCollection collection) {
    collection.AddScoped<IEventBus, EventBus>();
  }
}