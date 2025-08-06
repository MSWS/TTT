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

  public static IMsg TEST_PREFIX => MsgFactory.Create(nameof(TEST_PREFIX));

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

  public static IMsg AN_TEST_LOWER => MsgFactory.Create(nameof(AN_TEST_LOWER));

  public static IMsg AN_TEST_UPPER => MsgFactory.Create(nameof(AN_TEST_UPPER));

  public static IMsg AN_AMAZING_TEST_LOWER
    => MsgFactory.Create(nameof(AN_AMAZING_TEST_LOWER));


  public static IMsg AN_AMAZING_TEST_UPPER
    => MsgFactory.Create(nameof(AN_AMAZING_TEST_UPPER));

  public static IMsg AN_AMAZING_TEST_SENTENCE
    => MsgFactory.Create(nameof(AN_AMAZING_TEST_SENTENCE));

  public static IMsg AN_AMAZING_TEST_ODD
    => MsgFactory.Create(nameof(AN_AMAZING_TEST_ODD));

  public static IMsg AN_TEST_SUBSTITUTION_LOWER(string name) {
    return MsgFactory.Create(nameof(AN_TEST_SUBSTITUTION_LOWER), name);
  }

  public static IMsg AN_TEST_SUBSTITUTION_UPPER(string name) {
    return MsgFactory.Create(nameof(AN_TEST_SUBSTITUTION_UPPER), name);
  }

  public static IMsg AN_IN_BETWEEN => MsgFactory.Create(nameof(AN_IN_BETWEEN));

  public static IMsg AN_AT_END => MsgFactory.Create(nameof(AN_AT_END));
}