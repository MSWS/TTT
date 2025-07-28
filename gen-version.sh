#!/usr/bin/env bash
set -e

# Generate GitVersion JSON
dotnet-gitversion . > gitversion.json

# Generate C# source file
dotnet run --project tools/GenerateGitVersion "$(pwd)"