using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Locale;
using TTT.RTD.Actions;

namespace TTT.RTD;

public abstract class RoundStartReward(IServiceProvider provider)
  : IRtdReward, IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly ISet<IOnlinePlayer> givenPlayers =
    new HashSet<IOnlinePlayer>();

  protected readonly IMsgLocalizer Locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public abstract string Name { get; }
  public abstract string Description { get; }

  public void GrantReward(IOnlinePlayer player) { givenPlayers.Add(player); }

  public void Start() { bus.RegisterListener(this); }

  public void Dispose() { bus.UnregisterListener(this); }
  public abstract void GiveOnRound(IOnlinePlayer player);

  [UsedImplicitly]
  [EventHandler(Priority = Priority.LOW)]
  public virtual void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;

    foreach (var player in givenPlayers) {
      GiveOnRound(player);
      ev.Game.Logger.LogAction(new RolledAction(roles, player, Name));
    }

    givenPlayers.Clear();
  }
}