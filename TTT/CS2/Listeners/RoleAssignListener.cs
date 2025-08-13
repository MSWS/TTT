using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.Hats;
using TTT.CS2.Roles;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class RoleAssignListener(IServiceProvider provider)
  : IPluginModule, IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ITextSpawner? textSpawner =
    provider.GetService<ITextSpawner>();

  private readonly ISet<CPointWorldText> traitorIcons =
    new HashSet<CPointWorldText>();

  private readonly ISet<int> traitors = new HashSet<int>();

  public void Dispose() { bus.UnregisterListener(this); }

  public string Name => nameof(RoleAssignListener);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.CheckTransmit>(
        onTransmit);

    bus.RegisterListener(this);
  }

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    traitors.Clear();
    traitorIcons.Clear();
  }

  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) {
      ev.Role = new SpectatorRole(provider);
      return;
    }

    traitors.Remove(player.Slot);

    player.SwitchTeam(ev.Role is CS2DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    player.SetClan(ev.Role is CS2DetectiveRole ? ev.Role.Name : "", false);
    var pawn = player.Pawn.Value;
    if (pawn == null || !pawn.IsValid) return;

    pawn.SetModel(ev.Role is CS2DetectiveRole ?
      "characters/models/ctm_fbi/ctm_fbi_varianth.vmdl" :
      "characters/models/tm_phoenix/tm_phoenix.vmdl");

    if (ev.Role is CS2InnocentRole) return;

    var textSettings = new TextSetting {
      msg = ev.Role.Name.First(char.IsAsciiLetter) + "", color = ev.Role.Color
    };
    var roleIcon = textSpawner?.CreateTextHat(textSettings, player);
    if (roleIcon == null) return;

    if (ev.Role is not CS2TraitorRole) return;
    traitors.Add(player.Slot);
    foreach (var icon in roleIcon) traitorIcons.Add(icon);
  }

  private void onTransmit(CCheckTransmitInfoList infoList) {
    foreach (var (info, player) in infoList) {
      if (player == null || !player.IsValid) continue;
      if (traitors.Contains(player.Slot)) continue;
      foreach (var icon in traitorIcons) info.TransmitEntities.Remove(icon);
    }
  }
}