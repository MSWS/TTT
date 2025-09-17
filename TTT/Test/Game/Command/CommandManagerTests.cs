using TTT.API.Command;
using TTT.Game.Commands;
using Xunit;

namespace TTT.Test.Game.Command;

public class CommandManagerTests(IServiceProvider provider) {
  private readonly ICommandManager manager = new CommandManager(provider);

  [Fact]
  public void RegisterCommand_AddsAllAliases() {
    var cmd    = new TestEchoCommand();
    var result = manager.RegisterCommand(cmd);
    Assert.True(result);
  }

  [Fact]
  public void UnregisterCommand_RemovesAllAliases() {
    var cmd = new TestEchoCommand();
    manager.RegisterCommand(cmd);
    var result = manager.UnregisterCommand(cmd);
    Assert.True(result);
  }

  [Fact]
  public async Task ProcessCommand_EchoesBackArgs() {
    var cmd = new TestEchoCommand();
    manager.RegisterCommand(cmd);

    var player = TestPlayer.Random();
    var info = new TestCommandInfo(provider, player, "echo", "hello", "world");

    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains("hello world", player.Messages);
  }

  [Fact]
  public async Task ProcessCommand_ReturnsUnknownCommand() {
    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "doesnotexist");

    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.UNKNOWN_COMMAND, result);
    Assert.Contains("doesnotexist", player.Messages.Single());
  }

  [Fact]
  public async Task ProcessCommand_HandlesAlias() {
    var cmd = new TestEchoCommand();
    manager.RegisterCommand(cmd);

    var player = TestPlayer.Random();
    var info   = new TestCommandInfo(provider, player, "say", "hi");
    var result = await manager.ProcessCommand(info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Equal("hi", player.Messages.Single());
  }

  [Fact]
  public void RegisterCommand_FailsOnDuplicateAlias() {
    var cmd1 = new TestEchoCommand();
    var cmd2 = new TestEchoCommand();

    var result1 = manager.RegisterCommand(cmd1);
    var result2 = manager.RegisterCommand(cmd2);

    Assert.True(result1);
    Assert.False(result2); // alias conflict
  }
}