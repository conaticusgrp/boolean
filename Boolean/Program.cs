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
        // Dependency injection & application setup
        _client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            UseInteractionSnowflakeDate = false // Prevents a funny from happening when your OS clock is out of sync
        });
        
        var interactionService = new InteractionService(_client.Rest);
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();
        Config config = builder.Configuration.Get<Config>()
                         ?? throw new Exception("Failed to load valid config from appsettings.json, please refer to the README.md for instructions.");
        
        _serviceProvider = builder.Services
            .AddDbContext<DataContext>(options => options.UseNpgsql(config.GetConnectionString()))
            .AddSingleton(interactionService)
            .AddSingleton(_client)
            .AddSingleton(config)
            .AddSingleton<EventHandlers>()
            .BuildServiceProvider();
        
        // Start the bot
        await _client.LoginAsync(TokenType.Bot, config.DiscordToken);
        await _client.StartAsync();
        
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
        _client.JoinedGuild += eventHandlers.GuildCreate;
    }
}