using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Commands;
using TTT.Game.lang;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Game.Command;

public class LogsTest(IServiceProvider provider) : CommandTest(provider,
  new LogsCommand(provider)) {
  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  [Fact]
  public async Task LogsCommand_WithoutGame_PrintsNoActiveGame() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(Provider, player, Command.Id);
    var result = await Commands.ProcessCommand(info);
    Assert.Equal(CommandResult.ERROR, result);
    Assert.Single(player.Messages);
    Assert.Contains(locale[GameMsgs.GAME_LOGS_NONE], player.Messages);
  }

  [Fact]
  public async Task LogsCommand_WithGame_PrintsLogs() {
    var player = TestPlayer.Random();
    Provider.GetRequiredService<IPlayerFinder>()
     .AddPlayers(player, TestPlayer.Random());

    Provider.GetRequiredService<IGameManager>().CreateGame()?.Start();

    var info   = new TestCommandInfo(Provider, player, Command.Id);
    var result = await Commands.ProcessCommand(info);
    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains(locale[GameMsgs.GAME_LOGS_HEADER], player.Messages);
  }
}