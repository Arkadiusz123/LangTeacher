using LangTeacher.Server.Conversations;
using OllamaSharp;

namespace LangTeacher.Server.Services.ChatService
{
    public class OllamaService : IChatService
    {
        private readonly OllamaApiClient _ollamaApiClient;
        private readonly Chat _chat;

        public OllamaService(OllamaApiClient ollamaApiClient)
        {
            _ollamaApiClient = ollamaApiClient;
            _chat = new Chat(_ollamaApiClient);
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            await foreach (var answerToken in _chat.SendAsync(prompt))
            {
            }

            var lastResponse = _chat.Messages.LastOrDefault()?.Content;
            return lastResponse ?? "";
        }

        public void SetChatHistory(IEnumerable<AppMessage> messages)
        {
            _chat.Messages.Clear();

            var ollamaMessages = messages.Select(x => x.ToOllamaChatMessage());
            _chat.Messages.AddRange(ollamaMessages);
        }
    }
}
