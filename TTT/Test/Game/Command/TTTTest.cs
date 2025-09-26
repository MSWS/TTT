using TTT.Game.Commands;
using Xunit;

namespace TTT.Test.Game.Command;

public class TTTTest(IServiceProvider provider)
  : CommandTest(provider, new TTTCommand(provider)) {
  [Fact]
  public void Command_ShouldPrint_Version() {
    var player = TestPlayer.Random();

    Commands.ProcessCommand(new TestCommandInfo(Provider, player,
      Command.Name));

    Assert.Single(player.Messages);
    Assert.Contains(Command.Version, player.Messages.First());
  }

  [Theory]
  [InlineData("modules", "Loaded Modules")]
  [InlineData("commands", "Registered Commands")]
  [InlineData("listeners", "Registered Listeners")]
  [InlineData("", "Unknown specification")]
  public void SubCommand_ShouldPrint_Modules(string cmd, string exp) {
    var player = TestPlayer.Random();

    Commands.ProcessCommand(new TestCommandInfo(Provider, player, Command.Name,
      cmd));

    Assert.Contains(exp, player.Messages.First());
  }
}