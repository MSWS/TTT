using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Locale;

namespace TTT.Game.Roles;

public abstract class BaseRole(IServiceProvider provider) : IRole {
  protected readonly GameConfig Config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult();

  protected readonly IInventoryManager Inventory =
    provider.GetRequiredService<IInventoryManager>();

  protected readonly IMsgLocalizer? Localizer =
    provider.GetService<IMsgLocalizer>();

  protected readonly IServiceProvider Provider = provider;
  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  public abstract IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players);

  public virtual void OnAssign(IOnlinePlayer player) { }
}