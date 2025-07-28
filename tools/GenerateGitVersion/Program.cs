using System.Text.Json;

var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "gitversion.json");
var outputPath = Path.Combine(Directory.GetCurrentDirectory(),
  "GitVersionInformation.g.cs");

var json = JsonDocument.Parse(File.ReadAllText(jsonPath)).RootElement;

using var writer = new StreamWriter(outputPath);

writer.WriteLine("namespace GitVersion");
writer.WriteLine("{");
writer.WriteLine("  internal static class GitVersionInformation");
writer.WriteLine("  {");

foreach (var property in json.EnumerateObject()) {
  var name  = property.Name;
  var value = property.Value;

  string line;

  switch (value.ValueKind) {
    case JsonValueKind.String:
      line =
        $"    public const string {name} = \"{value.GetString()?.Replace("\"", "\\\"")}\";";
      break;
    case JsonValueKind.Number:
      if (value.TryGetInt32(out var intVal))
        line = $"    public const int {name} = {intVal};";
      else if (value.TryGetInt64(out var longVal))
        line = $"    public const long {name} = {longVal}L;";
      else if (value.TryGetDouble(out var dblVal))
        line = $"    public const double {name} = {dblVal};";
      else
        continue; // Skip if unknown number type
      break;
    case JsonValueKind.True:
    case JsonValueKind.False:
      line =
        $"    public const bool {name} = {value.GetBoolean().ToString().ToLowerInvariant()};";
      break;
    case JsonValueKind.Null:
      // No const nulls in C#, so use string.Empty or 0 as appropriate — or skip
      line = $"    // {name} is null and omitted";
      break;
    default:
      // Skip unexpected structures like arrays/objects
      line = $"    // {name} has unsupported type and is omitted";
      break;
  }

  writer.WriteLine(line);
}

writer.WriteLine("  }");
writer.WriteLine("}");