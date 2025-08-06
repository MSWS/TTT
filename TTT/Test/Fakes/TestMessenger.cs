using TTT.API.Player;
using TTT.Game;

namespace TTT.Test.Fakes;

public class TestMessenger(IServiceProvider provider)
  : EventModifiedMessenger(provider) {
  override protected Task<bool> SendMessage(IPlayer? player, string message,
    params object[] args) {
    if (player is not TestPlayer testPlayer)
      throw new ArgumentException("Player must be a TestPlayer",
        nameof(player));

    if (args.Length > 0) message = string.Format(message, args);

    testPlayer.Messages.Add(message);
    return Task.FromResult(true);
  }

  public override void Debug(string msg, params object[] args) {
    if (args.Length > 0) msg = string.Format(msg, args);
    Console.WriteLine(msg);
  }
}