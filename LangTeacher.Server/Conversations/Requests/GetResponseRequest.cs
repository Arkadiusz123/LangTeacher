using System.ComponentModel.DataAnnotations;

namespace LangTeacher.Server.Conversations
{
    public class GetResponseRequest : IValidatableObject
    {
        /// <summary>
        /// your message to ai
        /// </summary>
        /// <example>question you would like to ask</example>
        public required string Text { get; set; }

        /// <summary>
        /// optional id of conversation, that you wolud like to continue, if empty then start new
        /// </summary>
        /// <example>12</example>
        public int? ConversationId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Text))
            {
                yield return new ValidationResult("Text required", [nameof(Text)]);
            }
        }
    }
}
