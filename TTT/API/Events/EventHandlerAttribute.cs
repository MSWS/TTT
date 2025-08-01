namespace TTT.API.Events;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute {
  public uint Priority { get; set; } = Events.Priority.DEFAULT;
  public bool IgnoreCanceled { get; set; } = false;
}

/// <summary>
///   Represents the priority levels for event handlers.
///   The lower the number, the higher the priority.
///   Higher priority handlers are executed before lower priority ones. Thus,
///   if you want a handler to have final say in an event's processing,
///   use a lower priority to be executed last.
/// </summary>
public static class Priority {
  public const uint HIGHEST = 10;
  public const uint VERY_HIGH = 20;
  public const uint HIGH = 40;
  public const uint DEFAULT = 60;
  public const uint LOW = 80;
  public const uint VERY_LOW = 100;
  public const uint LOWEST = 200;
}