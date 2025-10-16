using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Shop.Items;
using Xunit;

namespace TTT.Test.Shop.Items;

public class DeagleTests {
  private readonly IEventBus bus;
  private readonly OneShotDeagleItem item;
  private readonly IServiceProvider provider;
  private readonly IShop shop;
  private readonly TestPlayer testPlayer;
  private readonly IOnlinePlayer victim, survivor;

  public DeagleTests(IServiceProvider provider) {
    this.provider = provider;
    var games  = provider.GetRequiredService<IGameManager>();
    var finder = provider.GetRequiredService<IPlayerFinder>();
    shop = provider.GetRequiredService<IShop>();
    bus  = provider.GetRequiredService<IEventBus>();
    item = new OneShotDeagleItem(provider);

    testPlayer = (finder.AddPlayer(TestPlayer.Random()) as TestPlayer)!;
    victim     = finder.AddPlayer(TestPlayer.Random());
    survivor   = finder.AddPlayer(TestPlayer.Random());

    bus.RegisterListener(new DeagleDamageListener(provider));
    bus.RegisterListener(new TestDamageApplier(provider));
    games.CreateGame()?.Start();
  }

  [Fact]
  public void Deagle_Kills_OnDamage() {
    shop.GiveItem(testPlayer, item);

    var playerDmgEvent =
      new PlayerDamagedEvent(victim, testPlayer, 1) { Weapon = item.WeaponId };
    bus.Dispatch(playerDmgEvent);

    Assert.Equal(0, victim.Health);
    Assert.False(victim.IsAlive);
  }

  [Fact]
  public void Deagle_DoesNotKill_AfterFirstKill() {
    shop.GiveItem(testPlayer, item);

    var playerDmgEvent =
      new PlayerDamagedEvent(victim, testPlayer, 1) { Weapon = item.WeaponId };
    bus.Dispatch(playerDmgEvent);

    var secondDmgEvent =
      new PlayerDamagedEvent(survivor, testPlayer, 1) {
        Weapon = item.WeaponId
      };
    bus.Dispatch(secondDmgEvent);

    Assert.NotEqual(0, survivor.Health);
  }
}