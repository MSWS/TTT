using TTT.Locale;
using Xunit;

namespace TTT.Test.Locale;

public class LocaleFileKVData : TheoryData<string, string> {
  public LocaleFileKVData() {
    var localizer = new StringLocalizer(new JsonLocalizerFactory());
    foreach (var key in localizer.GetAllStrings(true)) Add(key.Name, key.Value);
  }
}