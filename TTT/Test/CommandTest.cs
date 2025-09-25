using Microsoft.Testing.Platform.Services;
using TTT.API.Command;

namespace TTT.Test;

public abstract class CommandTest {
  protected readonly ICommandManager Commands;
  protected readonly ICommand Command;
  protected readonly IServiceProvider Provider;

  public CommandTest(IServiceProvider provider, ICommand command) {
    Provider = provider;
    Commands = provider.GetRequiredService<ICommandManager>();
    Command  = command;

    Commands.RegisterCommand(Command);
  }
}