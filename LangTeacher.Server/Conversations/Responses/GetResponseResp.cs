namespace LangTeacher.Server.Conversations.Responses
{
    public class GetResponseResp
    {
        /// <example>0ea69312-44c4-441d-b256-2d044940deb0</example>
        public required Guid ConversationId { get; set; }

        /// <example>title_example</example>
        public required string Title { get; set; }

        /// <example>this is example ai response</example>
        public required string Response { get; set; }
    }
}
