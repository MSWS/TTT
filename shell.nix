{ pkgs ? import <nixpkgs> {} }:

let
  dotnet = pkgs.dotnetCorePackages.sdk_8_0;
in
pkgs.mkShell {
  buildInputs = [ dotnet ];

  shellHook = ''
    export DOTNET_ROOT="${dotnet}/share/dotnet"
    export PATH="${dotnet}/bin:$PATH"
  '';
}

