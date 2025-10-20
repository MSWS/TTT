using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.Game.Roles;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Items.SilentAWP;

public static class SilentAWPServiceCollection {
  public static void AddSilentAWPServices(this IServiceCollection services) {
    services.AddModBehavior<SilentAWPItem>();
  }
}

public class SilentAWPItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider), IPluginModule {
  private SilentAWPConfig config
    => Provider.GetService<IStorage<SilentAWPConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SilentAWPConfig();

  private readonly IPlayerConverter<CCSPlayerController> playerConverter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IDictionary<string, int> silentShots =
    new Dictionary<string, int>();

  public override string Name => Locale[SilentAWPMsgs.SHOP_ITEM_SILENT_AWP];

  public override string Description
    => Locale[SilentAWPMsgs.SHOP_ITEM_SILENT_AWP_DESC];

  public override ShopItemConfig Config => config;

  public void Start(BasePlugin? plugin) {
    base.Start();
    plugin?.HookUserMessage(452, onWeaponSound);
  }

  public override void OnPurchase(IOnlinePlayer player) {
    silentShots[player.Id] = config.CurrentAmmo ?? 0 + config.ReserveAmmo ?? 0;
    Task.Run(async () => {
      await Inventory.RemoveWeaponInSlot(player, 0);
      await Inventory.GiveWeapon(player, config);
    });
  }

  private HookResult onWeaponSound(UserMessage msg) {
    var defIndex = msg.ReadUInt("item_def_index");

    if (config.WeaponIndex != defIndex) return HookResult.Continue;
    var splits = msg.DebugString.Split("\n");
    if (splits.Length < 5) return HookResult.Continue;
    var angleLines = msg.DebugString.Split("\n")[1..4]
     .Select(s => s.Trim())
     .ToList();
    if (!angleLines[0].Contains('x') || !angleLines[1].Contains('y')
      || !angleLines[2].Contains('z'))
      return HookResult.Continue;
    var x      = float.Parse(angleLines[0].Split(' ')[1]);
    var y      = float.Parse(angleLines[1].Split(' ')[1]);
    var z      = float.Parse(angleLines[2].Split(' ')[1]);
    var vec    = new Vector(x, y, z);
    var player = findPlayerByCoord(vec);

    if (player == null) return HookResult.Continue;
    if (playerConverter.GetPlayer(player) is not IOnlinePlayer apiPlayer)
      return HookResult.Continue;

    if (!silentShots.TryGetValue(apiPlayer.Id, out var shots) || shots <= 0)
      return HookResult.Continue;

    silentShots[apiPlayer.Id] = shots - 1;
    if (silentShots[apiPlayer.Id] == 0) {
      silentShots.Remove(apiPlayer.Id);
      Shop.RemoveItem<SilentAWPItem>(apiPlayer);
    }

    msg.Recipients.Clear();
    return HookResult.Handled;
  }

  private CCSPlayerController? findPlayerByCoord(Vector vec) {
    foreach (var pl in Utilities.GetPlayers()) {
      var origin = pl.GetEyePosition();
      if (origin == null) continue;
      var dist = vec.DistanceSquared(origin);
      if (dist < 1) return pl;
    }

    return null;
  }
}