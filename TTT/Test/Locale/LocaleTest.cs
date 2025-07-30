using Microsoft.Extensions.Localization;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleTest(IMsgLocalizer localizer) {
  [Fact]
  public void Locale_BasicTest() {
    var result = localizer[TestMsgs.BASIC_TEST];

    Assert.Equal("Foobar", result);
  }

  [Fact]
  public void Locale_Placeholder_Works() {
    var msg = localizer[TestMsgs.PLACEHOLDER_TEST("Test")];

    Assert.Equal("Placeholder: Test", msg);
  }
}