# Versioning

This project allows for manually generating a `GitVersionInformation.g.cs` on platforms where `MSBuild.GitVersion` is
not available (e.g. NixOS). You likely do not need to use this project (GitHub Actions is able to use
`MSBuild.GitVersion` by itself, and Windows similarly functions seamlessly).

## Usage

1. `dotnet build`
2. `dotnet run --project Versioning/Versioning.csproj`