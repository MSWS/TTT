using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;

namespace TTT.CS2.GameHandlers;

public class PropMover(IServiceProvider provider) : IPluginModule {
  public void Dispose() { }

  public string Name => nameof(PropMover);
  public string Version => GitVersionInformation.FullSemVer;

  private Dictionary<CCSPlayerController, CEntityInstance> playersPressingE =
    new();

  private HashSet<CEntityInstance> mapEntities = new();
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  private readonly string[] interactEntities = [
    "prop_physics_multiplayer", "prop_ragdoll"
  ];

  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnTick>(
      onTick);
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnPlayerButtonsChanged>(
        buttonsChanged);
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnMapStart>(
      onMapStart);

    if (!hotReload) return;
    onMapStart("");
  }

  private void onMapStart(string mapName) {
    var entities = Utilities.GetAllEntities().ToList();
    msg.Debug($"Found {entities.Count} entities on map start");

    msg.Debug(
      $"Of those, found {entities.OfType<CPhysicsProp>().Count()} physic props");
    msg.Debug(
      $"Of those, found {entities.OfType<CPhysicsPropMultiplayer>().Count()} multiplayer physic props");

    foreach (var ent in entities) {
      msg.Debug($"{ent.GetType()} {ent.DesignerName} : {ent.Index}");
      if (interactEntities.Contains(ent.DesignerName)) {
        mapEntities.Add(ent);
        msg.Debug($"Added {ent.DesignerName} to interactable entities");
      }
    }
  }

  private void buttonsChanged(CCSPlayerController player, PlayerButtons pressed,
    PlayerButtons released) {
    if (playersPressingE.ContainsKey(player)) {
      if (!released.HasFlag(PlayerButtons.Use)) return;
      return;
    }

    if (pressed.HasFlag(PlayerButtons.Use)) {
      var target = RayTrace.FindRayTraceIntersection(player);

      foreach (var ent in mapEntities) {
        if (!ent.IsValid) continue;
        // Check if the entity is within range of the player
        // if (target != null && ent.AbsOrigin != null && ) < 100f) {
        //   playersPressingE[player] = ent;
        //   msg.Debug($"Player {player.Name} is pressing 'E' on {ent.DesignerName}");
        //   return;
        // }
      }
    }
  }

  private void onTick() {
    foreach (var player in playersPressingE) {
      // if (!player.IsValid) continue;

      // var pawn = player.PlayerPawn.Value;
      // if (pawn == null || !pawn.IsValid) continue;
      //
      // // Check if the player is pressing the 'E' key
      // if (player.IsKeyPressed(CounterStrikeSharp.API.Core.Key.E)) {
      //   // Move the prop forward
      //   pawn.Position += pawn.Forward * 10f; // Adjust the speed as needed
      // }
    }
  }
}