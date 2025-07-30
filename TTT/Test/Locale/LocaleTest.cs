using Microsoft.Extensions.Localization;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleTest(IStringLocalizer localizer) {
  [Fact]
  public void Locale_BasicTest() {
    var msg = localizer["BASIC_TEST"];
    
    Assert.NotNull(msg);
    Assert.Equal("Foobar", msg.Value);
  }
}