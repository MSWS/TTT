using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Player;

namespace TTT.RTD;

public static class RtdServiceExtensions {
  public static void AddRtdServices(this IServiceCollection services) {
    services.AddModBehavior<IRewardGenerator, RewardGenerator>();
    services.AddModBehavior<RtdStatsCommand>();
    services.AddModBehavior<RTDCommand>();
    services.AddSingleton<IMuted, Muted>();
  }
}