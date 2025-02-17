using LangTeacher.Server.Conversations;
using Microsoft.EntityFrameworkCore;

namespace LangTeacher.Server.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<AppMessage> Messages { get; set; }
    }
}
