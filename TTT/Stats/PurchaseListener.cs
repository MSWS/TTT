using System.Text;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Events;
using TTT.API.Events;

namespace Stats;

public class PurchaseListener(IServiceProvider provider) : IListener {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IRoundTracker? roundTracker =
    provider.GetService<IRoundTracker>();

  public void Dispose() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnPurchase(PlayerPurchaseItemEvent ev) {
    var body = new {
      steam_id = ev.Player.Id,
      item_id  = ev.Item.Id,
      round_id = roundTracker?.CurrentRoundId
    };

    var payload = new StringContent(JsonSerializer.Serialize(body),
      Encoding.UTF8, "application/json");

    Task.Run(async () => await client.PostAsync("purchase", payload));
  }
}