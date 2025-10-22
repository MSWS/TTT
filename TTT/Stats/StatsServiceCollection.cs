using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;

namespace Stats;

public static class StatsServiceCollection {
  public static void AddStatsSerivces(this IServiceCollection collection) {
    var client = new HttpClient();
    client.BaseAddress = new Uri(StatsApi.API_URL!);

    collection.AddScoped<HttpClient>(_ => client);
    collection.AddModBehavior<PlayerCreationListener>();
    collection.AddModBehavior<IRoundTracker, RoundListener>();
    collection.AddModBehavior<ShopRegistrar>();
    collection.AddModBehavior<PurchaseListener>();
    collection.AddModBehavior<LogsUploader>();
    collection.AddModBehavior<KillListener>();
    collection.AddModBehavior<StatsCommand>();
    
    Console.WriteLine($"[Stats] Stats services registered with API URL: {client.BaseAddress.ToString()}");
  }
}