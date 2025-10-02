using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game;
using TTT.Locale;

namespace TTT.CS2;

public class CS2Body(IServiceProvider provider, CRagdollProp ragdoll,
  IPlayer player) : IBody {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public CRagdollProp Ragdoll { get; } = ragdoll;
  public IPlayer OfPlayer { get; } = player;
  public bool IsIdentified { get; set; }
  public IWeapon? MurderWeapon { get; private set; }

  public IPlayer? Killer { get; set; }
  public string Id { get; } = ragdoll.Index.ToString();
  public DateTime TimeOfDeath { get; } = DateTime.Now;

  public CS2Body WithWeapon(IWeapon weapon) {
    MurderWeapon = weapon;
    return this;
  }

  public CS2Body WithKiller(IPlayer? killer) {
    Killer = killer;
    return this;
  }
}