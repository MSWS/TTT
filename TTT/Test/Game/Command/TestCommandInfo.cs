using CounterStrikeSharp.API.Modules.Commands;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.Test.Game.Command;

public class TestCommandInfo(string[] args) : ICommandInfo {
  public List<string> Replies { get; } = [];
  public IOnlinePlayer? CallingPlayer { get; set; }
  public string[] Args { get; } = args;
  public CommandCallingContext CallingContext { get; set; }

  public string GetCommandString => string.Join(' ', Args);
  public int ArgCount => Args.Length;

  public void ReplySync(string message) { Replies.Add(message); }
}