using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.Roles;
using TTT.Game;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class RoundTimerListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new GameConfig();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.COUNTDOWN) {
      Server.NextWorldUpdate(() => {
        RoundUtil.SetTimeRemaining((int)config.RoundCfg.CountDownDuration
         .TotalSeconds);
        Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
        foreach (var player in
          Utilities.GetPlayers().Where(p => !p.PawnIsAlive))
          player.Respawn();
      });

      return;
    }

    if (ev.NewState != State.IN_PROGRESS) return;
    Server.NextWorldUpdate(() => {
      RoundUtil.SetTimeRemaining((int)config.RoundCfg
       .RoundDuration(ev.Game.Players.Count)
       .TotalSeconds);
      Server.ExecuteCommand("mp_ignore_round_win_conditions 0");
    });
  }

  public static MemoryFunctionVoid<nint, float, RoundEndReason, nint, nint>
    TerminateRoundFunc =
      new(GameData.GetSignature("CCSGameRules_TerminateRound"));

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    if (RoundUtil.GetTimeRemaining() <= 1) return;

    foreach (var inno in ev.Game.GetAlive(typeof(InnocentRole))) {
      var player = converter.GetPlayer(inno);
      player?.SwitchTeam(CsTeam.CounterTerrorist);
    }

    var endReason =
      ev.Game.WinningRole != null && ev.Game.WinningRole.GetType()
       .IsAssignableTo(typeof(TraitorRole)) ?
        RoundEndReason.TerroristsWin :
        RoundEndReason.CTsWin;
    var gameRules = ServerExtensions.GetGameRulesProxy();
    if (gameRules == null || gameRules.GameRules == null) return;
    TerminateRoundFunc.Invoke(gameRules.GameRules.Handle, 5f, endReason, 0, 0);

    // RoundUtil.EndRound(
    //   ev.Game.WinningRole is TraitorRole ?
    //     RoundEndReason.TerroristsWin :
    //     RoundEndReason.CTsWin,
    //   (float)config.RoundCfg.TimeBetweenRounds.TotalSeconds);
  }
}