using LangTeacher.Server.Conversations;
using LangTeacher.Server.Database;
using LangTeacher.Server.Services.ChatService;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;

namespace LangTeacher.Server.Configs
{
    public class ServicesConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<AppDbContext>(o => o.UseNpgsql(configuration.GetConnectionString("ConnStr")));

            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IChatService, OllamaService>();

            services.AddSingleton<OllamaApiClient>(provider =>
            {
                var apiUrl = configuration["Ollama:Url"];
                var uri = new Uri(apiUrl);
                var client = new OllamaApiClient(uri);
                client.SelectedModel = configuration["Ollama:Model"];
                return client;
            });
        }
    }
}
