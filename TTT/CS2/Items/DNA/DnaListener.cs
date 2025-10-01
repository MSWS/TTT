using TTT.API.Events;
using TTT.CS2.Events;
using TTT.Game.Listeners;

namespace TTT.Shop.Items.Detective.DNA;

public class DnaListener(IServiceProvider provider) : BaseListener(provider) {
  // Low priority to allow body identification to happen first
  [EventHandler(Priority = Priority.LOW)]
  public void OnPropPickup(PropPickupEvent ev) {
    
  }
}