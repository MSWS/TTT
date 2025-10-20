using ShopAPI;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Shop.Logs;

public class PurchaseAction(IRoleAssigner roles, IPlayer purchaser,
  IShopItem item) : IAction {
  public IPlayer Player { get; } = purchaser;
  public IPlayer? Other { get; }

  public IRole? PlayerRole { get; } =
    roles.GetRoles(purchaser).FirstOrDefault();

  public IRole? OtherRole { get; }
  public string Id { get; } = "ttt.shop.action.purchase";
  public string Verb { get; } = "purchased";
  public string Details { get; } = item.Name;
}