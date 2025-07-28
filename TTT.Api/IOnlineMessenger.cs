using TTT.Api.Player;

namespace TTT.Api;

public interface IOnlineMessenger : IMessenger {
  Task<bool> IMessenger.Message(IPlayer player, string message) {
    if (player is not IOnlinePlayer onlinePlayer)
      return
        Task.FromResult(false); // Cannot send message to non-online players

    return Message(onlinePlayer, message);
  }

  Task<bool> Message(IOnlinePlayer player, string message);
}