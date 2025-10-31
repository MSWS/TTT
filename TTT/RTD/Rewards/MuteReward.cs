using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Game;
using TTT.Locale;
using TTT.RTD.lang;

namespace TTT.RTD.Rewards;

public class MuteReward(IServiceProvider provider)
  : RoundStartReward(provider), IPluginModule {
  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMuted mutedPlayers = provider.GetRequiredService<IMuted>();
  public override string Name => "Mute";

  public override string Description
    => "you will not be able to communicate next round";

  public void Start(BasePlugin? plugin) {
    plugin?.RegisterListener<Listeners.OnClientVoice>(onVoice);
  }

  public override void GiveOnRound(IOnlinePlayer player) {
    mutedPlayers.Add(player.Id);
  }

  private void onVoice(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    if (player == null) return;

    if ((player.VoiceFlags & VoiceFlags.Muted) == VoiceFlags.Muted) return;

    if (mutedPlayers.Contains(player.SteamID.ToString())) {
      player.VoiceFlags |= VoiceFlags.Muted;
      player.PrintToChat(locale[RtdMsgs.RTD_MUTED]);
    }
  }

  public override void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.FINISHED) mutedPlayers.Clear();
    base.OnRoundStart(ev);
  }
}