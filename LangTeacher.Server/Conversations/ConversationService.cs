using LangTeacher.Server.Conversations.Responses;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace LangTeacher.Server.Conversations
{
    public interface IConversationService
    {
        Task<GetResponseResp> GetResponseAsync(GetResponseRequest request);
    }

    public class ConversationService : IConversationService
    {
        //private readonly OllamaApiClient _ollamaApiClient;
        private readonly IConversationRepository _conversationRepository;

        private static List<Message> _savedMessages = new List<Message>();

        public ConversationService(OllamaApiClient ollamaApiClient, IConversationRepository conversationRepository)
        {
            _ollamaApiClient = ollamaApiClient;
            _conversationRepository = conversationRepository;
        }

        public async Task<GetResponseResp> GetResponseAsync(GetResponseRequest request)
        {
            //var chat = new Chat(_ollamaApiClient);

            if(request.ConversationId is not null)
            {
                var messages = await _conversationRepository.GetLastMessagesAsync(request.ConversationId.Value);

                var appMessages = messages
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => x.ToChatMessage());

                chat.Messages.AddRange(appMessages);
            }

            await foreach (var answerToken in chat.SendAsync(request.Text))
            {
            }

            //var lastResponse = chat.Messages.Last().Content;

            //var messagesToSave = chat.Messages[^2..].Select(x => x.ToAppMessage());

            var messagesToSave = new List<AppMessage>();
            messagesToSave.Add(new AppMessage () { Content = request.Text, Role = ChatRoles.Assistant });
            messagesToSave.Add(new AppMessage () { Content = lastResponse, Role = ChatRoles.User });

            var conv = await _conversationRepository.AddMessagesAsync(messagesToSave, request.ConversationId);
            await _conversationRepository.SaveChangesAsync();

            var resp = new GetResponseResp
            {
                ConverstaionId = conv.ConversationId,
                Response = lastResponse
            };

            return resp;
        }
    }
}
