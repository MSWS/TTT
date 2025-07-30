using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TTT.Locale;

public static class Program {
  private static readonly JsonSerializerOptions opts =
    new() { WriteIndented = true };

  public static void Main(string[] args) {
    if (args.Length is < 1 or > 2) {
      Console.Error.WriteLine("Usage: YamlToJson <input.yml> [output.json]");
      Environment.Exit(1);
    }

    var inputPath = args[0];
    var outputPath = args.Length == 2 ?
      args[1] :
      Path.ChangeExtension(inputPath, ".json");

    if (!File.Exists(inputPath)) {
      Console.Error.WriteLine($"Error: File not found - {inputPath}");
      Environment.Exit(2);
    }

    var yaml = File.ReadAllText(inputPath);

    var deserializer = new DeserializerBuilder()
     .WithNamingConvention(NullNamingConvention.Instance)
     .Build();

    var data = deserializer.Deserialize<Dictionary<string, string>>(yaml);

    var json = JsonSerializer.Serialize(data, options: opts);

    File.WriteAllText(outputPath, json);
  }
}