using Boolean.Util;
using Boolean.Util.Preconditions;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean.Warnings;

[RequireSpecialChannel(SpecialChannelType.Appeals)]
[DefaultMemberPermissions(GuildPermission.ModerateMembers)]
[Group("warnings", "Warn or retrieve warning info")]
public partial class Warnings(DataContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("warn", "Warn a member for breaking server rules")]
    public async Task Warn(
        IUser offender,
        string reason,
        [Summary(description: "Whether a public chat notification will be sent.")] bool silent = false)
    {
        var moderator = Context.Interaction.User;
        var dbOffender = await MemberTools.FindOrCreateMember(db, offender.Id, Context.Guild.Id);
        var dbModerator = await MemberTools.FindOrCreateMember(db, moderator.Id, Context.Guild.Id);
        
        var warning = await db.Warnings.AddAsync(new Warning
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
        
        var appealBtn = new ComponentBuilder()
            .WithButton("Appeal", $"warning_appeal_btn:{warning.Entity.Id},{offender.Username}", ButtonStyle.Success);
        await offender.SendMessageAsync(embed: embed.Build(), components: appealBtn.Build());
        
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
        var userWarnings = db.Warnings.Where(w => w.Offender.Snowflake == user.Id && w.Offender.Guild.Snowflake == Context.Guild.Id)
            .Include(w => w.Offender)
            .Include(w => w.Moderator);
        
        var embed = new EmbedBuilder
        {
            Title = "Warnings History",
            Color = EmbedColors.Normal,
        };
        
        foreach (var warning in userWarnings)
            embed.AddField(warning.Reason, $"Moderator <@{warning.Moderator.Snowflake}>");
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}