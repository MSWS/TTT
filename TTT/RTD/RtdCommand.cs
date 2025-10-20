using JetBrains.Annotations;
using TTT.API;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Utils;
using TTT.Game.Events.Game;
using TTT.Locale;
using TTT.RTD.lang;

namespace TTT.RTD;

public class RTDCommand(IRewardGenerator generator, IPermissionManager perms,
  IMsgLocalizer locale) : ICommand, IPluginModule, IListener {
  private bool inBetweenRounds = true;

  private Dictionary<string, IRtdReward> playerRewards = new();

  public bool MustBeOnMainThread => true;

  public string Id => "rtd";

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    var bypass = perms.HasFlags(executor, "@css/root") && info.ArgCount == 2;
#if DEBUG
    bypass = true;
#endif

    if (!bypass && playerRewards.TryGetValue(executor.Id, out var existing)) {
      info.ReplySync(locale[RtdMsgs.RTD_ALREADY_ROLLED(existing)]);
      return Task.FromResult(CommandResult.SUCCESS);
    }

    if (!bypass) {
      if (!inBetweenRounds && !RoundUtil.IsWarmup() && executor.IsAlive) {
        info.ReplySync(locale[RtdMsgs.RTD_CANNOT_ROLL_YET]);
        return Task.FromResult(CommandResult.SUCCESS);
      }
    }

    var reward = generator.GetReward();
    if (bypass) {
      if (!int.TryParse(info.Args[1], out var slot)) {
        info.ReplySync("Invalid parameter: must be an integer.");
        return Task.FromResult(CommandResult.SUCCESS);
      }

      if (slot != -1) {
        var rewards = generator.ToList();

        if (slot < 0 || slot >= rewards.Count) {
          info.ReplySync(
            $"Invalid parameter: must be between 0 and {rewards.Count - 1}.");
          return Task.FromResult(CommandResult.SUCCESS);
        }

        reward = generator.ToList()[slot].Item1;
      }
    }

    info.ReplySync(locale[RtdMsgs.RTD_ROLLED(reward)]);
    reward.GrantReward(executor);
    playerRewards[executor.Id] = reward;
    return Task.FromResult(CommandResult.SUCCESS);
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameState(GameStateUpdateEvent ev) {
    switch (ev.NewState) {
      case State.FINISHED:
        inBetweenRounds = true;
        break;
      case State.IN_PROGRESS:
        inBetweenRounds = false;
        playerRewards.Clear();
        break;
    }
  }

  public void Dispose() { }
  public void Start() { }
}