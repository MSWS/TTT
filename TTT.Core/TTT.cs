using CounterStrikeSharp.API.Core;
using GitVersion;

namespace TTT.Core;

public class TTT : BasePlugin {
  public override string ModuleName => "TTT.Core";

  public override string ModuleVersion
    => $"{GitVersionInformation.BranchName}-{GitVersionInformation.FullSemVer}-{GitVersionInformation.BuildMetaDataPadded}";
}