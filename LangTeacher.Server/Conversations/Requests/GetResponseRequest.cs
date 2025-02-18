using System.ComponentModel.DataAnnotations;

namespace LangTeacher.Server.Conversations
{
    public class GetResponseRequest : IValidatableObject
    {
        public required string Text { get; set; }
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
