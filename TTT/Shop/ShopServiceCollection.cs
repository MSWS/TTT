using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Extensions;
using TTT.CS2.Items.Armor;
using TTT.CS2.Items.BodyPaint;
using TTT.CS2.Items.Camouflage;
using TTT.CS2.Items.DNA;
using TTT.CS2.Items.PoisonShots;
using TTT.CS2.Items.Station;
using TTT.Shop.Commands;
using TTT.Shop.Items;
using TTT.Shop.Items.Detective.Stickers;
using TTT.Shop.Items.M4A1;
using TTT.Shop.Items.Traitor.C4;
using TTT.Shop.Items.Traitor.Gloves;
using TTT.Shop.Listeners;

namespace TTT.Shop;

public static class ShopServiceCollection {
  public static void AddShopServices(this IServiceCollection collection) {
    collection.AddModBehavior<IShop, Shop>();

    collection.AddModBehavior<RoundShopClearer>();
    collection.AddModBehavior<RoleAssignCreditor>();

    collection.AddModBehavior<ShopCommand>();
    collection.AddModBehavior<BuyCommand>();
    collection.AddModBehavior<BalanceCommand>();

    collection.AddArmorServices();
    collection.AddBodyPaintServices();
    collection.AddC4Services();
    collection.AddCamoServices();
    collection.AddDamageStation();
    collection.AddDeagleServices();
    collection.AddDnaScannerServices();
    collection.AddGlovesServices();
    collection.AddHealthStation();
    collection.AddM4A1Services();
    collection.AddPoisonShots();
    collection.AddStickerServices();
  }
}