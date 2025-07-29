using GitVersion;

namespace TTT.Api;

public interface ITerrorApi : ITerrorModule {
  string ITerrorModule.Name => "Core";
  string ITerrorModule.Version => GitVersionInformation.FullSemVer;
  IServiceProvider Services { get; }
}