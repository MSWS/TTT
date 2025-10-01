using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.API;
using TTT.CS2.Extensions;
using TTT.CS2.Hats;
using TTT.CS2.Roles;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class RoleIconsHandler(IServiceProvider provider)
  : BaseListener(provider), IPluginModule, IIconManager {
  private static readonly string CT_MODEL =
    "characters/models/ctm_fbi/ctm_fbi_varianth.vmdl";

  private static readonly string T_MODEL =
    "characters/models/tm_phoenix/tm_phoenix.vmdl";

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ITextSpawner? textSpawner =
    provider.GetService<ITextSpawner>();

  private readonly ulong[] visibilities = new ulong[64];

  private HashSet<int> traitorsThisRound = new();

  // private readonly IDictionary<int, IEnumerable<CPointWorldText>> icons =
  //   new Dictionary<int, IEnumerable<CPointWorldText>>();
  private readonly IEnumerable<CPointWorldText>?[] icons =
    new IEnumerable<CPointWorldText>[64];

  public void Start(BasePlugin? plugin) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.CheckTransmit>(
        onTransmit);
    foreach (var text in Utilities
     .FindAllEntitiesByDesignerName<CPointWorldText>("point_worldtext"))
      text.AcceptInput("Kill");
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    for (var i = 0; i < icons.Length; i++) removeIcon(i);
    ClearAllVisibility();
    traitorsThisRound.Clear();
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
    removeIcon(player.Slot);

    player.SwitchTeam(ev.Role is DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    player.SetClan(ev.Role is DetectiveRole ? ev.Role.Name : "", false);
    var pawn = player.Pawn.Value;
    if (pawn == null || !pawn.IsValid) return;

    pawn.SetModel(ev.Role is DetectiveRole ? CT_MODEL : T_MODEL);
    assignIcon(player, ev.Role);

    switch (ev.Role) {
      case DetectiveRole: {
        for (var i = 0; i < Server.MaxPlayers; i++)
          AddVisiblePlayer(i, player.Slot);
        break;
      }
      case TraitorRole: {
        traitorsThisRound.Add(player.Slot);

        foreach (var traitor in traitorsThisRound) {
          AddVisiblePlayer(traitor, player.Slot);
          AddVisiblePlayer(player.Slot, traitor);
        }

        break;
      }
    }
  }

  private void removeIcon(int slot) {
    var existing = icons[slot];
    if (existing == null) return;
    foreach (var ent in existing) {
      if (!ent.IsValid) continue;
      ent.AcceptInput("Kill");
    }

    icons[slot] = null;
  }

  private void assignIcon(CCSPlayerController player, IRole role) {
    var textSettings = new TextSetting {
      msg = role.Name.First(char.IsAsciiLetter).ToString(), color = role.Color
    };
    var roleIcon = textSpawner?.CreateTextHat(textSettings, player);
    if (roleIcon == null) return;
    icons[player.Slot] = roleIcon;
  }

  [EventHandler(Priority = Priority.MONITOR)]
  public void OnDeath(PlayerDeathEvent ev) {
    var gamePlayer = players.GetPlayer(ev.Victim);
    if (gamePlayer == null || !gamePlayer.IsValid) return;

    removeIcon(gamePlayer.Slot);
  }

  private void onTransmit(CCheckTransmitInfoList infoList) {
    foreach (var (info, player) in infoList) {
      if (player == null || !player.IsValid) continue;
      hideIcons(info, player.Slot);
    }
  }

  private void hideIcons(CCheckTransmitInfo info, int source) {
    var visible = visibilities[source];
    if (visible == ulong.MaxValue) return;
    for (var i = 0; i < icons.Length; i++) {
      if ((visible & 1UL << i) != 0) continue;
      var iconList = icons[i];
      if (iconList == null) continue;
      foreach (var icon in iconList) info.TransmitEntities.Remove(icon);
    }
  }

  public ulong GetVisiblePlayers(int client) {
    if (client < 1 || client >= visibilities.Length)
      throw new ArgumentOutOfRangeException(nameof(client));
    return visibilities[client];
  }

  public void SetVisiblePlayers(int client, ulong playersBitmask) {
    guardRange(client, nameof(client));
    visibilities[client] = playersBitmask;
  }

  public void RevealToAll(int client) {
    guardRange(client, nameof(client));
    for (var i = 0; i < visibilities.Length; i++)
      visibilities[i] |= 1UL << client;
  }

  public void AddVisiblePlayer(int client, int player) {
    guardRange(client, nameof(client));
    guardRange(player, nameof(player));
    visibilities[client] |= 1UL << player;
  }

  public void RemoveVisiblePlayer(int client, int player) {
    guardRange(client, nameof(client));
    guardRange(player, nameof(player));
    visibilities[client] &= ~(1UL << player);
  }

  private void guardRange(int index, string name) {
    if (index < 0 || index >= visibilities.Length)
      throw new ArgumentOutOfRangeException(name);
  }

  public void ClearAllVisibility() {
    Array.Clear(visibilities, 0, visibilities.Length);
  }
}