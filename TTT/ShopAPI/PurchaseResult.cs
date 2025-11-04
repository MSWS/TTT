namespace ShopAPI;

public enum PurchaseResult {
  /// <summary>
  ///   The purchase was successful.
  /// </summary>
  SUCCESS,

  /// <summary>
  ///   The player does not have enough funds to purchase the item.
  /// </summary>
  INSUFFICIENT_FUNDS,

  /// <summary>
  ///   The item was not found in the shop or does not exist.
  /// </summary>
  ITEM_NOT_FOUND,

  /// <summary>
  ///   The player cannot purchase this item, either due to per-player restrictions
  ///   or the item being unavailable for purchase at the moment.
  /// </summary>
  ITEM_NOT_PURCHASABLE,

  /// <summary>
  ///   The player does not have the required role to purchase this item.
  /// </summary>
  WRONG_ROLE,

  /// <summary>
  ///   An event canceled the purchase.
  /// </summary>
  PURCHASE_CANCELED,

  /// <summary>
  ///   An unknown error occurred during the purchase process.
  /// </summary>
  UNKNOWN_ERROR,

  /// <summary>
  ///   The item cannot be purchased multiple times, and the player already owns it.
  /// </summary>
  ALREADY_OWNED,

  /// <summary>
  ///  The tripwire placement is too far from the player.
  /// </summary>
  TRIPWIRE_TOO_FAR
}

public static class PurchaseResultExtensions {
  public static string ToMessage(this PurchaseResult result) {
    return result switch {
      PurchaseResult.SUCCESS => "Purchase successful",
      PurchaseResult.INSUFFICIENT_FUNDS =>
        "You do not have enough funds to complete this purchase",
      PurchaseResult.ITEM_NOT_FOUND => "The item was not found in the shop",
      PurchaseResult.ITEM_NOT_PURCHASABLE =>
        "You cannot purchase this item at the moment",
      PurchaseResult.PURCHASE_CANCELED => "The purchase was canceled",
      PurchaseResult.UNKNOWN_ERROR =>
        "An unknown error occurred during the purchase",
      PurchaseResult.WRONG_ROLE =>
        "You do not have the required role to purchase this item",
      PurchaseResult.ALREADY_OWNED =>
        "You have purchased the maximum amount of this item",
      PurchaseResult.TRIPWIRE_TOO_FAR =>
        "The tripwire placement is too far from you",
      _ => "An unexpected error occurred"
    };
  }
}