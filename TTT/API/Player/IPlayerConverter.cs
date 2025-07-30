namespace TTT.API.Player;

public interface IPlayerConverter<T> : ITerrorModule {
  public IPlayer GetPlayer(T player);
  public T? GetPlayer(IPlayer player);
}