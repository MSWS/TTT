using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace Stats;

public class StatsCommand(IServiceProvider provider) : ICommand {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  public void Dispose() { }
  public void Start() { }

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    var response = await client.GetAsync("stats/players");

    return CommandResult.SUCCESS;
  }
}