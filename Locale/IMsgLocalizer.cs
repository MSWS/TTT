using Microsoft.Extensions.Localization;

namespace TTT.Locale;

public interface IMsgLocalizer : IStringLocalizer {
  string this[IMsg msg] { get; }
}