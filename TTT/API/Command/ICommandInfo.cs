using CounterStrikeSharp.API.Modules.Commands;
using TTT.API.Player;

namespace TTT.API.Command;

public interface ICommandInfo {
  string[] Args { get; }
  IOnlinePlayer? CallingPlayer { get; }
  CommandCallingContext CallingContext { get; set; }
  string CommandString => string.Join(' ', Args);
  int ArgCount => Args.Length;
  void ReplySync(string message);
  ICommandInfo Skip(int count = 1);
}