namespace LangTeacher.Server.Conversations.Responses
{
    public class GetResponseResp
    {
        /// <example>12</example>
        public required int ConversationId { get; set; }

        /// <example>title_example</example>
        public required string Title { get; set; }

        /// <example>this is example ai response</example>
        public required string Response { get; set; }
    }
}
