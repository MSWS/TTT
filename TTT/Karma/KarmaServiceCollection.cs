using Microsoft.Extensions.DependencyInjection;

namespace TTT.Karma;

public static class KarmaServiceCollection {
  public static void AddKarmaService(this IServiceCollection collection) {
    collection.AddScoped<IKarmaService, KarmaStorage>();
  }
}