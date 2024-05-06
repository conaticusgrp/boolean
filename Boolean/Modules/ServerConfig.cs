using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

public enum ChannelTypes
{
    Logs,
    // [ChoiceDisplay("Server Logs")]
    // ServerLogs,
    Starboard,
    Welcome
}
public static class ChannelTypesString
{
    public static string AsString(this ChannelTypes enumValue)
    {
        switch (enumValue)
        {
            case ChannelTypes.Logs:
                return "logs";
            case ChannelTypes.Starboard:
                return "starboard";
            case ChannelTypes.Welcome:
                return "welcome";
        }

        return "";
    }
}

public class ServerConfig
{
    // /set, used to set configuration options
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [Group("set", "Set configuration options")]
    public class ServerSet(DataContext db, Config config)
        : InteractionModuleBase<SocketInteractionContext>
    {
        // channel configuration, welcome messages, starboard, etc 
        [SlashCommand("channel", "Marks a channel for a certain purpose")]
        public async Task ChannelSet(ChannelTypes type, SocketTextChannel channel)
        {
            var embed = new EmbedBuilder().WithColor(config.BotTheme);
            
            var permissions = channel.GetPermissionOverwrite(channel.Guild.CurrentUser);
            if (permissions.HasValue && permissions.Value.SendMessages == PermValue.Deny) {
                embed.Description = $"I am unable to send messages in <#{channel.Id}>.";
                await RespondAsync(embed: embed.Build(), ephemeral: true);
                return;
            }

            ulong guildId = channel.Guild.Id;            
            Server? server = db.Servers.Find(guildId);
            if (server == null) {
                // Unsure why ID is here, for now snowflake and ID are the same 
                server = new Server { Id = guildId, Snowflake = guildId};
                db.Servers.Add(server);
            }

            UpdateChannel(type, server, channel.Id, db);
            
            await db.SaveChangesAsync();
            // might wanna make this sound less "robotic"
            embed.Description = $"{type.AsString()} channel has been set to <#{channel.Id}>.";
            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
    // /get, used to get configuration options
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [Group("get", "Get configuration options")]
    public class ServerGet(DataContext db, Config config)
        : InteractionModuleBase<SocketInteractionContext>
    {
        // This description sucks
        [SlashCommand("channel", "Get the current configuration for channels")]
        public async Task ChannelGet(ChannelTypes type)
        {
            var embed = new EmbedBuilder().WithColor(config.BotTheme);

            Channel? channel = db.Channels.FirstOrDefault(e => e.Server.Id == Context.Guild.Id);
            if (channel == null) {
                embed.Description = "This server does not have any configurations yet";
                await RespondAsync(embed: embed.Build(), ephemeral: true);
                return;
            }
            
            channel = db.Channels.FirstOrDefault(e => e.Server.Id == Context.Guild.Id && e.Purpose == type.AsString());
            
            embed.Description = channel == null ? $"There currently isn't a {type.AsString()} channel setup"
                    : $"The current {type.AsString()} channel is set to <#{channel.Snowflake}>";
            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
    // /unset, used to remove configuration options
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [Group("unset", "Unset configuration options")]
    public class ServerUnset(DataContext db, Config config) 
        : InteractionModuleBase<SocketInteractionContext>
    {
        // undoes channel configuration
        [SlashCommand("channel", "Unmarks a channel for a certain purpose")]
        public async Task ChannelUnset(ChannelTypes type)
        {
            var embed = new EmbedBuilder().WithColor(config.BotTheme);

            Server? server = db.Servers.Find(Context.Guild.Id);
            if (server == null) {
                embed.Description = "This server does not have any configurations yet";
                await RespondAsync(embed: embed.Build(), ephemeral: true);
                return;
            }
            
            UpdateChannel(type, server, null, db);
            
            await db.SaveChangesAsync();
            embed.Description = $"{type.AsString()} channel has been unset.";
            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
    public static string UpdateChannel(ChannelTypes type, Server server, ulong? value, DataContext db)
    {
        Channel? channel = db.Channels.FirstOrDefault(e => e.Server.Id == server.Id && e.Purpose == type.AsString());
        if (value == null) {
            // delete it from the table
            if (channel != null) {
                db.Channels.Remove(channel);
            }
            return type.AsString();
        }
        // create or edit it
        if (channel != null) 
            channel.Snowflake = value.Value;
        else {
            channel = new Channel { Server = server, Snowflake = value.Value, Purpose = type.AsString() };
            db.Channels.Add(channel);
        }
        return type.AsString();
    }
}