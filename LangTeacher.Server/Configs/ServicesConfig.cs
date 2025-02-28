using LangTeacher.Server.Conversations;
using LangTeacher.Server.Database;
using LangTeacher.Server.Services.ChatService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OllamaSharp;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace LangTeacher.Server.Configs
{
    public class ServicesConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LangTecherApi", Version = "v1" });
                //c.ExampleFilters();

                c.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request", false); // adds any string you like to the request headers - in this case, a correlation id
                c.OperationFilter<AddResponseHeadersFilter>(); 

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                c.IncludeXmlComments(filePath);

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>(); // Adds "(Auth)" to the summary so that you can see which endpoints have Authorization
                                                                              // or use the generic method, e.g. c.OperationFilter<AppendAuthorizeToSummaryOperationFilter<MyCustomAttribute>>();
                // add Security information to each operation for OAuth2
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

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
