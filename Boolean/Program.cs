using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Boolean;

class Program
{
    private static DbContext _dbContext;
    
    private static IServiceProvider _serviceProvider;
    private static DiscordSocketClient _client;
    private static BotConfig _config;
    private static InteractionService _interactionService;

    static IServiceProvider CreateServices()
    {
        _client = new DiscordSocketClient();
        _interactionService = new InteractionService(_client.Rest);

        var collection = new ServiceCollection()
            .AddSingleton(_interactionService)
            .AddSingleton(_client)
            .AddSingleton(_config)
            .AddSingleton(_dbContext)
            .AddSingleton<DiscordSocketConfig>()
            .AddSingleton<EventHandlers>();
        
        return collection.BuildServiceProvider();
    }
    
    public static async Task Main()
    {
        // Load config
        _config = new BotConfig();
        
        // Set up Postgres
        _dbContext = new DbContext(_config);
        await _dbContext.Database.EnsureCreatedAsync();
        
        // Set up Discord.net
        _serviceProvider = CreateServices();
        
        AttachEventHandlers();
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
    
    private static void AttachEventHandlers()
    {
        var eventHandlers = _serviceProvider.GetRequiredService<EventHandlers>();
        
        _client.Log += eventHandlers.LogMessage;
        _client.Ready += eventHandlers.Ready;
        _client.InteractionCreated += eventHandlers.InteractionCreated;
    }
}