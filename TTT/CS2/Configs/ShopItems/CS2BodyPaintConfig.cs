using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2BodyPaintConfig : IStorage<BodyPaintConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_bodypaint_price", "Price of the Body Paint item", 40,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_MAX_USES = new(
    "css_ttt_shop_bodypaint_max_uses",
    "Maximum number of times the Body Paint can be applied per item", 4,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 100));

  public static readonly FakeConVar<string> CV_COLOR = new(
    "css_ttt_shop_bodypaint_color",
    "Color to apply to the player's body (HTML hex or known color name)",
    "GreenYellow", ConVarFlags.FCVAR_NONE);

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<BodyPaintConfig?> Load() {
    Color parsedColor;
    try { parsedColor = ColorTranslator.FromHtml(CV_COLOR.Value); } catch {
      try { parsedColor = Color.FromName(CV_COLOR.Value); } catch {
        parsedColor = Color.GreenYellow;
      }
    }

    var cfg = new BodyPaintConfig {
      Price        = CV_PRICE.Value,
      MaxUses      = CV_MAX_USES.Value,
      ColorToApply = parsedColor
    };

    return Task.FromResult<BodyPaintConfig?>(cfg);
  }
}