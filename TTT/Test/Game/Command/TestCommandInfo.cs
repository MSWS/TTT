using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.Test.Game.Command;

public class TestCommandInfo(IServiceProvider provider, IOnlinePlayer? caller,
  params string[] args) : ICommandInfo {
  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  public IOnlinePlayer? CallingPlayer { get; init; } = caller;
  public string[] Args { get; } = args;
  public CommandCallingContext CallingContext { get; set; }

  public string CommandString => string.Join(' ', Args);
  public int ArgCount => Args.Length;

  public void ReplySync(string message) {
    messenger.Message(CallingPlayer, message);
  }

  public ICommandInfo Skip(int count = 1) {
    return new TestCommandInfo(provider, CallingPlayer,
      Args.Skip(count).ToArray());
  }
}