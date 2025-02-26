namespace LangTeacher.Server.Services
{
    public static class StringExtensions
    {
        public static string GetConversationTitle(this string input, int wordCount = 5)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Untitled";

            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Take(wordCount)) + "...";
        }
    }
}
