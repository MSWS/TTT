using CounterStrikeSharp.API.Core;

namespace TTT.API;

public interface IPluginModule : ITerrorModule {
  void Start(BasePlugin? plugin) { Start(); }

  void Start(BasePlugin? plugin, bool hotReload) { Start(plugin); }
}