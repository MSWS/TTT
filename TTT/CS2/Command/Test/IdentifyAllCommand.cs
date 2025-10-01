using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.API;
using TTT.Game.Events.Body;

namespace TTT.CS2.Command.Test;

public class IdentifyAllCommand(IServiceProvider provider) : ICommand {
  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public string Id => "identifyall";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    foreach (var body in bodies.Bodies.Keys) {
      if (body.IsIdentified) continue;
      var bodyIdentifyEvent = new BodyIdentifyEvent(body, executor);
      Server.NextWorldUpdate(() => bus.Dispatch(bodyIdentifyEvent));
    }

    info.ReplySync("Identified all bodies.");
    return Task.FromResult(CommandResult.SUCCESS);
  }
}