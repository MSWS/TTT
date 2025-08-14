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
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class RoleIconsHandler(IServiceProvider provider)
  : IPluginModule, IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IDictionary<int, IEnumerable<CPointWorldText>>
    detectiveIcons = new Dictionary<int, IEnumerable<CPointWorldText>>();

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ITextSpawner? textSpawner =
    provider.GetService<ITextSpawner>();

  private readonly IDictionary<int, IEnumerable<CPointWorldText>> traitorIcons =
    new Dictionary<int, IEnumerable<CPointWorldText>>();

  private readonly ISet<int> traitors = new HashSet<int>();

  public void Dispose() { bus.UnregisterListener(this); }

  public string Name => nameof(RoleIconsHandler);
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
    detectiveIcons.Clear();
  }

  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) {
      ev.Role = new SpectatorRole(provider);
      return;
    }

    traitorIcons.TryGetValue(player.Slot, out var icons);
    if (icons != null)
      foreach (var icon in icons) {
        if (!icon.IsValid) continue;
        icon.Remove();
      }

    traitors.Remove(player.Slot);

    player.SwitchTeam(ev.Role is DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    player.SetClan(ev.Role is DetectiveRole ? ev.Role.Name : "", false);
    var pawn = player.Pawn.Value;
    if (pawn == null || !pawn.IsValid) return;

    pawn.SetModel(ev.Role is DetectiveRole ?
      "characters/models/ctm_fbi/ctm_fbi_varianth.vmdl" :
      "characters/models/tm_phoenix/tm_phoenix.vmdl");

    if (ev.Role is InnocentRole) return;

    var textSettings = new TextSetting {
      msg = ev.Role.Name.First(char.IsAsciiLetter) + "", color = ev.Role.Color
    };
    var roleIcon = textSpawner?.CreateTextHat(textSettings, player);
    if (roleIcon == null) return;

    if (ev.Role is not TraitorRole) {
      detectiveIcons[player.Slot] = roleIcon;
      return;
    }

    traitors.Add(player.Slot);
    traitorIcons[player.Slot] = roleIcon;
  }

  [EventHandler(Priority = Priority.MONITOR)]
  public void OnDeath(PlayerDeathEvent ev) {
    var gamePlayer = players.GetPlayer(ev.Victim);
    if (gamePlayer == null || !gamePlayer.IsValid) return;

    detectiveIcons.TryGetValue(gamePlayer.Slot, out var icons);
    removeIcons(icons);
    if (!traitors.Contains(gamePlayer.Slot)) return;

    traitorIcons.TryGetValue(gamePlayer.Slot, out icons);
    removeIcons(icons);
  }

  public void RevealIconFor(CCSPlayerController player) {
    traitors.Add(player.Slot);
  }

  public void HideIconFor(CCSPlayerController player) {
    traitors.Remove(player.Slot);
  }

  private void removeIcons(IEnumerable<CPointWorldText>? icons) {
    if (icons == null) return;
    foreach (var icon in icons) {
      if (!icon.IsValid) continue;
      icon.Remove();
    }
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