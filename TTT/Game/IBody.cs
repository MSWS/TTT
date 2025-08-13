using TTT.API;
using TTT.API.Player;

namespace TTT.Game;

public interface IBody {
  IPlayer OfPlayer { get; }
  bool IsIdentified { get; set; }
  IWeapon? MurderWeapon { get; }
  IPlayer? Killer { get; }
  string Id { get; }
}