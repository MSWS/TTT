using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Storage;
using TTT.CS2.Validators;

namespace TTT.CS2.Configs.ShopItems;

public class CS2OneShotDeagleConfig : IStorage<OneShotDeagleConfig>,
  IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_onedeagle_price", "Price of the One-Shot Deagle item", 120,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<bool> CV_FRIENDLY_FIRE = new(
    "css_ttt_shop_onedeagle_ff",
    "Whether the One-Shot Deagle damages teammates");

  public static readonly FakeConVar<bool> CV_KILL_SHOOTER_ON_FF = new(
    "css_ttt_shop_onedeagle_kill_shooter_on_ff",
    "Whether the shooter is killed if they shoot a teammate", true);

  public static readonly FakeConVar<string> CV_WEAPON = new(
    "css_ttt_shop_onedeagle_weapon",
    "Weapon entity name used for the One-Shot Weapon", "weapon_revolver",
    ConVarFlags.FCVAR_NONE, new ItemValidator(allowEmpty: false));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) { plugin?.RegisterFakeConVars(this); }

  public Task<OneShotDeagleConfig?> Load() {
    var cfg = new OneShotDeagleConfig {
      Price            = CV_PRICE.Value,
      DoesFriendlyFire = CV_FRIENDLY_FIRE.Value,
      Weapon           = CV_WEAPON.Value,
      KillShooterOnFF  = CV_KILL_SHOOTER_ON_FF.Value
    };

    return Task.FromResult<OneShotDeagleConfig?>(cfg);
  }
}