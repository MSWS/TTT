using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Player;
using TTT.Karma;

namespace TTT.CS2.GameHandlers;

public class KarmaSyncer(IServiceProvider provider) : IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IKarmaService? karma = provider.GetService<IKarmaService>();

  private readonly IPlayerFinder players =
    provider.GetRequiredService<IPlayerFinder>();

  public void Dispose() { }
  public string Id => nameof(KarmaSyncer);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    if (karma == null) return HookResult.Continue;

    foreach (var p in Utilities.GetPlayers()) {
      if (!p.IsValid || p.IsBot) continue;

      var apiPlayer = converter.GetPlayer(p);
      Task.Run(async () => {
        var pk = await karma.Load(apiPlayer);

        await Server.NextFrameAsync(() => {
          p.Score = pk;
          Utilities.SetStateChanged(p, "CCSPlayerController",
            "m_pActionTrackingServices");
        });
      });
    }

    return HookResult.Continue;
  }
}