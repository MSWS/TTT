using TTT.Locale;

namespace TTT.Test.Locale;

public static class TestMsgs {
  public static IMsg BASIC_TEST => MsgFactory.Create(nameof(BASIC_TEST));

  public static IMsg PLACEHOLDER_TEST(string name)
    => MsgFactory.Create(nameof(PLACEHOLDER_TEST), name);
}