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
    var msg = localizer[TestMsgs.TEST_PREFIX];

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

  [Fact]
  public void Locale_AStatic_Lower() {
    var msg = localizer[TestMsgs.AN_TEST_LOWER];

    Assert.Equal("a test", msg);
  }

  [Fact]
  public void Locale_AStatic_Upper() {
    var msg = localizer[TestMsgs.AN_TEST_UPPER];

    Assert.Equal("A TEST", msg);
  }

  [Fact]
  public void Locale_AnAmazing_Lower() {
    var msg = localizer[TestMsgs.AN_AMAZING_TEST_LOWER];

    Assert.Equal("an amazing test", msg);
  }

  [Fact]
  public void Locale_AnAmazing_Upper() {
    var msg = localizer[TestMsgs.AN_AMAZING_TEST_UPPER];

    Assert.Equal("AN AMAZING TEST", msg);
  }

  [Fact]
  public void Locale_AnAmazing_Sentence() {
    var msg = localizer[TestMsgs.AN_AMAZING_TEST_SENTENCE];

    Assert.Equal("An amazing test", msg);
  }

  [Fact]
  public void Locale_AnAmazing_Odd() {
    var msg = localizer[TestMsgs.AN_AMAZING_TEST_ODD];

    Assert.Equal("aN amazing test", msg);
  }

  [Theory]
  [InlineData("test", "a test")]
  [InlineData("foobar", "a foobar")]
  [InlineData("young person", "a young person")]
  [InlineData("50-foot high tree", "a 50-foot high tree")]
  [InlineData("impressive jump", "an impressive jump")]
  [InlineData("ELEVATED PLATFORM", "an ELEVATED PLATFORM")]
  public void Locale_ASubstituion_Lower(string input, string output) {
    var msg = localizer[TestMsgs.AN_TEST_SUBSTITUTION_LOWER(input)];

    Assert.Equal(output, msg);
  }

  [Theory]
  [InlineData("test", "A test")]
  [InlineData("FOOBAR", "A FOOBAR")]
  [InlineData("YOUNG PERSON", "A YOUNG PERSON")]
  [InlineData("50-FOOT HIGH TREE", "A 50-FOOT HIGH TREE")]
  [InlineData("IMPRESSIVE JUMP", "AN IMPRESSIVE JUMP")]
  [InlineData("ELEVATED PLATFORM", "AN ELEVATED PLATFORM")]
  [InlineData("amazing test", "AN amazing test")]
  public void Locale_ASubstituion_Upper(string input, string output) {
    var msg = localizer[TestMsgs.AN_TEST_SUBSTITUTION_UPPER(input)];

    Assert.Equal(output, msg);
  }

  [Fact]
  public void Locale_AnBetween() {
    var msg = localizer[TestMsgs.AN_IN_BETWEEN];

    Assert.Equal("Foo a bar", msg);
  }

  [Fact]
  public void Locale_AnAtEnd() {
    var msg = localizer[TestMsgs.AN_AT_END];

    Assert.Equal("Foo a", msg);
  }
}