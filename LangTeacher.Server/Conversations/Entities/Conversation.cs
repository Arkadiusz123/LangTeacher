namespace LangTeacher.Server.Conversations
{
    public class Conversation
    {
        public Guid ConversationId { get; set; } = Guid.CreateVersion7();
        public required string Title { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public List<AppMessage> AppMessages { get; set; } = [];
    }
}
