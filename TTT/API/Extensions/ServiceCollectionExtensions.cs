using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;

namespace TTT.API.Extensions;

/// <summary>
///   We use this to register all instances of <see cref="ITerrorModule" /> under
///   its interface, allowing us to get the list of all modules simply using ITerrorModule.
/// </summary>
public static class ServiceCollectionExtensions {
  /// <summary>
  /// Adds a <see cref="IPluginModule" /> to the global service collection,
  /// this method should be used for all modules that implement <see cref="IPluginModule" />.
  /// </summary>
  /// <param name="collection"></param>
  /// <typeparam name="TExtension"></typeparam>
  // public static void AddPluginBehavior<TExtension>(
  //   this IServiceCollection collection)
  //   where TExtension : class, IPluginModule {
  //   collection.AddScoped<TExtension>();
  //   collection.AddTransient<ITerrorModule>(p
  //     => p.GetRequiredService<TExtension>());
  //   collection.AddTransient<IPluginModule, TExtension>(p
  //     => p.GetRequiredService<TExtension>());
  // }

  // public static void AddPluginBehavior<TInterface, TExtension>(
  //   this IServiceCollection collection)
  //   where TExtension : class, TInterface, IPluginModule
  //   where TInterface : class {
  //   //	Add the root extension itself as a scoped service.
  //   //	This means every time Load is called in the main Jailbreak loader,
  //   //	the extension will be fetched and kept as a singleton for the duration
  //   //	until "Unload" is called.
  //   // collection.AddScoped<IPluginBehavior, PluginBehavior>();
  //   // collection.AddScoped<TExtension>();
  //   // collection.AddTransient<IPluginModule, TExtension>(provider
  //   //   => provider.GetRequiredService<TExtension>());
  //   // collection.AddModBehavior<TExtension>();
  //   collection.AddPluginBehavior<TExtension>();
  //   collection.AddTransient<TInterface, TExtension>(p
  //     => p.GetRequiredService<TExtension>());
  // }
  public static void AddModBehavior<TExtension>(
    this IServiceCollection collection)
    where TExtension : class, ITerrorModule {
    if (typeof(IPluginModule).IsAssignableFrom(typeof(TExtension)))
      collection.AddTransient<IPluginModule>(provider
        => (provider.GetRequiredService<TExtension>() as IPluginModule)!);

    if (typeof(IListener).IsAssignableFrom(typeof(TExtension)))
      collection.AddTransient<IListener>(provider
        => (provider.GetRequiredService<TExtension>() as IListener)!);

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