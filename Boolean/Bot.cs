using Discord;
using Discord.WebSocket;

namespace Boolean;

public static class Bot
{
    private static DiscordSocketClient _client;
    private static BotConfig _config;
    
    public static async Task Start()
    {
        _client = new DiscordSocketClient();
        _config = new BotConfig();
        
        AttachEventHandlers();

        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private static void AttachEventHandlers()
    {
        _client.Log += EventHandlers.LogMessage;
    }
}