using TTT.API.Command;
using TTT.Game.Commands;
using Xunit;

namespace TTT.Test.Game.Command;

public class CommandManagerTests {
  private readonly CommandManager manager;

  public CommandManagerTests() {
    manager = new CommandManager(null!);
  }

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

    var player = new TestPlayer();
    var info = new TestCommandInfo(["echo", "hello", "world"]) {
      CallingPlayer = player
    };

    var result = await manager.ProcessCommand(player, info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Contains("hello world", info.Replies);
  }

  [Fact]
  public async Task ProcessCommand_ReturnsUnknownCommand() {
    var player = new TestPlayer();
    var info = new TestCommandInfo(["doesnotexist"]) {
      CallingPlayer = player
    };

    var result = await manager.ProcessCommand(player, info);

    Assert.Equal(CommandResult.UNKNOWN_COMMAND, result);
    Assert.Contains("doesnotexist", info.Replies[0]);
  }
  
  [Fact]
  public async Task ProcessCommand_HandlesAlias() {
    var cmd = new TestEchoCommand();
    manager.RegisterCommand(cmd);

    var info = new TestCommandInfo(["say", "hi"]) { CallingPlayer = new TestPlayer() };
    var result = await manager.ProcessCommand(info.CallingPlayer, info);

    Assert.Equal(CommandResult.SUCCESS, result);
    Assert.Equal("hi", info.Replies.Single());
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