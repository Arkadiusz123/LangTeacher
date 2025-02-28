using LangTeacher.Server.Conversations;
using OllamaSharp.Models.Chat;

namespace LangTeacherTests.Conversations
{
    public class ConversationMapperTests
    {
        [Fact]
        public void MapOllamaMessageToAppMessageTest()
        {
            var ollamaMessage = new Message
            {
                Content = "Test content",
                Role = ChatRole.Assistant
            };

            var mapResult = ollamaMessage.ToAppMessage();

            Assert.Equal("Test content", mapResult.Content);
            Assert.Equal("assistant", mapResult.Role);
        }

        [Fact]
        public void MapAppMessageToOllamaMessageTest()
        {
            var appMessage = new AppMessage
            {
                Content = "Test content",
                Role = "assistant"
            };

            var mapResult = appMessage.ToOllamaChatMessage();

            Assert.Equal("Test content", mapResult.Content);
            Assert.Equal("assistant", mapResult.Role);
        }
    }
}
