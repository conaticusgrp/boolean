using Boolean.Util;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

static class MemberTools
{
    // Finds a member from the database, if one does not exist a member object is created, this DOES NOT insert the member into the database
    public static async Task<Member> FindOrCreateMember(DataContext db, ulong memberId, ulong serverId)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Snowflake == memberId && m.Server.Snowflake == serverId);
        if (member != null)
            return member;
        
        member = new Member
        {
            Snowflake = memberId,
            ServerId = serverId
        };
        
        await db.Members.AddAsync(member);
        await db.SaveChangesAsync();
        return member;
    }
}

[DefaultMemberPermissions(GuildPermission.ModerateMembers)]
[Group("warnings", "Warn or retrieve warning info")]
public class Warnings(DataContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("warn", "Warn a member for breaking server rules")]
    public async Task Warn(
        IUser offender,
        string reason,
        [Summary(description: "Whether a public chat notification will be sent.")] bool silent = false)
    {
        var moderator = Context.Interaction.User;
        
        // Database Entry
        var dbOffender = await MemberTools.FindOrCreateMember(db, offender.Id, Context.Guild.Id);
        var dbModerator = await MemberTools.FindOrCreateMember(db, moderator.Id, Context.Guild.Id);
        
        await db.Warnings.AddAsync(new Warning
        {
            Offender = dbOffender,
            Moderator = dbModerator,
            Reason = reason,
        });
        
        await db.SaveChangesAsync();
        
        // User DM
        var embed = new EmbedBuilder().WithColor(EmbedColors.Fail);
        
        embed.Title = $"You have received a warning in '{Context.Guild.Name}'";
        embed.Description = $"Reason: `{reason}`";
        if (!silent)
            embed.Description += $"\nModerator: {moderator.Mention}";
        
        await offender.SendMessageAsync(embed: embed.Build());
        
        // Command Response
        embed.Title = "Warning Issued";
        embed.Description = null;
        embed.Fields =
        [
            new EmbedFieldBuilder
            {
                Name = "Offender",
                Value = offender.Mention,
                IsInline = true,
            },
            
            new EmbedFieldBuilder
            {
            Name = "Reason",
                Value = $"`{reason}`",
                IsInline = true,
            },
        ];
        
        await RespondAsync(embed: embed.Build(), ephemeral: silent);
    }
    
    [SlashCommand("history", "Warning history a server member")]
    public async Task History(IUser user)
    {
        var userWarnings = db.Warnings.Where(w => w.Offender.Snowflake == user.Id && w.Offender.Server.Snowflake == Context.Guild.Id)
            .Include(w => w.Offender)
            .Include(w => w.Moderator);
        
        var embed = new EmbedBuilder
        {
            Color = EmbedColors.Normal,
            Title = "Warnings History",
        };
        
        foreach (var warning in userWarnings)
            embed.AddField(warning.Reason, $"Moderator <@{warning.Moderator.Snowflake}>");
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}