using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;

namespace TTT.Game;

public class EventBus(IServiceProvider provider) : IEventBus, ITerrorModule {
  private readonly Dictionary<Type, List<(object listener, MethodInfo method)>>
    handlers = new();

  public void RegisterListener(IListener listener) {
    var dirtyTypes = new HashSet<Type>();
    appendListener(listener, dirtyTypes);

    if (dirtyTypes.Count == 0)
      throw new InvalidOperationException(
        $"Listener {listener.GetType().Name} has no valid event handlers.");

    resortListeners(dirtyTypes);
  }

  public void UnregisterListener(IListener listener) {
    foreach (var kvp in handlers) {
      kvp.Value.RemoveAll(h => h.listener == listener);
      if (kvp.Value.Count == 0) handlers.Remove(kvp.Key);
    }
  }

  public void Dispatch(Event ev) {
    var type = ev.GetType();

    handlers.TryGetValue(type, out var list);

    if (list == null || list.Count == 0) return;

    ICancelableEvent? cancelable           = null;
    if (ev is ICancelableEvent) cancelable = (ICancelableEvent)ev;

    foreach (var (listener, method) in list) {
      if (cancelable is { IsCanceled: true } && method
       .GetCustomAttribute<EventHandlerAttribute>()
      ?.IgnoreCanceled == true)
        continue;

      method.Invoke(listener, [ev]);
    }
  }

  public void Dispose() { handlers.Clear(); }

  public void Start() {
    var listeners = provider.GetServices<IListener>().ToList();
    foreach (var listener in listeners) RegisterListener(listener);
  }

  private void resortListeners(HashSet<Type> dirtyTypes) {
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

  private void appendListener(IListener listener, HashSet<Type> dirtyTypes) {
    var methods = listener.GetType()
     .GetMethods(BindingFlags.Instance | BindingFlags.Public
        | BindingFlags.NonPublic);
    foreach (var method in methods)
      registerListenerMethod(listener, dirtyTypes, method);
  }

  private void registerListenerMethod(IListener listener,
    HashSet<Type> dirtyTypes, MethodInfo method) {
    var attr = method.GetCustomAttribute<EventHandlerAttribute>();
    if (attr == null) return;

    if (method.ReturnType != typeof(void))
      throw new InvalidOperationException(
        $"Method {method.Name} in {listener.GetType().Name} "
        + "must have void return type.");

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
}