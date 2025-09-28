using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Hats;

public interface ITextSpawner {
  CPointWorldText CreateText(string text, Vector position, QAngle rotation) {
    return CreateText(new TextSetting { msg = text }, position, rotation);
  }

  CPointWorldText CreateText(TextSetting setting, Vector position,
    QAngle rotation);

  IEnumerable<CPointWorldText>
    CreateTextHat(string text, CCSPlayerController player) {
    return CreateTextHat(new TextSetting { msg = text }, player);
  }

  IEnumerable<CPointWorldText> CreateTextHat(TextSetting setting,
    CCSPlayerController player);
  
  IEnumerable<CPointWorldText> CreateTextScreen(TextSetting setting,
    CCSPlayerController player);
}