﻿using LangTeacher.Server.Conversations.Responses;
using LangTeacher.Server.Services.ChatService;

namespace LangTeacher.Server.Conversations
{
    public interface IConversationService
    {
        Task<GetResponseResp> GetResponseAsync(GetResponseRequest request);
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

        public async Task<GetResponseResp> GetResponseAsync(GetResponseRequest request)
        {
            if (request.ConversationId is not null)
            {
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
                Response = lastResponse
            };

            return resp;
        }
    }
}
