using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game.lang;
using TTT.Locale;

namespace TTT.Game.Roles;

public abstract class BaseRole(IServiceProvider provider) : IRole {
  protected readonly TTTConfig Config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  protected readonly IInventoryManager Inventory =
    provider.GetRequiredService<IInventoryManager>();

  protected readonly IMsgLocalizer? Localizer =
    provider.GetService<IMsgLocalizer>();

  protected readonly IMessenger msg = provider.GetRequiredService<IMessenger>();

  protected readonly IServiceProvider Provider = provider;

  protected readonly IRoleAssigner Roles =
    provider.GetRequiredService<IRoleAssigner>();

  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  public abstract IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players);

  public virtual void OnAssign(IOnlinePlayer player) {
    if (Localizer != null)
      msg.Message(player, Localizer[GameMsgs.ROLE_ASSIGNED(this)]);

    if (!Config.RoleCfg.StripWeaponsPriorToEquipping) return;

    Inventory.RemoveAllWeapons(player);
  }
}