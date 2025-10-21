using TTT.API;
using TTT.API.Player;

namespace TTT.RTD;

public interface IRtdReward : ITerrorModule {
  string Name { get; }
  string Description { get; }
  void GrantReward(IOnlinePlayer player);
}