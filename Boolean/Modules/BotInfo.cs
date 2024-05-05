using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

public class BotInfo(DiscordSocketClient client, Config config) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("guilds", "Gets the number of servers the bot is in.")]
    public async Task Guilds()
    {
        var embed = new EmbedBuilder
        {
            Description = $"I am currently in **{client.Guilds.Count}** guilds."
        }.WithColor(config.BotTheme);
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
    
    [SlashCommand("ping", "Gets the current client latency.")]
    public async Task Ping()
    {
        var embed = new EmbedBuilder
        {
            Description = $"Pong. Took **{client.Latency}ms** to respond."
        }.WithColor(config.BotTheme);
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}