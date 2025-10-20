using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.RTD;

public class RtdStatsCommand(IRewardGenerator generator) : ICommand {
  public void Dispose() { }
  public void Start() { }

  public string Id => "rtdstats";

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    var rewards = generator.ToList();
    var total   = rewards.Sum(r => r.Item2);

    var index = 0;
    foreach (var (reward, prob) in rewards) {
      var percent = (prob / total) * 100;
      info.ReplySync(
        $"{ChatColors.Orange}{index}. {reward.Name}{ChatColors.Grey}: {ChatColors.Yellow}{percent:0.00}%");
      index++;
    }

    return Task.FromResult(CommandResult.SUCCESS);
  }
}