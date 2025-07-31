using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TTT.Locale;

public static class Program {
  private static readonly JsonSerializerOptions opts =
    new() { WriteIndented = true };

  public static void Main(string[] args) {
    if (args.Length < 1) {
      PrintUsage();
      Environment.Exit(1);
    }

    var (inputPaths, outputPath) = ParseArguments(args);

    ValidateInputFiles(inputPaths);

    var merged = MergeYamlFiles(inputPaths);

    WriteJsonOutput(merged, outputPath);
  }

  private static void PrintUsage() {
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  YamlToJson <input.yml>");
    Console.Error.WriteLine(
      "  YamlToJson <input1.yml> <input2.yml> ... --out <output.json>");
  }

  private static (string[] inputPaths, string outputPath) ParseArguments(
    string[] args) {
    string   outputPath;
    string[] inputPaths;

    if (args.Length == 1) {
      inputPaths = new[] { args[0] };
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

    return (inputPaths, outputPath);
  }

  private static void ValidateInputFiles(string[] inputPaths) {
    foreach (var input in inputPaths)
      if (!File.Exists(input)) {
        Console.Error.WriteLine($"Error: File not found - {input}");
        Environment.Exit(2);
      }
  }

  private static Dictionary<string, string>
    MergeYamlFiles(string[] inputPaths) {
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

    return merged;
  }

  private static void WriteJsonOutput(Dictionary<string, string> merged,
    string outputPath) {
    var dir = Path.GetDirectoryName(outputPath);
    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
      Directory.CreateDirectory(dir);

    var json = JsonSerializer.Serialize(merged, opts);
    File.WriteAllText(outputPath, json);
  }
}