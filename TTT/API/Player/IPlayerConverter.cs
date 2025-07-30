namespace TTT.Api.Player;

public interface IPlayerConverter<in T> : ITerrorModule {
  public IPlayer GetPlayer(T player);
}