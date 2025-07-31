# TTT - Localizer

Responsible for managing the translations of the different TTT modules.
In and of itself does not provide any translations, but rather provides a framework for other modules to provide their
own translations.

For convenience, Localizer runs as a CLI tool that converts YML files to JSON files, as YML is nicer to work with, but
JSON is the format used for [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)'s localization system.

## Usage

1. Build (`dotnet build`)
2. Navigate to where the Localizer.dll is
3. Run `dotnet Localizer.dll [input files...] -o [output file]`

- CS# sources all translations from the single `.json` file; separating the YML files allows for easier management per
  module, the localizer will combine them into a single JSON file.
- If you are only providing one input file, you can omit the `-o` flag and it will output to `inputFileName.json` by
  default.