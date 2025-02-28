using LangTeacher.Server.Conversations;
using System.ComponentModel.DataAnnotations;

namespace LangTeacherTests.Conversations
{
    public class CoversationValidateTests
    {
        [Fact]
        public void GetResponseRequest_ShouldFailValidation()
        {
            // Arrange
            var model = new GetResponseRequest
            {
                Text = "",
                ConversationId = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Text required"));
        }

        [Fact]
        public void GetResponseRequest_ShouldPassValidation()
        {
            var model = new GetResponseRequest
            {
                Text = "test message",
                ConversationId = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            Assert.True(isValid);
        }
    }
}
