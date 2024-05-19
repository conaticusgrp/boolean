using Boolean.Util;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Boolean;

public class BotInfo(DiscordSocketClient client) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("guilds", "Gets the number of servers the bot is in")]
    public async Task Guilds()
    {
        var embed = new EmbedBuilder
        {
            Color = EmbedColors.Normal,
            Description = $"I am currently in **{client.Guilds.Count}** guilds"
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
    
    [SlashCommand("ping", "Gets the current client latency")]
    public async Task Ping()
    {
        var embed = new EmbedBuilder
        {
            Color = EmbedColors.Normal,
            Description = $"Pong. Took **{client.Latency}ms** to respond"
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }

    [SlashCommand("help", "Explains what can be configured in Boolean")]
    public async Task Help()
    {
        var embed = new EmbedBuilder
        {
            Title = "Configuration help",
            Description = Config.Strings.HelpMsg,
            Color = EmbedColors.Normal,
        };
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}