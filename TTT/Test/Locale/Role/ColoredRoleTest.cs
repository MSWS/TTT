using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale.Role;

public class ColoredRoleTest(IServiceProvider provider) {
  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  [Fact]
  public void RoleAssignMessage_ShouldHaveA_OnColoredName() {
    var msg = locale[GameMsgs.ROLE_ASSIGNED(new ColoredRole("foo"))];

    Assert.Contains(" are a ", msg);
  }

  [Fact]
  public void RoleAssignMessage_ShouldHaveAn_OnVowelName() {
    var msg = locale[GameMsgs.ROLE_ASSIGNED(new ColoredRole("inno"))];

    Assert.Contains(" are an ", msg);
  }

  [Theory]
  [InlineData("detective")]
  [InlineData("traitor")]
  [InlineData("Detective")]
  [InlineData("Traitor")]
  public void RoleAssignMessage_ShouldHaveA_OnConsonantName(string name) {
    var msg = locale[GameMsgs.ROLE_ASSIGNED(new ColoredRole(name))];

    Assert.Contains(" are a ", msg);
  }


  public class ColoredRole(string name) : IRole {
    public string Id => "test.role.colored";
    public string Name => ChatColors.Red + name;
    public Color Color => Color.Red;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return players.FirstOrDefault();
    }
  }
}