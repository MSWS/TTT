using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.CS2.Extensions;

namespace TTT.CS2.GameHandlers;

public class BodyHider(IServiceProvider provider) : IPluginModule {
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();
  public void Dispose() { }
  public string Name => nameof(BodyHider);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  [GameEventHandler]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;
    player.SetColor(Color.FromArgb(0, 0, 0, 0));
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnStart(EventRoundStart ev, GameEventInfo _) {
    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers())
        player.SetColor(Color.FromArgb(254, 255, 255, 255));
    });
    return HookResult.Continue;
  }
}