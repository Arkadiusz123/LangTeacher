using LangTeacher.Server.Conversations.Responses;
using LangTeacher.Server.Database;
using LangTeacher.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace LangTeacher.Server.Conversations
{
    public interface IConversationRepository
    {
        Task<Conversation> AddMessagesAsync(IEnumerable<AppMessage> messages, Guid? conversationId);
        Task<IEnumerable<AppMessage>> GetLastMessagesAsync(Guid conversationId, int limit = 20);       
        Task<IEnumerable<ConversationResponse>> GetConversationsAsync();
        Task<bool> ConversationExistsAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }

    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _dbContext;

        public ConversationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation> AddMessagesAsync(IEnumerable<AppMessage> messages, Guid? conversationId) 
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
                conversation = await _dbContext.Conversations.SingleAsync(x => x.ConversationId == conversationId);
            }
            
            conversation.AppMessages.AddRange(messages);
            _dbContext.Messages.AddRange(messages);

            return conversation;
        }

        public async Task<IEnumerable<AppMessage>> GetLastMessagesAsync(Guid conversationId, int limit = 20) 
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

        public async Task<bool> ConversationExistsAsync(Guid id)
        {
            return await _dbContext.Conversations.AnyAsync(x => x.ConversationId == id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deletedCount = await _dbContext.Conversations
                .Where(x => x.ConversationId == id)
                .ExecuteDeleteAsync();

            return deletedCount == 1;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
