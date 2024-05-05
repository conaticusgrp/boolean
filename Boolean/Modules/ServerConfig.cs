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

            string updatedChan = UpdateChannel(type, server, channel.Id);
            
            await db.SaveChangesAsync();
            // might wanna make this sound less "robotic"
            embed.Description = $"{updatedChan} channel has been set to <#{channel.Id}>.";
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

            Server? server = db.Servers.Find(Context.Guild.Id);
            if (server == null) {
                embed.Description = "This server does not have any configurations yet";
                await RespondAsync(embed: embed.Build(), ephemeral: true);
                return;
            }

            string chan = "";
            ulong? chanid = 0;
            switch (type) {
                case ChannelTypes.Logs:
                    chan = "log";
                    chanid = server.LogChannel;
                    break;
                case ChannelTypes.Starboard:
                    chan = "starboard";
                    chanid = server.Starboard;
                    break;
                case ChannelTypes.Welcome:
                    chan = "welcome";
                    chanid = server.Welcome;
                    break;
            }
            embed.Description = chanid == null ? $"There currently isn't a {chan} channel setup"
                    : $"The current {chan} channel is set to <#{chanid}>";
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
            
            string updatedChan = UpdateChannel(type, server, null);
            
            await db.SaveChangesAsync();
            embed.Description = $"{updatedChan} channel has been unset.";
            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
    public static string UpdateChannel(ChannelTypes type, Server server, ulong? value)
    {
        switch (type) {
            case ChannelTypes.Logs:
                server.LogChannel = value;
                return "log";
            case ChannelTypes.Starboard:
                server.Starboard = value;
                return "starboard";
            case ChannelTypes.Welcome:
                server.Welcome = value;
                return "welcome";
        }

        return "";
    }
}