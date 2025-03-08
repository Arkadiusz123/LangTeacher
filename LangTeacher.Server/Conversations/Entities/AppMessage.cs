namespace LangTeacher.Server.Conversations
{
    public class AppMessage
    {
        public Guid AppMessageId { get; set; } = Guid.CreateVersion7();
        public required string Role { get; set; }
        public required string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
