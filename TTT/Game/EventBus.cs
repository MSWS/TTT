using System.Reflection;
using TTT.API.Events;

namespace TTT.Game;

public class EventBus : IEventBus {
  private readonly Dictionary<Type, List<(object listener, MethodInfo method)>>
    handlers = new();

  public void RegisterListener(IListener listener) {
    var methods = listener.GetType()
     .GetMethods(BindingFlags.Instance | BindingFlags.Public
        | BindingFlags.NonPublic);

    var dirtyTypes = new HashSet<Type>();

    foreach (var method in methods) {
      var attr = method.GetCustomAttribute<EventHandlerAttribute>();
      if (attr == null) continue;

      var parameters = method.GetParameters();
      if (parameters.Length != 1
        || !typeof(Event).IsAssignableFrom(parameters[0].ParameterType))
        throw new InvalidOperationException(
          $"Method {method.Name} in {listener.GetType().Name} "
          + "must have exactly one parameter of type Event or its subclass.");

      var eventType = parameters[0].ParameterType;
      if (!handlers.ContainsKey(eventType)) handlers[eventType] = [];

      handlers[eventType].Add((listener, method));
      dirtyTypes.Add(eventType);
    }

    if (dirtyTypes.Count == 0)
      throw new InvalidOperationException(
        $"Listener {listener.GetType().Name} has no valid event handlers.");

    // Sort handlers by priority
    foreach (var type in dirtyTypes)
      handlers[type]
       .Sort((a, b) => {
          var aPriority = a.method.GetCustomAttribute<EventHandlerAttribute>()
          ?.Priority ?? Priority.DEFAULT;
          var bPriority = b.method.GetCustomAttribute<EventHandlerAttribute>()
          ?.Priority ?? Priority.DEFAULT;
          return aPriority.CompareTo(bPriority);
        });
  }

  public void UnregisterListener(IListener listener) {
    foreach (var kvp in handlers) {
      kvp.Value.RemoveAll(h => h.listener == listener);
      if (kvp.Value.Count == 0) handlers.Remove(kvp.Key);
    }
  }

  public Task Dispatch(Event ev) {
    var type = ev.GetType();
    if (!handlers.TryGetValue(type, out var list)) return Task.CompletedTask;
    ICancelableEvent? cancelable           = null;
    if (ev is ICancelableEvent) cancelable = (ICancelableEvent)ev;

    List<Task> tasks = [];

    foreach (var (listener, method) in list) {
      if (cancelable is { IsCanceled: true } && method
       .GetCustomAttribute<EventHandlerAttribute>()
      ?.IgnoreCanceled == true)
        continue;

      var result = method.Invoke(listener, [ev]);
      if (result is Task task) tasks.Add(task);
    }

    return Task.WhenAll(tasks);
  }
}