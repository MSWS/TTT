using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Storage;
using TTT.CS2.Validators;

namespace TTT.CS2.Configs.ShopItems;

public class CS2M4A1Config : IStorage<M4A1Config>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_m4a1_price", "Price of the M4A1 item", 90,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<string> CV_CLEAR_SLOTS = new(
    "css_ttt_shop_m4a1_clear_slots",
    "Slots to clear when granting M4A1 (comma-separated ints)", "0,1");

  public static readonly FakeConVar<string> CV_WEAPONS = new(
    "css_ttt_shop_m4a1_weapons",
    "Weapons granted with this item (comma-separated names)",
    "weapon_m4a1,weapon_usp_silencer", ConVarFlags.FCVAR_NONE,
    new ItemValidator(allowMultiple: true));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<M4A1Config?> Load() {
    var slots = CV_CLEAR_SLOTS.Value.Split(',')
     .Select(s => s.Trim())
     .Where(s => int.TryParse(s, out _))
     .Select(int.Parse)
     .ToArray();

    var weapons = CV_WEAPONS.Value.Split(',')
     .Select(s => s.Trim())
     .Where(s => !string.IsNullOrEmpty(s))
     .ToArray();

    var cfg = new M4A1Config {
      Price = CV_PRICE.Value, ClearSlots = slots, Weapons = weapons
    };

    return Task.FromResult<M4A1Config?>(cfg);
  }
}