namespace TTT.API.Player;

public interface IPlayerConverter<in T> : ITerrorModule {
  public IPlayer GetPlayer(T player);
}