using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.Extensions;
using TTT.Game;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.CS2;

public class CS2Body(IServiceProvider provider, CRagdollProp ragdoll,
  IPlayer player) : IBody {
  public CRagdollProp Ragdoll { get; } = ragdoll;
  public IPlayer OfPlayer { get; } = player;
  public bool IsIdentified { get; set; }
  public IWeapon? MurderWeapon { get; private set; }

  public IPlayer? Killer { get; private set; }
  public string Id { get; } = ragdoll.Index.ToString();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public CS2Body WithWeapon(IWeapon weapon) {
    MurderWeapon = weapon;
    return this;
  }

  public CS2Body WithWeapon(string weapon) {
    return WithWeapon(new BaseWeapon(weapon));
  }

  public CS2Body WithKiller(IPlayer? killer) {
    Killer = killer;
    return this;
  }
}