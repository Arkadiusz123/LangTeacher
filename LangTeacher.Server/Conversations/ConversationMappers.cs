using OllamaSharp.Models.Chat;

namespace LangTeacher.Server.Conversations
{
    public static class ConversationMappers
    {
        public static AppMessage ToAppMessage(this Message chatMessage)
        {
            return new AppMessage()
            {
                Content = chatMessage.Content,
                Role = chatMessage.Role.ToString()
            };
        }

        public static Message ToChatMessage(this AppMessage appMessage)
        {
            return new Message(new ChatRole(appMessage.Role), appMessage.Content);
        }
    }
}
