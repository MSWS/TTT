using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.CS2.Extensions;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.GameHandlers;

public class PropMover(IServiceProvider provider) : IPluginModule {
  public void Dispose() { }

  public string Name => nameof(PropMover);
  public string Version => GitVersionInformation.FullSemVer;

  private Dictionary<CCSPlayerController, CBaseEntity> playersPressingE = new();

  private HashSet<CBaseEntity> mapEntities = new();
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
      if (!interactEntities.Contains(ent.DesignerName)) continue;
      msg.Debug($"Added {ent.DesignerName} to interactable entities");
      // if (ent is CPhysicsPropMultiplayer propMultiplayer) {
      //   msg.DebugAnnounce(
      //     $"Added {propMultiplayer.DesignerName} to map entities");
      //   mapEntities.Add(propMultiplayer);
      //   continue;
      // }
      //
      // if (ent is CRagdollProp ragdollProp) {
      //   msg.DebugAnnounce($"Added {ragdollProp.DesignerName} to map entities");
      //   mapEntities.Add(ragdollProp);
      //   continue;
      // }
      if (ent.DesignerName.Equals("prop_physics_multiplayer")) {
        var propMultiplayer = new CPhysicsPropMultiplayer(ent.Handle);
        msg.DebugAnnounce(
          $"Added {propMultiplayer.DesignerName} to map entities {propMultiplayer.AbsOrigin}");
        mapEntities.Add(propMultiplayer);
        continue;
      }

      msg.Debug(
        $"Skipping {ent.DesignerName} as it is not a valid interactable entity");
    }
  }

  private void buttonsChanged(CCSPlayerController player, PlayerButtons pressed,
    PlayerButtons released) {
    if (playersPressingE.ContainsKey(player)) {
      if (!released.HasFlag(PlayerButtons.Use)) return;
      return;
    }

    var playerPos = player.PlayerPawn.Value?.AbsOrigin;
    if (playerPos == null) return;

    if (pressed.HasFlag(PlayerButtons.Use)) {
      msg.DebugAnnounce("Player pressed 'E' key");
      var target = RayTrace.FindRayTraceIntersection(player);
      if (target == null) {
        msg.DebugInform("No target found for player");
        return;
      }

      msg.DebugInform(target.ToString());

      CBaseEntity? foundEntity = null;
      foreach (var ent in mapEntities) {
        if (!ent.IsValid) {
          msg.Debug($"Entity {ent.DesignerName} is not valid, skipping");
          continue;
        }

        var entPos = ent.AbsOrigin;
        if (entPos == null) continue;
        var dist = entPos.DistanceSquared(target);
        msg.DebugInform($"Distance from player to {ent.DesignerName}: {dist}");
        if (dist < 500) {
          foundEntity = ent;
          break;
        }
      }

      if (foundEntity == null) {
        msg.DebugInform("No interactable entity found within range");
        return;
      }

      playersPressingE[player] = foundEntity;
    }
  }

  private void onTick() {
    foreach (var (player, ent) in playersPressingE) onTick(player, ent);
  }

  private void onTick(CCSPlayerController player, CBaseEntity ent) {
    if (!player.IsValid || !ent.IsValid) {
      playersPressingE.Remove(player);
      return;
    }

    var playerPawn = player.PlayerPawn.Value;
    if (playerPawn == null || !playerPawn.IsValid) return;
    var playerOrigin = playerPawn?.AbsOrigin;
    if (playerOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    var entOrigin = ent.AbsOrigin;
    if (entOrigin == null) {
      playersPressingE.Remove(player);
      return;
    }

    var dist      = playerOrigin.DistanceSquared(entOrigin);
    var eyeAngles = playerPawn!.EyeAngles;

    var addedDist = playerOrigin + new Vector(
      (float)Math.Sin(eyeAngles.X * Math.PI / 180) * 10,
      (float)Math.Cos(eyeAngles.Y * Math.PI / 180) * 10, 0f);

    ent.Teleport(addedDist);
  }
}