using Boolean.Util;
using Boolean.Util.Preconditions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public partial class GuildSet
{
    [RequireGuild]
    [SlashCommand("joinrole", "Sets role members are assigned when joining the server")]
    public async Task JoinRoleSet(SocketRole joinRole)
    {
        var botHierarchy = Context.Guild.CurrentUser.Hierarchy;
        if (joinRole.Position >= botHierarchy) {
            await RespondAsync(embed: new EmbedBuilder
            {
                Description = $"I do not have permission to assign that role. Please ensure my role is above {joinRole.Mention} in the roles list.",
                Color = EmbedColors.Fail,
            }.Build(), ephemeral: true);
            return;
        }
        
        if (joinRole.IsEveryone) {
            await RespondAsync(embed: new EmbedBuilder
            {
                Description = "Cannot assign join role to the @everyone role because this is a special role.",
                Color = EmbedColors.Fail,
            }.Build(), ephemeral: true);
            return;
        }
        
        var guild = await db.Guilds.FirstAsync(g => g.Snowflake == Context.Guild.Id);
        guild.JoinRoleSnowflake = joinRole.Id;
        await db.SaveChangesAsync();
        
        await RespondAsync(embed: new EmbedBuilder 
        {
            Description = $"Join role has been set to {joinRole.Mention}",
            Color = EmbedColors.Success,
        }.Build(), ephemeral: true);
    }
}

public partial class GuildGet
{
    [RequireGuild]
    [SlashCommand("joinrole", "Gets the role members are assigned when joining the server")]
    public async Task JoinRoleGet()
    {
        var guild = await db.Guilds.FirstAsync(s => s.Snowflake == Context.Guild.Id);
        if (guild.JoinRoleSnowflake == null) {
            await RespondAsync(embed: new EmbedBuilder 
            {
                Description = $"There is currently no join role set up. You can set one up with the `/set joinrole` command.",
                Color = EmbedColors.Fail,
            }.Build(), ephemeral: true);
            return;
        }
        
        await RespondAsync(embed: new EmbedBuilder 
        {
            Description = $"The current join role is set to <@&{guild.JoinRoleSnowflake}>",
            Color = EmbedColors.Normal,
        }.Build(), ephemeral: true);
    }
}

public partial class GuildUnset
{
    [RequireGuild]
    [SlashCommand("joinrole", "Unsets the role members are assigned when joining the server")]
    public async Task JoinRoleUnset()
    {
        var guild = await db.Guilds.FirstAsync(s => s.Snowflake == Context.Guild.Id);
        db.Guilds.Remove(guild);
        await db.SaveChangesAsync();
        
        await RespondAsync(embed: new EmbedBuilder 
        {
            Description = "Join role has been unset",
            Color = EmbedColors.Success,
        }.Build(), ephemeral: true);
    }
}