using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Boolean;

class Program
{
    private static ServiceProvider _serviceProvider;
    private static DiscordSocketClient _client;
    
    public static async Task Main()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        Config config = builder.Configuration.Get<Config>()
                         ?? throw new Exception("Failed to load valid config from appsettings.json, please refer to the README.md for instructions.");
        
        _client = new DiscordSocketClient();
        
        await _client.LoginAsync(TokenType.Bot, config.DiscordToken);
        await _client.StartAsync();
        
        var interactionService = new InteractionService(_client.Rest);
        
        _serviceProvider = builder.Services
            .AddSingleton(interactionService)
            .AddSingleton(_client)
            .AddSingleton(config)
            .AddSingleton<DiscordSocketConfig>()
            .AddSingleton<EventHandlers>()
            .AddDbContext<DataContext>(options => options.UseNpgsql(config.GetConnectionString()))
            .BuildServiceProvider();
        
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        AttachEventHandlers();
        
        IHost app = builder.Build();
        await app.RunAsync();
    }
    
    private static void AttachEventHandlers()
    {
        var eventHandlers = _serviceProvider.GetRequiredService<EventHandlers>();
        
        _client.Log += eventHandlers.LogMessage;
        _client.Ready += eventHandlers.Ready;
        _client.InteractionCreated += eventHandlers.InteractionCreated;
    }
}