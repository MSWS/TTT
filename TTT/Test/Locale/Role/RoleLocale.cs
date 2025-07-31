using TTT.Game.Roles;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale.Role;

public class RoleLocale(IMsgLocalizer locale) {
  [Fact]
  public void RoleLocale_Innocent() {
    var role = new InnocentRole(locale);
    Assert.Equal("Innocent", role.Name);
  }

  [Fact]
  public void RoleLocale_Traitor() {
    var role = new TraitorRole(locale);
    Assert.Equal("Traitor", role.Name);
  }

  [Fact]
  public void RoleLocale_Detective() {
    var role = new DetectiveRole(locale);
    Assert.Equal("Detective", role.Name);
  }
}