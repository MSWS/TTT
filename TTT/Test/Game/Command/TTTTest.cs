using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.Game.Commands;
using Xunit;

namespace TTT.Test.Game.Command;

public class TTTTest {
  private readonly ICommandManager commands;
  private readonly IServiceProvider provider;

  public TTTTest(IServiceProvider provider) {
    this.provider = provider;
    commands      = provider.GetRequiredService<ICommandManager>();

    commands.RegisterCommand(new TTTCommand(provider));
  }

  [Fact]
  public void Command_ShouldPrint_Version() {
    var player = TestPlayer.Random();

    commands.ProcessCommand(new TestCommandInfo(provider, player, "ttt"));

    Assert.Single(player.Messages);
  }
}