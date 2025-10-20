using CounterStrikeSharp.API;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Game;

namespace TTT.RTD;

public abstract class RoundStartReward(IServiceProvider provider)
  : IRtdReward, IListener {
  private readonly ISet<IOnlinePlayer> givenPlayers =
    new HashSet<IOnlinePlayer>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public abstract string Name { get; }
  public abstract string Description { get; }

  public void GrantReward(IOnlinePlayer player) { givenPlayers.Add(player); }
  public abstract void GiveOnRound(IOnlinePlayer player);

  [UsedImplicitly]
  [EventHandler(Priority = Priority.LOW)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;

    foreach (var player in givenPlayers) GiveOnRound(player);

    givenPlayers.Clear();
  }

  public void Start() { bus.RegisterListener(this); }

  public void Dispose() { bus.UnregisterListener(this); }
}