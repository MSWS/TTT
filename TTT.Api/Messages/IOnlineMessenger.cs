using TTT.Api.Player;

namespace TTT.Api.Messages;

public interface IOnlineMessenger : IMessenger {
  Task<bool> IMessenger.Message(IPlayer player, string message) {
    if (player is not IOnlinePlayer onlinePlayer)
      return
        Task.FromResult(false); // Cannot send message to non-online players

    return Message(onlinePlayer, message);
  }

  Task<bool> Message(IOnlinePlayer player, string message);

  async Task<bool> MessageAll(IPlayerFinder finder, string message) {
    var tasks = finder.GetAllPlayers()
     .Select(onlinePlayer => Message(onlinePlayer, message))
     .ToList();

    var results = await Task.WhenAll(tasks);
    return results.All(r => r);
  }
}