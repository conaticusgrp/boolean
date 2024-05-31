using System.Diagnostics;
using System.Net.NetworkInformation;
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

    // We might later want to consider making this maintainers only (as people could use this to exploit the bot)
    [SlashCommand("status", "Shows the bot's compute usage (CPU, RAM, etc)")]
    public async Task Status()
    {
        var botProcess = Process.GetCurrentProcess();
        var ramUsageGb = botProcess.WorkingSet64 / (float) Math.Pow(1024, 3);

        // Calculate the CPU usage
        var startTime = DateTime.UtcNow;
        var startCpuUsage = botProcess.TotalProcessorTime;

        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        float startBytesSent = 0;
        float startBytesReceived = 0;

        foreach (NetworkInterface networkInterface in networkInterfaces) {
            startBytesSent += networkInterface.GetIPv4Statistics().BytesSent;
            startBytesReceived += networkInterface.GetIPv4Statistics().BytesReceived;
        }

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = botProcess.TotalProcessorTime;

        var cpuUsage = (float) (endCpuUsage - startCpuUsage).TotalMilliseconds
                       / (float) (Environment.ProcessorCount * (endTime - startTime).TotalMilliseconds);

        float endBytesSent = 0;
        float endBytesReceived = 0;

        foreach (NetworkInterface networkInterface in networkInterfaces) {
            endBytesSent += networkInterface.GetIPv4Statistics().BytesSent;
            endBytesReceived += networkInterface.GetIPv4Statistics().BytesReceived;
        }

        float megabytesSentPerSecond = ((endBytesSent - startBytesSent) / (float) Math.Pow(1024, 2)) 
                                        / (float) (endTime - startTime).TotalSeconds;

        float megabytesReceivedPerSecond = ((endBytesReceived - startBytesReceived) / (float) Math.Pow(1024, 2)) 
                                        / (float) (endTime - startTime).TotalSeconds;

        var embed = new EmbedBuilder
        {
            Title = "Bot Status",
            Color = EmbedColors.Normal,
        };
        
        var uptime = DateTime.Now - botProcess.StartTime;

        embed
            .AddField("RAM", $"`{Math.Round(ramUsageGb, 2)} GB`", true)
            .AddField("CPU Usage", $"`{Math.Round(cpuUsage * 100, 2)}%`", true)
            .AddField("** **", "** **") // Empty line to display the following fields in a separate row
            .AddField("MB/S Sent", $"`{Math.Round(megabytesSentPerSecond, 2)} MB/S`", true)
            .AddField("MB/S Received", $"`{Math.Round(megabytesReceivedPerSecond, 2)} MB/S`", true)
            .AddField("Up Time", $"{uptime.Days} days, {uptime.Hours} hours, {uptime.Minutes} minutes, {uptime.Seconds} seconds", false);
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}