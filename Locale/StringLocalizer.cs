using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace TTT.Locale;

/// <summary>
///   A custom implementation of <see cref="IStringLocalizer" /> that adds support
///   for in-string placeholders like %key% and grammatical pluralization with %s%.
/// </summary>
public partial class StringLocalizer : IMsgLocalizer {
  private readonly IStringLocalizer localizer;

  public StringLocalizer(IStringLocalizerFactory factory) {
    var type = typeof(StringLocalizer);
    var assemblyName =
      new AssemblyName(type.GetTypeInfo().Assembly.FullName ?? string.Empty);
    localizer = factory.Create(string.Empty, assemblyName.FullName);
  }

  public string this[IMsg msg] => getString(msg.Key, msg.Args);

  public LocalizedString this[string name] => getString(name);

  public LocalizedString this[string name, params object[] arguments]
    => getString(name, arguments);

  public IEnumerable<LocalizedString>
    GetAllStrings(bool includeParentCultures) {
    return localizer.GetAllStrings(includeParentCultures)
     .Select(str => getString(str.Name));
  }

  [GeneratedRegex("%.*?%")]
  private static partial Regex percentRegex();

  [GeneratedRegex(@"\b(\w+)%s%")]
  private static partial Regex pluralRegex();

  [GeneratedRegex(@"%an%", RegexOptions.IgnoreCase)]
  private static partial Regex anRegex();

  private LocalizedString getString(string name, params object[] arguments) {
    // Get the localized value
    string value;
    try { value = localizer[name].Value; } catch (NullReferenceException) {
      return new LocalizedString(name, name, true);
    }

    // Replace placeholders like %key% with their respective values
    var matches = percentRegex().Matches(value);
    foreach (Match match in matches) {
      var key        = match.Value;
      var trimmedKey = key[1..^1]; // Trim % symbols

      // NullReferenceException catch block if key does not exist
      try {
        // CS# forces a space before a chat color if the entirety
        // of the strong is a color code. This is undesired
        // in our case, so we trim the value when we have a prefix.
        var replacement = localizer[trimmedKey].Value;
        if (replacement == trimmedKey) continue;
        value = value.Replace(key,
          trimmedKey.Contains("PREFIX", StringComparison.OrdinalIgnoreCase) ?
            replacement :
            replacement.TrimStart());
      } catch (NullReferenceException) {
        // Key doesn't exist, move on
      }
    }

    // Format with arguments if provided
    if (arguments.Length > 0) value = string.Format(value, arguments);

    // Handle pluralization
    value = HandlePluralization(value);
    value = HandleAn(value);

    if (!string.IsNullOrWhiteSpace(value) && hasChatColor(value)) {
      var first = value.First(c => !char.IsWhiteSpace(c));
      if (char.IsAsciiLetterOrDigit(first)) value = value.TrimStart();
    }

    return new LocalizedString(name, value);
  }

  private static bool isChatColor(char c) {
    return c is '\x01' or '\x02' or '\x03' or '\x04' or '\x05' or '\x06'
      or '\x07' or '\x08' or '\x09' or '\x0A' or '\x0B' or '\x0C' or '\x0D'
      or '\x0E' or '\x0F' or '\x10';
  }

  private static bool hasChatColor(string value) {
    return value.Any(isChatColor);
  }

  public static string HandlePluralization(string value) {
    var pluralMatches = pluralRegex().Matches(value);
    foreach (Match match in pluralMatches) {
      var word   = match.Groups[1].Value.ToLower();
      var index  = match.Index;
      var prefix = value[..index].Trim();

      var lastWords = prefix.Split(' ')
       .Select(w
          => w.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

      var previousNumber = lastWords.LastOrDefault(w => int.TryParse(w, out _));

      if (previousNumber != null)
        value = value[..index] + value[index..]
         .Replace("%s%", int.Parse(previousNumber) == 1 ? "" : "s");
      else
        value = value[..index] + value[index..]
         .Replace("%s%", word.EndsWith('s') ? "" : "s");
    }

    value = value.Replace("%s%", "s");

    // We have to do this chicanery due to support colors in the string
    value = handleTrailingS(value);

    return value;
  }

  private static string handleTrailingS(string value) {
    var trailingIndex = -1;
    while ((trailingIndex =
      value.IndexOf("'s", trailingIndex + 1, StringComparison.Ordinal)) != -1) {
      var startingWordBoundary = value[..trailingIndex].LastIndexOf(' ');
      if (startingWordBoundary == -1
        || startingWordBoundary + 2 > value.Length) {
        if (value.EndsWith("s's")) value = value[..^1];
        break;
      }

      var endingWordBoundary = value.IndexOf(' ', trailingIndex + 2);
      var word = value[(startingWordBoundary + 1)..endingWordBoundary];
      var filteredWord = word.Where(c => char.IsLetterOrDigit(c) || c == '\'')
       .ToArray();
      if (new string(filteredWord).EndsWith("s's",
        StringComparison.OrdinalIgnoreCase))
        value = value[..(trailingIndex + 1)] + " "
          + value[(trailingIndex + 4)..];
    }

    return value;
  }

  public static string HandleAn(string value) {
    var anMatches = anRegex().Matches(value);
    foreach (Match match in anMatches) {
      var anMatch = match.Value[1..^1];
      var index   = match.Index;
      var prefix  = value[..index];
      var suffix  = value[(index + match.Length)..];

      // Determine if the next word starts with a vowel sound
      var nextChar = char.ToLower(suffix.FirstOrDefault(char.IsLetterOrDigit));
      value = nextChar switch {
        'a' or 'e' or 'i' or 'o' or 'u' => prefix + anMatch + suffix,
        _                               => prefix + anMatch[0] + suffix
      };
    }

    return value;
  }
}