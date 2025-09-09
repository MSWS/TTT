using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;
using TTT.Karma;
using Xunit;

namespace TTT.Test.Karma;

public class KarmaListenerTests {
  private readonly IGameManager games;
  private readonly IRoleAssigner roles;
  private readonly IEventBus bus;
  private readonly IKarmaService karma;
  private readonly IPlayerFinder players;

  public enum RoleEnum {
    Innocent, Traitor, Detective
  }

  private readonly IList<IRole> roleSet;

  public KarmaListenerTests(IServiceProvider provider) {
    games   = provider.GetRequiredService<IGameManager>();
    roles   = provider.GetRequiredService<IRoleAssigner>();
    bus     = provider.GetRequiredService<IEventBus>();
    karma   = provider.GetRequiredService<IKarmaService>();
    players = provider.GetRequiredService<IPlayerFinder>();
    roleSet = new List<IRole> {
      new InnocentRole(provider),
      new TraitorRole(provider),
      new DetectiveRole(provider)
    };

    bus.RegisterListener(new KarmaListener(provider));
  }

  [Fact]
  public async Task OnKill_WithoutGame_DoesNothing() {
    var victim   = TestPlayer.Random();
    var attacker = TestPlayer.Random();

    roles.AssignRoles(new HashSet<IOnlinePlayer>([victim, attacker]), roleSet);

    var deathEvent = new PlayerDeathEvent(victim);
    deathEvent.WithKiller(attacker);

    bus.Dispatch(deathEvent);

    var victimKarma   = await karma.Load(victim);
    var attackerKarma = await karma.Load(attacker);

    Assert.Equal(50, victimKarma);
    Assert.Equal(50, attackerKarma);
  }

  [Theory]
  [InlineData(RoleEnum.Innocent, RoleEnum.Innocent, 46, 49)]
  [InlineData(RoleEnum.Innocent, RoleEnum.Traitor, 52, 50)]
  [InlineData(RoleEnum.Innocent, RoleEnum.Detective, 44, 50)]
  [InlineData(RoleEnum.Traitor, RoleEnum.Innocent, 50, 50)]
  [InlineData(RoleEnum.Traitor, RoleEnum.Traitor, 45, 50)]
  [InlineData(RoleEnum.Traitor, RoleEnum.Detective, 51, 50)]
  [InlineData(RoleEnum.Detective, RoleEnum.Innocent, 46, 49)]
  [InlineData(RoleEnum.Detective, RoleEnum.Traitor, 52, 50)]
  [InlineData(RoleEnum.Detective, RoleEnum.Detective, 44, 50)]
  public async Task OnKill_AffectsKarma(RoleEnum attackerRole,
    RoleEnum victimRole, int expAttackerKarma, int expVictimKarma) {
    var victim   = TestPlayer.Random();
    var attacker = TestPlayer.Random();

    players.AddPlayer(victim);
    players.AddPlayer(attacker);

    var game = games.CreateGame();
    Assert.NotNull(game);
    game.Start();

    await roles.Write(victim, (List<IRole>) [roleSet[(int)victimRole]]);
    await roles.Write(attacker, (List<IRole>) [roleSet[(int)attackerRole]]);
    game.Players.Add(victim);
    game.Players.Add(attacker);

    var deathEvent = new PlayerDeathEvent(victim);
    deathEvent.WithKiller(attacker);

    await bus.Dispatch(deathEvent);

    var victimKarma   = await karma.Load(victim);
    var attackerKarma = await karma.Load(attacker);

    Assert.Equal(expVictimKarma, victimKarma);
    Assert.Equal(expAttackerKarma, attackerKarma);
  }
}