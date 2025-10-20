namespace TTT.RTD;

public interface IRewardGenerator : IReadOnlyCollection<(IRtdReward, float)> {
  IRtdReward GetReward();
}