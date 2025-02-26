namespace LangTeacher.Server.Conversations
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public required string Title { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public List<AppMessage> AppMessages { get; set; } = [];
    }
}
