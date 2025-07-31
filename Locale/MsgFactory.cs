namespace TTT.Locale;

public static class MsgFactory {
  public static IMsg Create(string key, params object[] args) {
    return new Msg(key, args);
  }

  private sealed record Msg(string Key, object[] Args) : IMsg;
}