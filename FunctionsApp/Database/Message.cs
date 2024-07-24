namespace FunctionsApp.Database;

public sealed class Message
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
