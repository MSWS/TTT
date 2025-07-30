namespace TTT.API;

public interface ITerrorApi : ITerrorModule {
  IServiceProvider Services { get; }
  string ITerrorModule.Name => "Core";
  string ITerrorModule.Version => GitVersionInformation.FullSemVer;
}