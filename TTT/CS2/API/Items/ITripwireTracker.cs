namespace TTT.CS2.API.Items;

public interface ITripwireTracker {
  public List<TripwireInstance> ActiveTripwires { get; }
  void RemoveTripwire(TripwireInstance instance);
}