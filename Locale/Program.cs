using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TTT.Locale;

public static class Program {
  private static readonly JsonSerializerOptions opts =
    new() { WriteIndented = true };

  public static void Main(string[] args) {
    if (args.Length < 1) {
      Console.Error.WriteLine("Usage:");
      Console.Error.WriteLine("  YamlToJson <input.yml>");
      Console.Error.WriteLine(
        "  YamlToJson <input1.yml> <input2.yml> ... --out <output.json>");
      Environment.Exit(1);
    }

    string   outputPath;
    string[] inputPaths;

    if (args.Length == 1) {
      inputPaths = [args[0]];
      outputPath = Path.ChangeExtension(args[0], ".json");
    } else {
      var outIndex = Array.IndexOf(args, "--out");
      if (outIndex == -1 || outIndex == args.Length - 1 || outIndex < 1) {
        Console.Error.WriteLine(
          "Error: When specifying multiple input files, use: --out <output.json>");
        Environment.Exit(1);
      }

      outputPath = args[outIndex + 1];
      inputPaths = args.Take(outIndex).ToArray();
    }

    foreach (var input in inputPaths) {
      if (File.Exists(input)) continue;
      Console.Error.WriteLine($"Error: File not found - {input}");
      Environment.Exit(2);
    }

    var deserializer = new DeserializerBuilder()
     .WithNamingConvention(NullNamingConvention.Instance)
     .Build();

    var merged = new Dictionary<string, string>();

    foreach (var input in inputPaths) {
      var yaml   = File.ReadAllText(input);
      var parsed = deserializer.Deserialize<Dictionary<string, string>>(yaml);
      if (parsed == null) {
        Console.Error.WriteLine($"Error: Failed to parse YAML - {input}");
        continue;
      }

      foreach (var (k, v) in parsed) {
        if (merged.TryGetValue(k, out var existing) && existing != v)
          Console.Error.WriteLine(
            $"Warning: Duplicate key '{k}' found in {input}, overwriting value.");

        merged[k] = v; // Overwrite if duplicate
      }
    }

    var dir = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
      Directory.CreateDirectory(dir);

    var json = JsonSerializer.Serialize(merged, opts);
    File.WriteAllText(outputPath, json);
  }
}