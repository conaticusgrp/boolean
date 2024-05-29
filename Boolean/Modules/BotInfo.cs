using System.Diagnostics;
using Boolean.Util;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

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

    // Later we might want to add extensions for users and admins (e.g /help setup or just /help)
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("help", "Explains what can be configured in Boolean")]
    public async Task Help()
    {
        var embed = new EmbedBuilder
        {
            Title = "Configuration Help",
            Description = Config.Strings.HelpMsg,
            Color = EmbedColors.Normal,
        };
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
    
    [SlashCommand("contribute", "Information on how to contribute to the discord bot.")]
    public async Task Contribute()
    {
        var embed = new EmbedBuilder
        {
            Title = "Contributing to Boolean",
            Description = Config.Strings.ContributeMsg,
            Color = EmbedColors.Normal
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("status", "Shows the bot's compute usage (CPU, RAM, ...)")]
    public async Task Status()
    {
        
        Process botProcess = Process.GetCurrentProcess();
        float ramUsedInGigabytes = botProcess.WorkingSet64 / (float) Math.Pow(1024, 3);
        TimeSpan totalProcessorTime = botProcess.TotalProcessorTime;

        // Calculate the CPU usage
        var startTime = DateTime.UtcNow;
        var startCpuUsage = botProcess.TotalProcessorTime;

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = botProcess.TotalProcessorTime;

        float cpuUsage = (float) (endCpuUsage - startCpuUsage).TotalMilliseconds / (float) (Environment.ProcessorCount * (endTime - startTime).TotalMilliseconds);

        var embed = new EmbedBuilder
        {
            Title = "Bot Status",
            Color = EmbedColors.Normal,
            Description = "The current bot compute status",
        };

        embed
            .AddField("RAM", ramUsedInGigabytes)
            .AddField("CPU Usage", $"{cpuUsage * 100}%")
            .AddField("Total Processor Time", totalProcessorTime);
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}