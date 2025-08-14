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

  [Theory]
  [InlineData("1 traitor%s%", "1 traitor")]
  [InlineData("2 traitor%s%", "2 traitors")]
  [InlineData("1 non-Traitor%s%", "1 non-Traitor")]
  [InlineData("2 non-Traitor%s%", "2 non-Traitors")]
  public void HandlePluralization_WorksWithDashes(string input, string output) {
    var message = StringLocalizer.HandlePluralization(input);

    Assert.Equal(output, message);
  }

  [Fact]
  public void HandlePluralization_WorksWithColors_Single() {
    var message =
      StringLocalizer.HandlePluralization("1 " + new ColoredRole("traitor").Name
        + "%s%");

    Assert.Equal("1 " + new ColoredRole("traitor").Name, message);
  }

  [Fact]
  public void HandlePluralization_WorksWithColors_Plurals() {
    var message =
      StringLocalizer.HandlePluralization("2 " + new ColoredRole("traitor").Name
        + "%s%");

    Assert.Equal("2 " + new ColoredRole("traitor").Name + "s", message);
  }

  [Fact]
  public void HandlePluralization_WorksWithColors_Dashed_Single() {
    var message = StringLocalizer.HandlePluralization(
      $"1 {ChatColors.Green}non-{new ColoredRole("traitor").Name}%s%");

    Assert.Equal($"1 {ChatColors.Green}non-{new ColoredRole("traitor").Name}",
      message);
  }

  [Fact]
  public void HandlePluralization_WorksWithColors_Dashed() {
    var message = StringLocalizer.HandlePluralization(
      $"2 {ChatColors.Green}non-{new ColoredRole("traitor").Name}%s%");

    Assert.Equal($"2 {ChatColors.Green}non-{new ColoredRole("traitor").Name}s",
      message);
  }

  public void HandlePluralization_WorksWithColors(string input, string output) {
    var message = StringLocalizer.HandlePluralization(input);

    Assert.Equal(output, message);
  }

  public class ColoredRole(string name) : IRole {
    public string Id => "test.role.colored";
    public string Name => ChatColors.DarkBlue + name;
    public Color Color => Color.Red;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return players.FirstOrDefault();
    }
  }
}