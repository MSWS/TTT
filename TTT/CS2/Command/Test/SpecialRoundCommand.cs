using Microsoft.Extensions.DependencyInjection;
using SpecialRound;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class SpecialRoundCommand(IServiceProvider provider) : ICommand {
  private readonly ISpecialRoundStarter tracker =
    provider.GetRequiredService<ISpecialRoundStarter>();

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (info.ArgCount == 1) {
      tracker.TryStartSpecialRound(null);
      info.ReplySync("Started a random special round.");
      return Task.FromResult(CommandResult.SUCCESS);
    }

    var rounds = provider.GetServices<ITerrorModule>()
     .OfType<AbstractSpecialRound>()
     .ToDictionary(r => r.GetType().Name.ToLower(), r => r);

    var roundName = info.Args[1].ToLower();
    if (!rounds.TryGetValue(roundName, out var round)) {
      info.ReplySync($"No special round found with name '{roundName}'.");
      foreach (var name in rounds.Keys) info.ReplySync($"- {name}");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    tracker.TryStartSpecialRound(round);
    info.ReplySync($"Started special round '{roundName}'.");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}