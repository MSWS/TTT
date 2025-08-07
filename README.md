# TTT | [![ReadMe](https://img.shields.io/badge/ReadMe-018EF5?logo=readme&logoColor=fff&style=for-the-badge)](./TTT/README.md)

![Code Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/MSWS/6289e1f34da3b70fbba5f84f5ffb25a1/raw/code-coverage.json)
![Discord](https://img.shields.io/discord/623439460683481091?style=for-the-badge&logo=discord&label=Discord)
![Endpoint Badge](https://img.shields.io/endpoint?url=https%3A%2F%2Fwaka.msws.xyz%2Fapi%2Fcompat%2Fshields%2Fv1%2Fmsws%2Fproject%3ATTT%2Finterval%3Aall_time%26label%3DAll%2520time%26color%3Dblue&style=for-the-badge&label=Dev%20Time)

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

## Versioning

This project adheres to [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html).
The versioning scheme consists of three components:

- **MAJOR** version indicates incompatible API changes,
- **MINOR** version signifies the addition of functionality in a backwards-compatible manner, and
- **PATCH** version reflects backwards-compatible bug fixes.

# Modules

### [TTT](./TTT)

You likely want to read the [TTT README](./TTT/README.md), which covers the structure of its own directory.

### [Versioning](./Versioning)

To allow for `MSBuild.GitVersion` to be used on both Windows and Linux (specifically NixOS), this project manually
converts `dotnet-gitversion` to the `GitVersionInformation` that is used.

### [Locale](./Locale)

Due to this project being primarily developed with Counter-Strike 2 (and more
specifically, [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)) in mind, localization has been
built with flat-file storage based around YML/JSON.

In short, we write our locales in `en.yml`, run `Locale.csproj` to convert and combine all `**/Lang/en.yml` -> a master
`lang/en.json`, and then run our tests / release pipeliens with it.

It is recommend to read the [Locale README](./Locale/README.md) for more information on how to use it.

## Development

1. `git clone ...`
2. `dotnet restore`
3. `dotnet build`
4. Convert all `lang/en.yml` -> `lang/en.json` (Required for testing, refer to [Locale](./Locale/README.md))
5. `dotnet test` (Optional)
