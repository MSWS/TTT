using TTT.CS2.Items.Tripwire;

namespace TTT.CS2.API.Items;

public interface ITripwireTracker {
  public List<TripwireInstance> ActiveTripwires { get; }
}