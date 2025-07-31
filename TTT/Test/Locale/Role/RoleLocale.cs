using TTT.Game.Roles;
using Xunit;

namespace TTT.Test.Locale.Role;

public class RoleLocale(IServiceProvider provider) {
  [Fact]
  public void RoleLocale_Innocent() {
    var role = new InnocentRole(provider);
    Assert.Equal("Innocent", role.Name);
  }

  [Fact]
  public void RoleLocale_Traitor() {
    var role = new TraitorRole(provider);
    Assert.Equal("Traitor", role.Name);
  }

  [Fact]
  public void RoleLocale_Detective() {
    var role = new DetectiveRole(provider);
    Assert.Equal("Detective", role.Name);
  }
}