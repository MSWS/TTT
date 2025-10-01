using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.API;
using TTT.CS2.Events;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.DNA;

public class DnaListener(IServiceProvider provider) : BaseListener(provider) {
  private static readonly TimeSpan cooldown = TimeSpan.FromSeconds(15);

  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  private readonly DnaScannerConfig config = provider
   .GetService<IStorage<DnaScannerConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new DnaScannerConfig();

  private readonly Dictionary<string, DateTime> lastMessages = new();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  // Low priority to allow body identification to happen first
  [UsedImplicitly]
  [EventHandler(Priority = Priority.LOW)]
  public void OnPropPickup(PropPickupEvent ev) {
    if (ev.Player is not IOnlinePlayer player) return;
    if (!shop.HasItem<DnaScanner>(player)) return;

    if (!bodies.TryLookup(ev.Prop.Index.ToString(), out var body)) return;
    if (body == null) return;

    var victimRole = Roles.GetRoles(body.OfPlayer).FirstOrDefault();
    if (victimRole == null) return;

    if (lastMessages.TryGetValue(player.Id + "." + body.Id,
      out var lastMessageTime))
      if (DateTime.Now - lastMessageTime < cooldown)
        return;

    lastMessages[player.Id + "." + body.Id] = DateTime.Now;

    if (DateTime.Now - body.TimeOfDeath > config.DecayTime) {
      Messenger.Message(player,
        Locale[DnaMsgs.SHOP_ITEM_DNA_EXPIRED(victimRole, body.OfPlayer)]);
      return;
    }

    if (body.Killer == null)
      Messenger.Message(player,
        Locale[
          DnaMsgs.SHOP_ITEM_DNA_SCANNED_SUICIDE(victimRole, body.OfPlayer)]);
    else
      Messenger.Message(player,
        Locale[
          DnaMsgs.SHOP_ITEM_DNA_SCANNED(victimRole, body.OfPlayer,
            body.Killer)]);
  }

  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    lastMessages.Clear();
  }
}