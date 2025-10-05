using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Karma;

namespace TTT.Shop.Listeners;

public class RoleAssignCreditor(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly ShopConfig config =
    provider.GetService<IStorage<ShopConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new ShopConfig(provider);

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IKarmaService? karmaService =
    provider.GetService<IKarmaService>();

  private readonly KarmaConfig karmaConfig =
    provider.GetService<IStorage<KarmaConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new KarmaConfig();

  [UsedImplicitly]
  [EventHandler]
  public async Task OnRoleAssign(PlayerRoleAssignEvent ev) {
    var toGive = config.StartingCreditsForRole(ev.Role);
    if (ev.Player is not IOnlinePlayer online) return;

    if (karmaService != null) {
      var karma = await karmaService.Load(ev.Player);
      var percent = (karma + karmaConfig.MinKarma)
        / (float)karmaConfig.MaxKarma(ev.Player);
      var givenScale = toGive * getKarmaScale(percent);
      toGive = (int)Math.Ceiling(givenScale);
    }

    shop.AddBalance(online, toGive, "Round Start", false);
  }

  private float getKarmaScale(float percent) {
    if (percent >= 0.9) return 1.1f;
    if (percent >= 0.8f) return 1;
    if (percent >= 0.5) return 0.8f;
    if (percent >= 0.3) return 0.5f;
    return 0.25f;
  }
}