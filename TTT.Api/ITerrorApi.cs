using GitVersion;

namespace TTT.Api;

public interface ITerrorApi : ITerrorModule {
  IServiceProvider Services { get; }
  string ITerrorModule.Name => "Core";
  string ITerrorModule.Version => GitVersionInformation.FullSemVer;
}