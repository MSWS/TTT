using TTT.Api.Player;

namespace TTT.Api;

public interface IPlayerConverter<in T> : ITerrorModule {
  public IPlayer GetPlayer(T player);
}