using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using TTT.Locale;

namespace TTT.Test.Fakes;

public partial class FakeLocalizer : IMsgLocalizer {
  public LocalizedString this[string name] => new(name, name);
  public LocalizedString this[string name, params object[] arguments] =>
    new(name, string.Format(name, arguments));

  public string this[IMsg msg] =>
    msg.Args.Length == 0
      ? msg.Key
      : string.Format(msg.Key, msg.Args);

  public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
    Enumerable.Empty<LocalizedString>();
}