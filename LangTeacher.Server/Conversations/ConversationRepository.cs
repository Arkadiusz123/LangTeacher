using LangTeacher.Server.Conversations.Responses;
using LangTeacher.Server.Database;
using LangTeacher.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace LangTeacher.Server.Conversations
{
    public interface IConversationRepository
    {
        Task<Conversation> AddMessagesAsync(IEnumerable<AppMessage> messages, int? conversationId);
        Task<IEnumerable<AppMessage>> GetLastMessagesAsync(int conversationId, int limit = 10);       
        Task<IEnumerable<ConversationResponse>> GetConversationsAsync();
        Task SaveChangesAsync();
    }

    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _dbContext;

        public ConversationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation> AddMessagesAsync(IEnumerable<AppMessage> messages, int? conversationId) 
        {
            Conversation conversation;

            if (conversationId is null) 
            {
                var firstMessage = messages.OrderBy(x => x.CreatedAt).First();

                conversation = new Conversation()
                {
                    Title = firstMessage.Content.GetConversationTitle()
                };

                _dbContext.Conversations.Add(conversation);
            }
            else
            {
                conversation = await _dbContext.Conversations.SingleOrDefaultAsync(x => x.ConversationId == conversationId);
            }

            conversation.AppMessages.AddRange(messages);
            return conversation;
        }

        public async Task<IEnumerable<AppMessage>> GetLastMessagesAsync(int conversationId, int limit = 10) 
        { 
            var messages = await _dbContext.Messages
                .AsNoTracking()
                .Where(x => x.ConversationId == conversationId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<ConversationResponse>> GetConversationsAsync()
        {
            var query = await _dbContext.Conversations
                .Select(x => new ConversationResponse
                {
                    ConversationId = x.ConversationId,
                    LastMessageDate = x.AppMessages.Select(y => y.CreatedAt).Max(),
                    Title = x.Title
                })
                .OrderByDescending(x => x.LastMessageDate)
                .ToListAsync();

            return query;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
