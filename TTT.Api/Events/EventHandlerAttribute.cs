namespace TTT.Api.Events;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute {
  public uint Priority { get; set; } = Events.Priority.DEFAULT;
  public bool IgnoreCanceled { get; set; } = false;
}

public static class Priority {
  public const uint VERY_HIGH = 20;
  public const uint HIGH = 40;
  public const uint DEFAULT = 60;
  public const uint LOW = 80;
  public const uint VERY_LOW = 100;
}