using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;

namespace TTT.Karma;

public static class KarmaServiceCollection {
  public static void AddKarmaService(this IServiceCollection collection) {
    collection.AddModBehavior<IKarmaService, KarmaStorage>();
    collection.AddModBehavior<KarmaListener>();
  }
}