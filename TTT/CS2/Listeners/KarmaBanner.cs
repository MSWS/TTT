using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Karma;
using TTT.Karma.Events;
using TTT.Karma.lang;

namespace TTT.CS2.Listeners;

public class KarmaBanner(IServiceProvider provider) : BaseListener(provider) {
  private readonly KarmaConfig config =
    provider.GetService<IStorage<KarmaConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new KarmaConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly Dictionary<IPlayer, int> cooldownRounds = new();

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  private readonly Dictionary<IPlayer, DateTime> lastWarned = new();

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnKarmaUpdate(KarmaUpdateEvent ev) {
    if (ev.Karma < config.MinKarma) {
      issueKarmaBan(ev.Player);
      return;
    }

    if (ev.Karma >= config.KarmaTimeoutThreshold) return;
    var timeSinceLastWarn = DateTime.UtcNow
      - lastWarned.GetValueOrDefault(ev.Player, DateTime.MinValue);
    if (timeSinceLastWarn <= config.KarmaWarningWindow) return;

    issueKarmaWarning(ev.Player);
  }

  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    if (!cooldownRounds.TryGetValue(ev.Player, out var rounds) || rounds <= 0)
      return;
    Messenger.Message(ev.Player, Locale[KarmaMsgs.KARMA_WARNING(rounds)]);
    cooldownRounds[ev.Player]--;
    if (cooldownRounds[ev.Player] <= 0) cooldownRounds.Remove(ev.Player);
    ev.Role = new SpectatorRole(Provider);
  }

  private void issueKarmaBan(IPlayer player) {
    Server.NextWorldUpdate(() => {
      var userId = converter.GetPlayer(player);
      if (userId == null) return;
      Server.ExecuteCommand(string.Format(config.CommandUponLowKarma,
        userId.UserId));
      Task.Run(async () => await karma.Write(player, config.DefaultKarma));
    });
  }

  private void issueKarmaWarning(IPlayer player) {
    cooldownRounds[player] = config.KarmaRoundTimeout;
    lastWarned[player]     = DateTime.UtcNow;
  }
}