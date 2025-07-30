using Microsoft.Extensions.Localization;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleTest(IMsgLocalizer localizer) {
  [Fact]
  public void Locale_BasicTest() {
    var result = localizer[TestMsgs.CONSTANT];

    Assert.Equal("Foobar", result);
  }

  [Fact]
  public void Locale_Placeholder_Works() {
    var msg = localizer[TestMsgs.SINGLE_SUBSTITUTION("Test")];

    Assert.Equal("Placeholder: Test", msg);
  }

  [Fact]
  public void Locale_Plural_HasS() {
    var msg = localizer[TestMsgs.CONSTANT_S];

    Assert.Equal("s", msg);
  }

  [Theory]
  [InlineData("Foo", "Foos")]
  [InlineData("Fos", "Fos")]
  [InlineData("Foo'", "Foo's")]
  [InlineData("Fos'", "Fos'")]
  public void Locale_Plural_AddsBasic(string input, string output) {
    var msg = localizer[TestMsgs.TRAILING_PLURALS(input)];

    Assert.Equal(output, msg);
  }

  [Theory]
  [InlineData("-2 Foo", "-2 Foos")]
  [InlineData("-1 Foo", "-1 Foos")]
  [InlineData("0 Foo", "0 Foos")]
  [InlineData("1 Foo", "1 Foo")]
  [InlineData("1.0 Foo", "1.0 Foos")]
  [InlineData("2 Foo", "2 Foos")]
  public void Locale_Plural_HandlesAmount(string input, string output) {
    var msg = localizer[TestMsgs.TRAILING_PLURALS(input)];

    Assert.Equal(output, msg);
  }

  [Fact]
  public void Locale_PluralApo_AddsS() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_CONSTANT_APO];

    Assert.Equal("Foo's", msg);
  }
}