using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Role;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Utils;

public class EntityNameHelper {
  public enum Role {
    Innocent, Traitor, Detective
  }

  private static readonly Vector RELAY_POSITION = new(69, 420, -60);

  public static void SetEntityName(CCSPlayerController player, IRole role) {
    switch (role) {
      case InnocentRole:
        SetEntityName(player, Role.Innocent);
        break;
      case TraitorRole:
        SetEntityName(player, Role.Traitor);
        break;
      case DetectiveRole:
        SetEntityName(player, Role.Detective);
        break;
    }
  }

  public static void SetEntityName(CCSPlayerController player, Role role) {
    Server.NextWorldUpdate(() => {
      var name = role switch {
        Role.Innocent  => "ttt_innocent_assigner",
        Role.Traitor   => "ttt_traitor_assigner",
        Role.Detective => "ttt_detective_assigner",
        _              => "ttt_innocent_assigner"
      };

      var entities =
        Utilities.FindAllEntitiesByDesignerName<CLogicRelay>("logic_relay");

      foreach (var ent in entities) {
        Server.PrintToChatAll($"Entity: {ent.Entity?.Name} at {ent.AbsOrigin}");
      }

      var entity = Utilities
       .FindAllEntitiesByDesignerName<CLogicRelay>("logic_relay")
       .FirstOrDefault(e
          => e.Entity?.Name == name
          && e.AbsOrigin.DistanceSquared(RELAY_POSITION) < 100);

      Server.PrintToChatAll(
        $"Using entity: {entity?.Entity?.Name} at {entity?.AbsOrigin}");

      entity?.AcceptInput("Trigger", player, player);
    });
  }
}