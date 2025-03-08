using LangTeacher.Server.Conversations;
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
            var exampleGuid = Guid.CreateVersion7();

            _conversationRepository.ConversationExistsAsync(exampleGuid).Returns(false);

            var request = new GetResponseRequest 
            { 
                ConversationId = exampleGuid,
                Text = "test" 
            };

            var result = await _conversationService.GetResponseAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal($"No conversation with id {exampleGuid}", result.Error);

            _conversationRepository.Received(1).ConversationExistsAsync(exampleGuid);
            _conversationRepository.DidNotReceive().GetLastMessagesAsync(Arg.Any<Guid>());
            _chatService.DidNotReceive().SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _conversationRepository.DidNotReceive().AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<Guid?>());
            _conversationRepository.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task GetResponse_CorrectDataWithoutConversationId_ReturnsSuccess()
        {
            var conversation = new Conversation
            {
                Title = "test_title"
            };

            _conversationRepository.AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<Guid?>()).Returns(conversation);

            var request = new GetResponseRequest
            {
                ConversationId = null,
                Text = "test"
            };

            var result = await _conversationService.GetResponseAsync(request);

            Assert.True(result.IsSuccess);

            _conversationRepository.DidNotReceive().ConversationExistsAsync(Arg.Any<Guid>());
            _conversationRepository.DidNotReceive().GetLastMessagesAsync(Arg.Any<Guid>());
            _chatService.DidNotReceive().SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _chatService.Received(1).GetResponseAsync("test");
            _conversationRepository.Received(1).AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), Arg.Any<Guid?>());
            _conversationRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetResponse_CorrectDataWithConversationId_ReturnsSuccess()
        {
            var exampleGuid = Guid.CreateVersion7();

            var conversation = new Conversation
            {
                Title = "test_title",
                ConversationId = exampleGuid
            };

            _conversationRepository.AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), exampleGuid).Returns(conversation);

            var request = new GetResponseRequest
            {
                ConversationId = exampleGuid,
                Text = "test"
            };

            _conversationRepository.ConversationExistsAsync(exampleGuid).Returns(true);

            var result = await _conversationService.GetResponseAsync(request);           

            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.ConversationId, exampleGuid);

            _conversationRepository.Received(1).ConversationExistsAsync(exampleGuid);
            _conversationRepository.Received(1).GetLastMessagesAsync(exampleGuid);
            _chatService.Received(1).SetChatHistory(Arg.Any<IEnumerable<AppMessage>>());
            _chatService.Received(1).GetResponseAsync("test");
            _conversationRepository.Received(1).AddMessagesAsync(Arg.Any<IEnumerable<AppMessage>>(), exampleGuid);
            _conversationRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task GetConversations_ReturnsSuccess()
        {
            var exampleGuid = Guid.CreateVersion7();

            var conversations = new List<ConversationResponse>
            {
                new ConversationResponse { Title = "test", ConversationId = exampleGuid, LastMessageDate = DateTimeOffset.UtcNow },
                new ConversationResponse { Title = "test2", ConversationId = exampleGuid, LastMessageDate = DateTimeOffset.UtcNow.AddSeconds(2) },
            };

            _conversationRepository.GetConversationsAsync().Returns(conversations);

            var result = await _conversationService.GetConversationsAsync();

            Assert.Equal(conversations, result);
            _conversationRepository.Received(1).GetConversationsAsync();
        }

        [Fact]
        public async Task DeleteConversation_CorrectId_ReturnsSuccess()
        {
            var exampleGuid = Guid.CreateVersion7();

            _conversationRepository.DeleteAsync(exampleGuid).Returns(true);

            var deleteResult = await _conversationService.DeleteConversationAsync(exampleGuid);

            Assert.True(deleteResult.IsSuccess);
            _conversationRepository.Received(1).DeleteAsync(exampleGuid);
        }

        [Fact]
        public async Task DeleteConversation_WrongId_ReturnsFail()
        {
            var exampleGuid = Guid.CreateVersion7();

            _conversationRepository.DeleteAsync(exampleGuid).Returns(false);

            var deleteResult = await _conversationService.DeleteConversationAsync(exampleGuid);

            Assert.False(deleteResult.IsSuccess);
            Assert.Equal($"Conversation with id {exampleGuid} not found", deleteResult.Error);

            _conversationRepository.Received(1).DeleteAsync(exampleGuid);
        }
    }
}
