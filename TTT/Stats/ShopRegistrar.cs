using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API;
using TTT.Game.Roles;

namespace Stats;

public class ShopRegistrar(IServiceProvider provider) : ITerrorModule {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { }

  public void Start() {
    Task.Run(async () => {
      await Task.Delay(TimeSpan.FromSeconds(1));
      List<Task> tasks = [];

      foreach (var item in shop.Items) {
        var data = new {
          item_name = item.Name, cost = item.Config.Price, team_restriction = ""
        };

        data = item switch {
          RoleRestrictedItem<TraitorRole> => data with {
            team_restriction = "traitor"
          },
          RoleRestrictedItem<DetectiveRole> => data with {
            team_restriction = "detective"
          },
          RoleRestrictedItem<InnocentRole> => data with {
            team_restriction = "innocent"
          },
          _ => data
        };

        var payload = new StringContent(JsonSerializer.Serialize(data),
          Encoding.UTF8, "application/json");

        Console.WriteLine(
          $"[Stats] Registering shop item '{item.Name}' at '{client.BaseAddress}item/{item.Id}'");

        tasks.Add(client.PutAsync("item/" + item.Id, payload));
      }

      await Task.WhenAll(tasks);
    });
  }
}