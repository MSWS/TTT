using Microsoft.Extensions.DependencyInjection;
using TTT.Api.Events;
using TTT.Core.Events;

namespace TTT.Test;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IEventBus, EventBus>();
  }
}