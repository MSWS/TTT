# TTT

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/MSWS/6289e1f34da3b70fbba5f84f5ffb25a1/raw/code-coverage.json)

TTT (Trouble in Terrorist Town) is a game mode similar to Among Us where a group of players are attempting to
survive while eliminating the traitors among them.

## Features

- [X] Unit Testing
- [ ] Basic Gameplay
    - [ ] Traitors
    - [ ] Detectives
    - [ ] Innocents
- [ ] Shop
- [ ] Karma
- [ ] Statistics

# Modules

## [TTT](./TTT)

You likely want to read the [TTT README](./TTT/README.md), which covers the structure of its own directory.

## [Versioning](./Versioning)

To allow for `MSBuild.GitVersion` to be used on both Windows and Linux (specifically NixOS), this project manually
converts `dotnet-gitversion` to the `GitVersionInformation` that is used.

## [Locale](./Locale)

Due to this project being primarily developed with Counter-Strike 2 (and more
specifically, [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)) in mind, localization has been
built with flat-file storage based around YML/JSON.

In short, we write our locales in `en.yml`, run `Locale.csproj` to convert and combine all `**/Lang/en.yml` -> a master
`lang/en.json`, and then run our tests / release pipeliens with it.

It is recommend to read the [Locale README](./Locale/README.md) for more information on how to use it.

# Development

1. `git clone ...`
2. `dotnet restore`
3. `dotnet build`
4. Convert all `lang/en.yml` -> `lang/en.json` (Required for testing, refer to [Locale](./Locale/Locale.csproj))
5. `dotnet test` (Optional)
