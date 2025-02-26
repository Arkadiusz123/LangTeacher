namespace LangTeacher.Server.Conversations
{
    public class AppMessage
    {
        public int AppMessageId { get; set; }
        public required string Role { get; set; }
        public required string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
