using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Karma.lang;
using TTT.Locale;

namespace TTT.Karma;

public class KarmaCommand(IServiceProvider provider) : ICommand {
  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  public void Dispose() { }
  public void Start() { }

  public string Id => "karma";

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    var value = await karma.Load(executor);

    info.ReplySync(locale[KarmaMsgs.KARMA_COMMAND(value)]);
    return CommandResult.SUCCESS;
  }
}