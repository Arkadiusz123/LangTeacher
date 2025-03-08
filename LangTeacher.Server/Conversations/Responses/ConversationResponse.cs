namespace LangTeacher.Server.Conversations.Responses
{
    public class ConversationResponse
    {
        public required Guid ConversationId { get; set; }
        public required string Title { get; set; }
        public required DateTimeOffset LastMessageDate { get; set; }
    }
}
