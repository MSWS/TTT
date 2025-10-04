using System.Drawing;
using CounterStrikeSharp.API.Core;

namespace TTT.CS2.Hats;

public class TextSetting {
  public Color color = Color.White;
  public float depthOffset = 0.0f;
  public bool enabled = true;
  public string fontName = "Arial";
  public float fontSize = 64;
  public bool fullbright = true;

  public PointWorldTextJustifyHorizontal_t horizontal =
    PointWorldTextJustifyHorizontal_t
     .POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER;

  public required string msg;

  public PointWorldTextReorientMode_t reorient =
    PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_NONE;

  public PointWorldTextJustifyVertical_t vertical =
    PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER;

  public float worldUnitsPerPx = 0.5f;
}