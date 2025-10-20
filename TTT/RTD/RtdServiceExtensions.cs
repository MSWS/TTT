using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;

namespace TTT.RTD;

public static class RtdServiceExtensions {
  public static void AddRtdServices(this IServiceCollection services) {
    services.AddModBehavior<IRewardGenerator, RewardGenerator>();
    services.AddModBehavior<RtdStatsCommand>();
    services.AddModBehavior<RTDCommand>();
  }
}