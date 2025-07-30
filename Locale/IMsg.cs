namespace TTT.Locale;

public interface IMsg {
  string Key { get; }
  object[] Args { get; }
}