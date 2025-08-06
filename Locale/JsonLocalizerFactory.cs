using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;

namespace TTT.Locale;

public class JsonLocalizerFactory : IStringLocalizerFactory {
  private readonly string langPath;

  public JsonLocalizerFactory() {
    // Lang folder is in the root of the project
    // keep moving up until we find it
    var current = Directory.GetCurrentDirectory();
    while (!File.Exists(Path.Combine(current, "lang", "en.json"))) {
      current = Directory.GetParent(current)?.FullName;
      if (current == null)
        throw new DirectoryNotFoundException("Could not find lang folder");
    }

    langPath = Path.Combine(current, "lang");
  }

  public IStringLocalizer Create(Type resourceSource) {
    return new JsonStringLocalizer(langPath);
  }

  public IStringLocalizer Create(string baseName, string location) {
    return new JsonStringLocalizer(langPath);
  }
}