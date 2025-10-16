using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class LogsCommand(IServiceProvider provider) : ICommand {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IIconManager? icons = provider.GetService<IIconManager>();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  public void Dispose() { }
  public string[] RequiredFlags => ["@ttt/admin"];

  public bool MustBeOnMainThread => true;

  public string Id => "logs";
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (games.ActiveGame is not {
      State: State.IN_PROGRESS or State.FINISHED
    }) {
      messenger.Message(executor, localizer[GameMsgs.GAME_LOGS_NONE]);
      return Task.FromResult(CommandResult.ERROR);
    }

    if (executor is { IsAlive: true }) {
      messenger.MessageAll(localizer[GameMsgs.LOGS_VIEWED_ALIVE(executor)]);
    } else if (icons != null && executor != null) {
      icons.SetVisiblePlayers(executor, ulong.MaxValue);
      messenger.Message(executor, localizer[GameMsgs.LOGS_VIEWED_INFO]);
    }

    games.ActiveGame.Logger.PrintLogs(executor);
    return Task.FromResult(CommandResult.SUCCESS);
  }
}