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
        
        builder.Services
            .AddDbContext<DataContext>(options => options.UseNpgsql(config.GetConnectionString()));
        
        // Only start the bot outside of design time (avoids app running during dotnet ef commands)
        if (EF.IsDesignTime) {
            _serviceProvider = builder.Services.BuildServiceProvider();
            goto buildApp;
        }
        
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = false, // Prevents a funny from happening when your OS clock is out of sync
            MessageCacheSize = 100,
        });
        
        var interactionService = new InteractionService(_client.Rest);
        
        builder.Services
            .AddSingleton(interactionService)
            .AddSingleton(_client)
            .AddSingleton(config)
            .AddSingleton<EventHandlers>();
        
        _serviceProvider = builder.Services.BuildServiceProvider();
        
        await _client.LoginAsync(TokenType.Bot, config.DiscordToken);
        await _client.StartAsync();
        
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        AttachEventHandlers();
        
        buildApp:
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
        _client.ButtonExecuted += eventHandlers.ButtonExecuted;
        _client.ReactionAdded += eventHandlers.ReactionAdded;
        _client.ReactionRemoved += eventHandlers.ReactionRemoved;
        _client.UserJoined += eventHandlers.UserJoined;
    }
}