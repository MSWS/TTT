using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command;

public class TestCommand(IServiceProvider provider) : ICommand, IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  public void Dispose() { }

  public string Name => "test";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) {
      info.ReplySync("Unknown command");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    Server.NextWorldUpdate(() => {
      var gameExecutor = converter.GetPlayer(executor);
      switch (info.Args[1].ToLower()) {
        case "alive":
          info.ReplySync("marking everyone alive");
          foreach (var gp in finder.GetOnline()
           .Select(p => converter.GetPlayer(p))
           .OfType<CCSPlayerController>()) {
            gp.PawnIsAlive = true;
            Utilities.SetStateChanged(gp, "CCSPlayerController",
              "m_bPawnIsAlive");
          }

          break;
        case "dead":
          info.ReplySync("marking everyone dead");

          foreach (var gp in finder.GetOnline()
           .Select(p => converter.GetPlayer(p))
           .OfType<CCSPlayerController>())
            gp.PawnIsAlive = false;
          // Utilities.SetStateChanged(gp, "CCSPlayerController",
          //   "m_bPawnIsAlive");
          break;
        case "gettarget":
          var target = gameExecutor?.PlayerPawn.Value?.Target;
          info.ReplySync(target ?? "null");
          info.ReplySync(gameExecutor?.PlayerPawn.Value?.LookTargetPosition
           .ToString() ?? "null");
          break;
        case "getdeath":
          info.ReplySync("DeathInfoTime: "
            + gameExecutor?.PlayerPawn.Value?.DeathInfoTime.ToString(CultureInfo
             .CurrentCulture));
          info.ReplySync("DeathTime: "
            + gameExecutor?.PlayerPawn.Value?.DeathTime.ToString(CultureInfo
             .CurrentCulture));
          break;
      }
    });

    return Task.FromResult(CommandResult.SUCCESS);
  }
}