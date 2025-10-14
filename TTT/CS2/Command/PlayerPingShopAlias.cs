using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command;

public class PlayerPingShopAlias(IServiceProvider provider) : IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IItemSorter itemSorter =
    provider.GetRequiredService<IItemSorter>();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.AddCommandListener("player_ping", onPlayerPing, HookMode.Post);

    for (var i = 0; i < 10; i++) {
      var index = i; // Capture variable
      plugin?.AddCommand($"css_{index}", "",
        (player, _) => { onButton(player, index); });
    }
  }

  private HookResult onPlayerPing(CCSPlayerController? player,
    CommandInfo commandInfo) {
    if (player == null) return HookResult.Continue;
    var gamePlayer = converter.GetPlayer(player) as IOnlinePlayer;
    var cmdInfo =
      new CS2CommandInfo(provider, gamePlayer, 0, "css_shop", "list");
    cmdInfo.CallingContext = CommandCallingContext.Chat;
    provider.GetRequiredService<ICommandManager>().ProcessCommand(cmdInfo);
    return HookResult.Continue;
  }

  private void onButton(CCSPlayerController? player, int index) {
    if (player == null) return;
    if (converter.GetPlayer(player) is not IOnlinePlayer gamePlayer) return;

    var lastUpdated = itemSorter.GetLastUpdate(gamePlayer);
    if (lastUpdated == null
      || DateTime.Now - lastUpdated > TimeSpan.FromSeconds(20))
      return;
    var cmdInfo = new CS2CommandInfo(provider, gamePlayer, 0, "css_shop", "buy",
      (index - 1).ToString());
    cmdInfo.CallingContext = CommandCallingContext.Chat;
    provider.GetRequiredService<ICommandManager>().ProcessCommand(cmdInfo);
  }
}