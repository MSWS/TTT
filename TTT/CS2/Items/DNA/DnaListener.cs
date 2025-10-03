using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Detective;
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

  private static readonly string[] missingDnaExplanations = {
    "the killer used gloves... for their bullets",
    "the killer was very careful", "the killer wiped the weapon clean",
    "the killer retrieved the bullets", "the bullets disintegrated on impact",
    "the killer was GOATed", "but no DNA was found",
    "but legal litigation caused the DNA to be lost",
    "and confirmed they were dead", "and they will remember that", "good job"
  };

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

    if (body.Killer == null) {
      var explanation =
        missingDnaExplanations[
          Random.Shared.Next(missingDnaExplanations.Length)];
      Messenger.Message(player,
        Locale[
          DnaMsgs.SHOP_ITEM_DNA_SCANNED_OTHER(victimRole, body.OfPlayer,
            explanation)]);
      return;
    }

    if (body.Killer == body.OfPlayer) {
      Messenger.Message(player,
        Locale[
          DnaMsgs.SHOP_ITEM_DNA_SCANNED_SUICIDE(victimRole, body.OfPlayer)]);
      return;
    }

    Messenger.Message(player,
      Locale[
        DnaMsgs.SHOP_ITEM_DNA_SCANNED(victimRole, body.OfPlayer, body.Killer)]);
  }

  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    lastMessages.Clear();
  }
}