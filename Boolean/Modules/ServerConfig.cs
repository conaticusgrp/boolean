using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public static class ServerConfig
{
    public static Task<SpecialChannel?> GetSpecialChannel(DataContext db, ulong serverId, SpecialChannelType specialChannelType)
    {
        return db.SpecialChannels.FirstOrDefaultAsync(sc =>
             sc.Server.Snowflake == serverId && sc.Type == specialChannelType);
    }
}

// Used to set configuration options
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("set", "Set configuration options")]
public class ServerSet(DataContext db, Config config)
    : InteractionModuleBase<SocketInteractionContext>
{
    // Channel configuration, welcome messages, starboard, etc
    [SlashCommand("channel", "Marks a channel for a certain purpose")]
    public async Task ChannelSet(SpecialChannelType specialChannelType, SocketTextChannel channelTarget)
    {
        var embed = new EmbedBuilder().WithColor(Color.Green);
        
        // Ensure bot has permission to talk in the specified channel
        var permissions = channelTarget.GetPermissionOverwrite(channelTarget.Guild.CurrentUser);
        if (permissions is { ViewChannel: PermValue.Deny }) {
            embed.Description = $"I am unable to view <#{channelTarget.Id}>";
            embed.Color = Color.Red;
            await RespondAsync(embed: embed.Build(), ephemeral: true);
            return;
        }

        Server? server = await db.Servers.FindAsync(channelTarget.Guild.Id);
        if (server == null) {
            server = new Server { Snowflake = channelTarget.Guild.Id };
            await db.Servers.AddAsync(server);
        }

        SpecialChannel? specialChannel = await ServerConfig.GetSpecialChannel(db, Context.Guild.Id, specialChannelType);
        if (specialChannel != null)
            specialChannel.Snowflake = channelTarget.Id;
        else
            await db.SpecialChannels.AddAsync(new SpecialChannel
            {
                Server = server,
                Snowflake = channelTarget.Id,
                Type = specialChannelType
            });
        
        await db.SaveChangesAsync();
        // Use of ToString() is fine for now, we will want to implement a parser later when we add special channels with multiple words (for upper & lower case)
        embed.Description = $"{specialChannelType.ToString()} channel has been set to <#{channelTarget.Id}>";
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}
// /get, used to get configuration options
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("get", "Get configuration options")]
public class ServerGet(DataContext db, Config config)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("channel", "Get the current configuration for channels")]
    public async Task ChannelGet(SpecialChannelType specialChannelType)
    {
        var embed = new EmbedBuilder().WithColor(config.ColorTheme);
        
        SpecialChannel? channel = await ServerConfig.GetSpecialChannel(db, Context.Guild.Id, specialChannelType);
        string specialChannelName = specialChannelType.ToString().ToLower();
        
        if (channel != null)
            embed.Description = $"The current {specialChannelName} channel is set to <#{channel.Snowflake}>";
        else {
            embed.Description = $"There currently isn't a {specialChannelName} channel setup. To set it up use the `/set channel` command";
            embed.Color = Color.Red;
        }
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("unset", "Unset configuration options")]
public class ServerUnset(DataContext db, Config config) 
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("channel", "Unmarks a channel for a certain purpose")]
    public async Task ChannelUnset(SpecialChannelType specialChannelType)
    {
        SpecialChannel? specialChannel = await ServerConfig.GetSpecialChannel(db, Context.Guild.Id, specialChannelType);
        
        if (specialChannel != null) {
            db.SpecialChannels.Remove(specialChannel);
            await db.SaveChangesAsync();
        }
        
        var embed = new EmbedBuilder
        {
            Description = $"{specialChannelType.ToString()} channel has been unset",
            Color = Color.Green,
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}