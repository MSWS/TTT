using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;

namespace TTT.API.Extensions;

/// <summary>
///   We use this to register all instances of <see cref="ITerrorModule" /> under
///   its interface, allowing us to get the list of all modules simply using ITerrorModule.
/// </summary>
public static class ServiceCollectionExtensions {
  public static void AddModBehavior<TExtension>(
    this IServiceCollection collection)
    where TExtension : class, ITerrorModule {
    if (typeof(TExtension).IsAssignableTo(typeof(IPluginModule)))
      collection.AddTransient<IPluginModule>(provider
        => (provider.GetRequiredService<TExtension>() as IPluginModule)!);

    if (typeof(TExtension).IsAssignableTo(typeof(IListener)))
      collection.AddTransient<IListener>(provider
        => (provider.GetRequiredService<TExtension>() as IListener)!);

    if (typeof(TExtension).IsAssignableTo(typeof(ICommand)))
      collection.AddTransient<ICommand>(provider
        => (provider.GetRequiredService<TExtension>() as ICommand)!);

    collection.AddScoped<TExtension>();

    collection.AddTransient<ITerrorModule, TExtension>(provider
      => provider.GetRequiredService<TExtension>());
  }

  /// <summary>
  ///   Add a <see cref="ITerrorModule" /> to the global service collection
  /// </summary>
  /// <param name="collection"></param>
  /// <typeparam name="TExtension"></typeparam>
  /// <typeparam name="TInterface"></typeparam>
  public static void AddModBehavior<TInterface, TExtension>(
    this IServiceCollection collection)
    where TExtension : class, ITerrorModule, TInterface
    where TInterface : class {
    //	Add the root extension itself as a scoped service.
    //	This means every time Load is called in the main Jailbreak loader,
    //	the extension will be fetched and kept as a singleton for the duration
    //	until "Unload" is called.
    collection.AddModBehavior<TExtension>();
    collection.AddTransient<TInterface, TExtension>(p
      => p.GetRequiredService<TExtension>());
  }
}