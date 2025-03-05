using LangTeacher.Server.Conversations.Responses;
using LangTeacher.Server.Services.ChatService;
using LangTeacher.Server.Shared;

namespace LangTeacher.Server.Conversations
{
    public interface IConversationService
    {
        Task<ValueResult<GetResponseResp>> GetResponseAsync(GetResponseRequest request);
        Task<IEnumerable<ConversationResponse>> GetConversationsAsync();
        Task<Result> DeleteConversationAsync(int id);
    }

    public class ConversationService : IConversationService
    {
        private readonly IChatService _chatService;
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IChatService chatService, IConversationRepository conversationRepository)
        {
            _chatService = chatService;
            _conversationRepository = conversationRepository;
        }

        public async Task<ValueResult<GetResponseResp>> GetResponseAsync(GetResponseRequest request)
        {
            if (request.ConversationId is not null)
            {
                if (!await _conversationRepository.ConversationExistsAsync(request.ConversationId.Value))
                {
                    return ValueResult<GetResponseResp>.Failure($"No conversation with id {request.ConversationId}");
                }

                var messages = await _conversationRepository.GetLastMessagesAsync(request.ConversationId.Value);

                var appMessages = messages.OrderBy(x => x.CreatedAt);

                _chatService.SetChatHistory(appMessages);
            }

            var lastResponse = await _chatService.GetResponseAsync(request.Text);

            var messagesToSave = new List<AppMessage>
            {
                new AppMessage() { Content = request.Text, Role = ChatRoles.User },
                new AppMessage() { Content = lastResponse, Role = ChatRoles.Assistant }
            };

            var conv = await _conversationRepository.AddMessagesAsync(messagesToSave, request.ConversationId);
            await _conversationRepository.SaveChangesAsync();

            var resp = new GetResponseResp
            {
                ConversationId = conv.ConversationId,
                Title = conv.Title,
                Response = lastResponse
            };

            return ValueResult<GetResponseResp>.Success(resp);
        }

        public async Task<IEnumerable<ConversationResponse>> GetConversationsAsync()
        {
            var conversations = await _conversationRepository.GetConversationsAsync();
            return conversations;
        }

        public async Task<Result> DeleteConversationAsync(int id)
        {
            var deleteResult = await _conversationRepository.DeleteAsync(id);

            if (!deleteResult)
                return Result.Failure($"Conversation with id {id} not found");

            return Result.Success();
        }
    }
}
