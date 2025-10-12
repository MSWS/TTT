using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class TestCommand(IServiceProvider provider) : ICommand, IPluginModule {
  private readonly IDictionary<string, ICommand> subCommands =
    new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

  public void Dispose() { }

  public string Id => "test";

  public void Start() {
    subCommands.Add("setrole", new SetRoleCommand(provider));
    subCommands.Add("stop", new StopCommand(provider));
    subCommands.Add("forcealive", new ForceAliveCommand(provider));
    subCommands.Add("identifyall", new IdentifyAllCommand(provider));
    subCommands.Add("state", new StateCommand(provider));
    subCommands.Add("screencolor", new ScreenColorCommand(provider));
    subCommands.Add("giveitem", new GiveItemCommand(provider));
    subCommands.Add("index", new IndexCommand());
    subCommands.Add("showicons", new ShowIconsCommand(provider));
    subCommands.Add("sethealth", new SetHealthCommand());
    subCommands.Add("emitsound", new EmitSoundCommand(provider));
    subCommands.Add("credits", new CreditsCommand(provider));
  }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    if (info.ArgCount == 1) {
      foreach (var c in subCommands.Values)
        info.ReplySync(
          $"- {c.Id} {c.Usage.FirstOrDefault()}: {c.Description ?? "No description provided."}");

      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    if (!subCommands.TryGetValue(info.Args[1], out var cmd)) {
      info.ReplySync($"Unknown sub-command '{info.Args[1]}'");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    return cmd.Execute(executor, info.Skip());
  }

  public void Start(BasePlugin? plugin, bool hotload) {
    ((IPluginModule)this).Start();
    foreach (var cmd in subCommands.Values.OfType<IPluginModule>())
      cmd.Start(plugin, hotload);
  }
}