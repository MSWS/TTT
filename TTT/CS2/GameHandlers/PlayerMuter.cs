using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.CS2.lang;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace TTT.CS2.GameHandlers;

public class PlayerMuter(IServiceProvider provider) : IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IGameManager game =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnClientVoice>(
        onVoice);
  }

  private void onVoice(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    if (player == null) return;

    if (player.Pawn.Value is { Health: > 0 }) return;

    if (game.ActiveGame is not { State: State.IN_PROGRESS }) return;

    if ((player.VoiceFlags & VoiceFlags.Muted) != VoiceFlags.Muted) {
      var apiPlayer = converter.GetPlayer(player);
      messenger.Message(apiPlayer, locale[CS2Msgs.DEAD_MUTE_REMINDER]);
    }

    player.VoiceFlags |= VoiceFlags.Muted;
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnSpawn(EventPlayerSpawn ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;
    player.VoiceFlags &= ~VoiceFlags.Muted;
    return HookResult.Continue;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnGameEvent(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    foreach (var p in Utilities.GetPlayers()) {
      p.VoiceFlags &= ~VoiceFlags.Muted;
    }
  }
}