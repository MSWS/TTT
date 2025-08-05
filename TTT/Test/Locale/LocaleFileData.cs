using Microsoft.Extensions.Localization;
using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleFileKVData : TheoryData<string, string> {
  public LocaleFileKVData() {
    foreach (var key in StringLocalizer.Instance.GetAllStrings(true))
      Add(key.Name, key.Value);
  }
}