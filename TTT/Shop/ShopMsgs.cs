using TTT.Locale;

namespace TTT.Shop;

public static class ShopMsgs {
  public static IMsg CREDITS_NAME => MsgFactory.Create(nameof(CREDITS_NAME));

  public static IMsg CREDITS_GIVEN(int amo) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN), amo > 0 ? "+" : "-",
      Math.Abs(amo));
  }

  public static IMsg CREDITS_GIVEN_REASON(int amo, string reason) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN_REASON), amo > 0 ? "+" : "-",
      Math.Abs(amo), reason);
  }
}