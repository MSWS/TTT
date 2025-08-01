namespace TTT.Shop;

public enum PurchaseResult {
  /// <summary>
  /// The purchase was successful.
  /// </summary>
  SUCCESS,

  /// <summary>
  /// The player does not have enough funds to purchase the item.
  /// </summary>
  INSUFFICIENT_FUNDS,

  /// <summary>
  /// The item was not found in the shop or does not exist.
  /// </summary>
  ITEM_NOT_FOUND,

  /// <summary>
  /// The player cannot purchase this item, either due to per-player restrictions
  /// or the item being unavailable for purchase at the moment.
  /// </summary>
  ITEM_NOT_PURCHASABLE,

  /// <summary>
  /// An event canceled the purchase.
  /// </summary>
  PURCHASE_CANCELED,

  /// <summary>
  /// An unknown error occurred during the purchase process.
  /// </summary>
  UNKNOWN_ERROR,

  /// <summary>
  /// The item cannot be purchased multiple times, and the player already owns it.
  /// </summary>
  ALREADY_OWNED
}