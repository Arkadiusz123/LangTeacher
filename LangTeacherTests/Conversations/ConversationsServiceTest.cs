﻿using LangTeacher.Server.Conversations;
using LangTeacher.Server.Conversations.Responses;
using LangTeacher.Server.Services.ChatService;
using NSubstitute;

namespace LangTeacherTests.Conversations
{
    public class ConversationsServiceTest
    {
        private readonly ConversationService _conversationService;
        private readonly IChatService _chatService;
        private readonly IConversationRepository _conversationRepository;

        public ConversationsServiceTest()
        {
            _chatService = Substitute.For<IChatService>();
            _conversationRepository = Substitute.For<IConversationRepository>();

            _conversationService = new ConversationService(_chatService, _conversationRepository);
        }

        [Fact]
        public async Task GetResponse_WrongConversationId_ReturnsFail()
        {
            _conversationRepository.ConversationExistsAsync(4).Returns(false);

            var request = new GetResponseRequest 
            { 
                ConversationId = 4,
                Text = "test" 
            };

            var result = await _conversationService.GetResponseAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("No conversation with id 4", result.Error);

            _conversationRepository.Received(1).ConversationExistsAsync(4);
            _conversationRepository.DidNotReceive().GetLastMessagesAsync(Arg.Any<int>());
            _chatService.DidNotReceive().SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _conversationRepository.DidNotReceive().AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<int?>());
            _conversationRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task GetResponse_CorrectDataWithoutConversationId_ReturnsSuccess()
        {
            var conversation = new Conversation
            {
                Title = "test_title"
            };

            _conversationRepository.AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<int?>()).Returns(conversation);

            var request = new GetResponseRequest
            {
                ConversationId = null,
                Text = "test"
            };

            var result = await _conversationService.GetResponseAsync(request);

            Assert.True(result.IsSuccess);

            _conversationRepository.DidNotReceive().ConversationExistsAsync(Arg.Any<int>());
            _conversationRepository.DidNotReceive().GetLastMessagesAsync(Arg.Any<int>());
            _chatService.DidNotReceive().SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _chatService.Received(1).GetResponseAsync("test");
            _conversationRepository.Received(1).AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<int?>());
            _conversationRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetResponse_CorrectDataWithConversationId_ReturnsSuccess()
        {
            var conversation = new Conversation
            {
                Title = "test_title",
                ConversationId = 1
            };

            _conversationRepository.AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), 1).Returns(conversation);

            var request = new GetResponseRequest
            {
                ConversationId = 1,
                Text = "test"
            };

            _conversationRepository.ConversationExistsAsync(1).Returns(true);

            var result = await _conversationService.GetResponseAsync(request);           

            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.ConversationId, 1);

            _conversationRepository.Received(1).ConversationExistsAsync(1);
            _conversationRepository.Received(1).GetLastMessagesAsync(1);
            _chatService.Received(1).SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _chatService.Received(1).GetResponseAsync("test");
            _conversationRepository.Received(1).AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), 1);
            _conversationRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetConversations_ReturnsSuccess()
        {
            var conversations = new List<ConversationResponse>
            {
                new ConversationResponse { Title = "test", ConversationId = 1, LastMessageDate = DateTimeOffset.UtcNow },
                new ConversationResponse { Title = "test2", ConversationId = 2, LastMessageDate = DateTimeOffset.UtcNow.AddSeconds(2) },
            };

            _conversationRepository.GetConversationsAsync().Returns(conversations);

            var result = await _conversationService.GetConversationsAsync();

            Assert.Equal(conversations, result);
            _conversationRepository.Received(1).GetConversationsAsync();
        }
    }
}
