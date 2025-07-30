# TTT

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/MSWS/6289e1f34da3b70fbba5f84f5ffb25a1/raw/code-coverage.json)

TTT (Trouble in Terrorist Town) is a game mode similar to Among Us where a group of players are attempting to
survive while eliminating the traitors among them.

# Structure

## TTT

You likely want to read the [TTT ReadMe](./TTT/README.md), which covers the structure of its own directory.

## Versioning

To allow for `MSBuild.GitVersion` to be used on both Windows and Linux (specifically NixOS), this project manually
converts `dotnet-gitversion` to the `GitVersionInformation` that is used.

# Development

1. `git clone ...`
2. `dotnet restore`
3. `dotnet build`
4. `dotnet test` (Optional)
