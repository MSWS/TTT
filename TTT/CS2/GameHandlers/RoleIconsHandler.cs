using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Extensions;
using TTT.CS2.Hats;
using TTT.CS2.Roles;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class RoleIconsHandler(IServiceProvider provider)
  : BaseListener(provider), IPluginModule {
  private readonly IDictionary<int, IEnumerable<CPointWorldText>>
    detectiveIcons = new Dictionary<int, IEnumerable<CPointWorldText>>();

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ITextSpawner? textSpawner =
    provider.GetService<ITextSpawner>();

  private readonly IDictionary<int, IEnumerable<CPointWorldText>> traitorIcons =
    new Dictionary<int, IEnumerable<CPointWorldText>>();

  private readonly ISet<int> traitors = new HashSet<int>();

  public void Start(BasePlugin? plugin) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.CheckTransmit>(
        onTransmit);
  }

  private static readonly string CT_MODEL =
    "characters/models/ctm_fbi/ctm_fbi_varianth.vmdl";

  private static readonly string T_MODEL =
    "characters/models/tm_phoenix/tm_phoenix.vmdl";

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    traitors.Clear();
    traitorIcons.Clear();
    detectiveIcons.Clear();
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) {
      ev.Role = new SpectatorRole(Provider);
      return;
    }

    // Remove in case we're re-assigning for some reason
    removeAllIcons(player);

    player.SwitchTeam(ev.Role is DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    player.SetClan(ev.Role is DetectiveRole ? ev.Role.Name : "", false);
    var pawn = player.Pawn.Value;
    if (pawn == null || !pawn.IsValid) return;

    pawn.SetModel(ev.Role is DetectiveRole ? CT_MODEL : T_MODEL);

    if (ev.Role is InnocentRole) return;

    assignIcon(player, ev.Role);
  }

  private void assignIcon(CCSPlayerController player, IRole role) {
    var textSettings = new TextSetting {
      msg = role.Name.First(char.IsAsciiLetter).ToString(), color = role.Color
    };
    var roleIcon = textSpawner?.CreateTextHat(textSettings, player);

    if (roleIcon == null) return;

    if (role is DetectiveRole) {
      detectiveIcons[player.Slot] = roleIcon;
      return;
    }

    traitors.Add(player.Slot);
    traitorIcons[player.Slot] = roleIcon;
  }

  private void removeAllIcons(CCSPlayerController player) {
    removeTraitorIcon(player);
    removeDetectiveIcon(player);
  }

  private void removeTraitorIcon(CCSPlayerController player) {
    removeIcons(player.Slot, traitorIcons);
  }

  private void removeDetectiveIcon(CCSPlayerController player) {
    removeIcons(player.Slot, detectiveIcons);
  }

  private void removeIcons(int slot,
    IDictionary<int, IEnumerable<CPointWorldText>> cache) {
    cache.Remove(slot, out var icons);
    if (icons == null) return;
    foreach (var icon in icons) {
      if (!icon.IsValid) continue;
      icon.Remove();
    }
  }

  [EventHandler(Priority = Priority.MONITOR)]
  public void OnDeath(PlayerDeathEvent ev) {
    var gamePlayer = players.GetPlayer(ev.Victim);
    if (gamePlayer == null || !gamePlayer.IsValid) return;

    removeAllIcons(gamePlayer);
  }

  // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
  private void onTransmit(CCheckTransmitInfoList infoList) {
    foreach (var (info, player) in infoList) {
      if (player == null || !player.IsValid) continue;
      if (traitors.Contains(player.Slot)) continue;
      foreach (var icon in traitorIcons.Values.SelectMany(s => s))
        info.TransmitEntities.Remove(icon);
    }
  }
}