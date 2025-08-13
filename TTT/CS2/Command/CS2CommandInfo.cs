using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.CS2.Player;

namespace TTT.CS2.Command;

/// <summary>
///   A CS2-specific implementation of <see cref="ICommandInfo" />
///   This is responsible for keeping track of who
///   executed a command, what the parameters were when executing,
///   and the execution context.
/// </summary>
public class CS2CommandInfo : ICommandInfo {
  private readonly IMessenger messenger;

  public CS2CommandInfo(IServiceProvider provider, IOnlinePlayer? executor,
    int offset = 0, params string[] args) {
    messenger     = provider.GetRequiredService<IMessenger>();
    CallingPlayer = executor;
    Args          = args.Skip(offset).ToArray();
    if (offset == 0 && Args.Length > 0) Args[0] = args[0].ToLower();
  }

  public CS2CommandInfo(IServiceProvider provider, CommandInfo info,
    int offset = 0) {
    messenger = provider.GetRequiredService<IMessenger>();
    if (info.CallingPlayer != null)
      CallingPlayer = new CS2Player(info.CallingPlayer);
    CallingContext = info.CallingContext;
    Args           = new string[info.ArgCount - offset];
    for (var i = 0; i < info.ArgCount - offset; i++)
      Args[i] = info.GetArg(i + offset);
    if (offset == 0 && Args.Length > 0) Args[0] = Args[0].ToLower();
  }

  public string this[int index] => Args[index];

  /// <summary>
  ///   The arguments the command consists of
  /// </summary>
  public string[] Args { get; }

  /// <summary>
  ///   The player that executed the command
  /// </summary>
  public IOnlinePlayer? CallingPlayer { get; }

  /// <summary>
  ///   The calling context of the command
  /// </summary>
  public CommandCallingContext CallingContext { get; set; }

  public int ArgCount => Args.Length;

  public string GetCommandString => string.Join(' ', Args);

  public void ReplySync(string message) {
    switch (CallingContext) {
      case CommandCallingContext.Chat:
        messenger.Message(CallingPlayer, message);
        break;
      case CommandCallingContext.Console:
        messenger.BackgroundMsg(CallingPlayer, message);
        break;
    }
  }
}