using CounterStrikeSharp.API;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Storage;
using TTT.Game.Listeners;
using TTT.Karma;
using TTT.Karma.Events;

namespace TTT.CS2.Listeners;

public class KarmaBanner(IServiceProvider provider) : BaseListener(provider) {
  private readonly KarmaConfig config =
    provider.GetService<IStorage<KarmaConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new KarmaConfig();

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnKarmaUpdate(KarmaUpdateEvent ev) {
    if (ev.Karma > config.MinKarma) return;

    Server.NextWorldUpdate(() => {
      Server.ExecuteCommand(string.Format(config.CommandUponLowKarma,
        ev.Player.Id));
      Task.Run(async () => await karma.Write(ev.Player, config.DefaultKarma));
    });
  }
}