using CounterStrikeSharp.API.Core.Capabilities;
using MAULActainShared.plugin;

namespace TTT.CS2.ThirdParties.eGO;

public class EgoApi {
  public static PluginCapability<IActain> MAUL { get; } =
    new("maulactain:core");
}