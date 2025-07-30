using Microsoft.Extensions.Localization;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleTest(IMsgLocalizer localizer) {
  [Fact]
  public void Locale_Works() {
    var result = localizer[TestMsgs.CONSTANT];

    Assert.Equal("Foobar", result);
  }

  [Fact]
  public void Locale_Replaces_Placeholder() {
    var msg = localizer[TestMsgs.SINGLE_SUBSTITUTION("Test")];

    Assert.Equal("Placeholder: Test", msg);
  }

  [Fact]
  public void Locale_Pluralizes_ByItself() {
    var msg = localizer[TestMsgs.CONSTANT_S];

    Assert.Equal("s", msg);
  }

  [Theory]
  [InlineData("Foo", "Foos")]
  [InlineData("Fos", "Fos")]
  [InlineData("Foo'", "Foo's")]
  [InlineData("Fos'", "Fos'")]
  public void Locale_Pluralizes_WithPlaceholder(string input, string output) {
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
  public void Locale_Pluralizes_WithNumericAmo(string input, string output) {
    var msg = localizer[TestMsgs.TRAILING_PLURALS(input)];

    Assert.Equal(output, msg);
  }

  [Fact]
  public void Locale_PluralApo_AddsS() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_CONSTANT_APO];

    Assert.Equal("Foo's", msg);
  }

  [Fact]
  public void Locale_PluralApo_DoesNotAddS() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_IMPROPER_CONSTANT];

    Assert.Equal("Bars'", msg);
  }

  [Theory]
  [InlineData(-2, "Foo", "-2 Foos")]
  [InlineData(-1, "Foo", "-1 Foos")]
  [InlineData(0, "Foo", "0 Foos")]
  [InlineData(1, "Foo", "1 Foo")]
  [InlineData(2, "Foo", "2 Foos")]
  public void Locale_Pluralizes_WithNumericAmoAndObject(int amo, string word,
    string output) {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_DOUBLE(amo, word)];

    Assert.Equal(output, msg);
  }
}