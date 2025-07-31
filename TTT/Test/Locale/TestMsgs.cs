using TTT.Locale;

namespace TTT.Test.Locale;

public static class TestMsgs {
  public static IMsg CONSTANT => MsgFactory.Create(nameof(CONSTANT));

  public static IMsg CONSTANT_S => MsgFactory.Create(nameof(CONSTANT_S));

  public static IMsg TRAILING_PLURALS_CONSTANT
    => MsgFactory.Create(nameof(TRAILING_PLURALS_CONSTANT));

  public static IMsg TRAILING_PLURALS_CONSTANT_APO
    => MsgFactory.Create(nameof(TRAILING_PLURALS_CONSTANT_APO));

  public static IMsg TRAILING_PLURALS_IMPROPER_CONSTANT
    => MsgFactory.Create(nameof(TRAILING_PLURALS_IMPROPER_CONSTANT));

  public static IMsg TRAILING_PLURALS_IMPROPER_CONSTANT_APO
    => MsgFactory.Create(nameof(TRAILING_PLURALS_IMPROPER_CONSTANT_APO));

  public static IMsg PREFIX => MsgFactory.Create(nameof(PREFIX));

  public static IMsg PREFIX_TEST => MsgFactory.Create(nameof(PREFIX_TEST));

  public static IMsg WHITESPACE_SUFFIX
    => MsgFactory.Create(nameof(WHITESPACE_SUFFIX));

  public static IMsg WHITESPACE_PREFIX
    => MsgFactory.Create(nameof(WHITESPACE_PREFIX));

  public static IMsg PREFIX_TEST_TRAILING
    => MsgFactory.Create(nameof(PREFIX_TEST_TRAILING));

  public static IMsg SUFFIX_TEST_TRAILING
    => MsgFactory.Create(nameof(SUFFIX_TEST_TRAILING));

  public static IMsg BOTH_TEST_TRAILING
    => MsgFactory.Create(nameof(BOTH_TEST_TRAILING));

  public static IMsg SINGLE_SUBSTITUTION(string name) {
    return MsgFactory.Create(nameof(SINGLE_SUBSTITUTION), name);
  }

  public static IMsg FLOAT_FORMATTING(float value) {
    return MsgFactory.Create(nameof(FLOAT_FORMATTING), value);
  }

  public static IMsg TRAILING_PLURALS(string s) {
    return MsgFactory.Create(nameof(TRAILING_PLURALS), s);
  }

  public static IMsg TRAILING_PLURALS_APO(string s) {
    return MsgFactory.Create(nameof(TRAILING_PLURALS_APO), s);
  }

  public static IMsg TRAILING_PLURALS_DOUBLE(int amo, string obj) {
    return MsgFactory.Create(nameof(TRAILING_PLURALS_DOUBLE), amo, obj);
  }
}