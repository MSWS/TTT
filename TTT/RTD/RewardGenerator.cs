using System.Collections;
using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.CS2.Items.Armor;
using TTT.CS2.Items.Camouflage;
using TTT.CS2.Items.Station;
using TTT.RTD.Rewards;
using TTT.Shop.Items;
using TTT.Shop.Items.Detective.Stickers;
using TTT.Shop.Items.Taser;

namespace TTT.RTD;

public class RewardGenerator(IServiceProvider provider)
  : IRewardGenerator, IPluginModule {
  private readonly List<(IRtdReward, float)> rewards = new();

  private const float PROB_LOTTERY = 1 / 5000f;
  private const float PROB_EXTREMELY_LOW = 1 / 800f;
  private const float PROB_VERY_LOW = 1 / 100f;
  private const float PROB_LOW = 1 / 20f;
  private const float PROB_MEDIUM = 1 / 10f;
  private const float PROB_OFTEN = 1 / 5f;
  private const float PROB_VERY_OFTEN = 1 / 2f;

  public IEnumerator<(IRtdReward, float)> GetEnumerator() {
    return rewards.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
  public int Count => rewards.Count;

  public IRtdReward GetReward() {
    var totalWeight = 0f;
    foreach (var (_, weight) in rewards) { totalWeight += weight; }

    var randomValue      = Random.Shared.NextSingle() * totalWeight;
    var cumulativeWeight = 0f;

    foreach (var (reward, weight) in rewards) {
      cumulativeWeight += weight;
      if (randomValue <= cumulativeWeight) { return reward; }
    }

    return rewards[^1].Item1;
  }

  public void Start() {
    rewards.AddRange([
      (new CreditReward(provider, 5), PROB_OFTEN),
      (new CreditReward(provider, -5), PROB_OFTEN),
      (new WeaponReward(provider, "weapon_flashbang"), PROB_MEDIUM),
      (new CreditReward(provider, -10), PROB_LOW),
      (new HealthReward(provider, 150), PROB_LOW),
      (new CreditReward(provider, -50), PROB_VERY_LOW),
      (new CreditReward(provider, 50), PROB_VERY_LOW),
      (new HealthReward(provider, 50), PROB_VERY_LOW),
      (new ShopItemReward<CamouflageItem>(provider), PROB_VERY_LOW),
      (new ShopItemReward<ArmorItem>(provider), PROB_VERY_LOW),
      (new ShopItemReward<TaserItem>(provider), PROB_VERY_LOW),
      (new ShopItemReward<Stickers>(provider), PROB_VERY_LOW),
      (new ShopItemReward<OneShotDeagleItem>(provider), PROB_VERY_LOW),
      (new ProvenReward(provider), PROB_VERY_LOW),
      (new MuteReward(provider), PROB_VERY_LOW),
      (new ShopItemReward<HealthStation>(provider), PROB_EXTREMELY_LOW),
      (new HealthReward(provider, 1), PROB_EXTREMELY_LOW),
      (new CreditReward(provider, 100), PROB_EXTREMELY_LOW),
      (new HealthReward(provider, 200), PROB_EXTREMELY_LOW),
    ]);

    rewards.ForEach(r => r.Item1.Start());
  }

  public void Start(BasePlugin? plugin) {
    Start();
    foreach (var (reward, _) in rewards) {
      if (reward is IPluginModule module) { module.Start(plugin); }
    }
  }

  public void Dispose() { }
}