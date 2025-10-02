using TTT.API.Player;
using TTT.API.Role;

namespace ShopAPI;

public abstract class RoleRestrictedItem<T>(IServiceProvider provider)
  : BaseItem(provider) where T : IRole {
  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return Roles.GetRoles(player).Any(r => r is T) ?
      PurchaseResult.SUCCESS :
      PurchaseResult.WRONG_ROLE;
  }
}