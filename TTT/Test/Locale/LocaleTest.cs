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

  [Theory]
  [InlineData(1.0f, "1")]
  [InlineData(1.5f, "1.5")]
  [InlineData(1.52f, "1.52")]
  [InlineData(2.0f, "2")]
  [InlineData(0.9999, "1")]
  public void Locale_Formats_Float(float f, string output) {
    var msg = localizer[TestMsgs.FLOAT_FORMATTING(f)];

    Assert.Equal(output, msg);
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

  [Theory]
  [InlineData("Foo", "Foo's")]
  [InlineData("Fos", "Fos'")]
  public void
    Locale_Pluralizes_WithPlaceHolder_Apo(string input, string output) {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_APO(input)];

    Assert.Equal(output, msg);
  }

  [Fact]
  public void Locale_Pluralizes_WithConstant() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_CONSTANT];

    Assert.Equal("Foos", msg);
  }

  [Fact]
  public void Locale_Pluralizes_WithConstant_Apo() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_CONSTANT_APO];

    Assert.Equal("Foo's", msg);
  }

  [Fact]
  public void Locale_Propers_Constant() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_IMPROPER_CONSTANT];

    Assert.Equal("Bars", msg);
  }

  [Fact]
  public void Locale_Propers_Constant_Apo() {
    var msg = localizer[TestMsgs.TRAILING_PLURALS_IMPROPER_CONSTANT_APO];

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

  [Fact]
  public void Locale_Replaces_Prefix() {
    var msg = localizer[TestMsgs.PREFIX];

    Assert.Equal("SUPERLIMINAL ", msg);
  }

  [Fact]
  public void Locale_Prefix_RetainsWhitespace() {
    var msg = localizer[TestMsgs.PREFIX_TEST];

    Assert.Equal("SUPERLIMINAL Foo", msg);
  }

  [Fact]
  public void Locale_Trailing_RetainsWhitespace() {
    var msg = localizer[TestMsgs.WHITESPACE_SUFFIX];

    Assert.Equal("Trailing  ", msg);
  }

  [Fact]
  public void Locale_Leading_RetainsWhitespace() {
    var msg = localizer[TestMsgs.WHITESPACE_PREFIX];

    Assert.Equal("  Leading", msg);
  }

  [Fact]
  public void Locale_PrefixAndTrailing_RetainsWhitespace() {
    var msg = localizer[TestMsgs.PREFIX_TEST_TRAILING];

    Assert.Equal("SUPERLIMINAL Foo ", msg);
  }

  [Fact]
  public void Locale_SuffixAndTrailing_RetainsWhitespace() {
    var msg = localizer[TestMsgs.SUFFIX_TEST_TRAILING];

    Assert.Equal(" Foo SUPERLIMINAL ", msg);
  }

  [Fact]
  public void Locale_BothAndTrailing_RetainsWhitespace() {
    var msg = localizer[TestMsgs.BOTH_TEST_TRAILING];

    Assert.Equal(" Foo SUPERLIMINAL  ", msg);
  }
}