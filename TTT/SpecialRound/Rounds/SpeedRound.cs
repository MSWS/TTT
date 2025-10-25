using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.CS2.Listeners;
using TTT.CS2.Utils;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class SpeedRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public override string Name => "Speed";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_SPEED;

  public override SpecialRoundConfig Config => config;

  private SpeedRoundConfig config
    => Provider.GetService<IStorage<SpeedRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SpeedRoundConfig();

  private IDisposable? endTimer;

  public override void ApplyRoundEffects() {
    Provider.GetService<RoundTimerListener>()?.EndTimer?.Dispose();

    Server.RunOnTick(Server.TickCount + 2,
      () => setTime(config.InitialSeconds));
  }

  private void addTime(TimeSpan span) {
    Server.NextWorldUpdate(() => {
      var remaining = RoundUtil.GetTimeRemaining();
      var newSpan   = TimeSpan.FromSeconds(remaining + (int)span.TotalSeconds);

      setTime(newSpan);
    });
  }

  private void setTime(TimeSpan span) {
    Server.NextWorldUpdate(() => {
      RoundUtil.SetTimeRemaining((int)span.TotalSeconds);
    });

    endTimer?.Dispose();
    endTimer = scheduler.Schedule(span,
      () => Server.NextWorldUpdate(() => games.ActiveGame?.EndGame(
        EndReason.TIMEOUT(new InnocentRole(Provider)))));
  }

  public override void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    endTimer?.Dispose();
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnDeath(PlayerDeathEvent ev) {
    var game = games.ActiveGame;
    if (game == null) return;
    if (Tracker.CurrentRound != this) return;

    var victimRoles = roles.GetRoles(ev.Victim);
    if (!victimRoles.Any(r => r is InnocentRole)) return;

    addTime(config.SecondsPerKill);
  }
}