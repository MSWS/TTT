using System.Text.RegularExpressions;
using Xunit;

namespace TTT.Test.Locale;

public partial class FormatTests {
  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Parity_Brackets(string _, string val) {
    // For each opening bracket, make sure there is a closing bracket
    var brackets = 0;
    foreach (var c in val) {
      if (c == '{') brackets++;
      if (c == '}') brackets--;
    }

    Assert.Equal(0, brackets);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Parity_Percents(string _, string val) {
    // For each opening bracket, make sure there is a closing bracket
    var percs = val.Count(c => c == '%');
    Assert.Equal(0, percs % 2);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void No_Empty_Values(string _, string val) {
    Assert.False(string.IsNullOrEmpty(val));
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Keys_Use_Subchars(string key, string _) {
    Assert.Matches(@"^[A-Z_\.]+$", key);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Ends_In_Punctuation(string key, string val) {
    if (!key.Contains(' ')) return;
    Assert.Matches(@"[.,!? ]$", val);
  }

  [GeneratedRegex("{(.*?)}")]
  private static partial Regex ColorBrackets();
}