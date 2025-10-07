using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class BuyMenuHandler(IServiceProvider provider) : IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IInventoryManager inventory =
    provider.GetRequiredService<IInventoryManager>();

  public void Dispose() { }
  public void Start() { }

  private readonly Dictionary<string, string> shopAliases = new() {
    { "item_assaultsuit", "Armor" },
    { "item_kevlar", "Armor" },
    { "weapon_taser", "Taser" },
    { "weapon_deagle", "Revolver" },
    { "weapon_smokegrenade", "Poison Smoke" },
    { "weapon_m4a1_silencer", "M4A1" },
    { "weapon_usp_silencer", "M4A1" },
    { "weapon_mp5sd", "M4A1" },
    { "weapon_decoy", "healthshot" }
  };

  [UsedImplicitly]
  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPurchase(EventItemPurchase ev, GameEventInfo info) {
    if (ev.Userid == null) return HookResult.Continue;
    if (converter.GetPlayer(ev.Userid) is not IOnlinePlayer player)
      return HookResult.Continue;
    if (ev.Weapon is "item_assaultsuit" or "item_kevlar") {
      var user = ev.Userid;
      user.SetArmor(0);
    }

    inventory.RemoveWeapon(player, new BaseWeapon(ev.Weapon));

    if (shopAliases.TryGetValue(ev.Weapon, out var alias))
      ev.Userid.ExecuteClientCommandFromServer("css_buy " + alias);
    return HookResult.Handled;
  }
}