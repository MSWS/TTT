namespace TTT.Api.Events;

[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute {
  public uint Priority => Events.Priority.PRIORITY_DEFAULT;
  public bool IgnoreCanceled => false;
}

public static class Priority {
  public const uint PRIORITY_VERY_HIGH = 20;
  public const uint PRIORITY_HIGH = 40;
  public const uint PRIORITY_NORMAL = 60;
  public const uint PRIORITY_LOW = 80;
  public const uint PRIORITY_VERY_LOW = 100;
  public const uint PRIORITY_DEFAULT = 50;
}