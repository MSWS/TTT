# TTT

This directory is primarily an intermediary to allow [`Directory.Build.props`](./Directory.Build.props) to only apply to
the subsidiary projects (i.e. not [Versioning](../Versioning)).

# Structure
## [API](./API)
The public API for TTT. Include this to add-on extra features, modules, roles, etc.

## [CS2](./CS2)
A linker for CS2 to TTT. This adds support for CS2 to TTT.

## [Game](./Game)
The core game logic for TTT. This is the main project that runs the game.
The goal of separating this from CS2 is to allow for other games to be supported in the future.
Theoretically, `Game` should not care about the underlying game engine, but rather just the game logic.
In other words, you would be able to run TTT within a CLI, a web browser, or any other platform that supports C#.

## [Plugin](./Plugin)
The bootstrapper for TTT, which registers the CS2 specific services.

## [Test](_./Test)
Testing for TTT.