using Boolean.Util;
using Boolean.Util.Preconditions;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

[RequireSpecialChannel(SpecialChannelType.Appeals)]
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
    
    [ComponentInteraction("warning_appeal_btn:*,*", true)]
    public async Task AppealWarningModal(long warningId, string offenderName)
    {
        // Not a fan of doing multiple queries, if there is a better way to check if appealed please feel free to change
        var warning = await db.Warnings.FirstAsync(w => w.Id == warningId);
        
        if (!warning.HasAppealed) {
            await Context.Interaction.RespondWithModalAsync<AppealModal>($"warning_appeal_modal:{warningId},{offenderName}");
            return;
        }
        
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = "You have already appealed this warning",
            Color = EmbedColors.Fail,
        }.Build(), ephemeral: true);
    }
    
    [ModalInteraction("warning_appeal_modal:*,*", true)]
    public async Task AppealWarningMessage(long warningId, string offenderName, AppealModal modal)
    {
        var warning = await db.Warnings
            .Include(w => w.Offender.Server)
            .Include(w => w.Moderator)
            .FirstAsync(w => w.Id == warningId);
        
        var embed = new EmbedBuilder
        {
            Title = $"{offenderName}'s warning appeal",
            Fields = [
                new EmbedFieldBuilder
                {
                    Name = "Warning Reason",
                    Value = warning.Reason,
                    IsInline = true,
                },
                
                new EmbedFieldBuilder
                {
                    Name = "Offender",
                    Value = $"<@{warning.Offender.Snowflake}>",
                    IsInline = true,
                },
                
                new EmbedFieldBuilder
                {
                    Name = "Moderator",
                    Value = $"<@{warning.Moderator.Snowflake}>",
                    IsInline = true,
                },
                
                new EmbedFieldBuilder
                {
                    Name = "User Appeal",
                    Value = modal.UserAppeal,
                    IsInline = false,
                }
            ],
            Color = EmbedColors.Normal,
        };
        
        var acceptBtn = new ComponentBuilder()
            .WithButton("Accept", $"warning_appeal_accept_btn:{warningId}", ButtonStyle.Success);
        
        var dbAppealsChannel = await db.SpecialChannels.FirstAsync(s =>
            s.Server.Snowflake == warning.Offender.Server.Snowflake && s.Type == SpecialChannelType.Appeals);
        
        var appealsChannel = await Context.Client.GetChannelAsync(dbAppealsChannel.Snowflake);
        if (appealsChannel is IMessageChannel msgChannel)
            await msgChannel.SendMessageAsync(embed: embed.Build(), components: acceptBtn.Build());
        
        warning.HasAppealed = true;
        await db.SaveChangesAsync();
        
        await RespondAsync(embed: new EmbedBuilder
        {
            Title = "Appeal Submitted",
            Description = modal.UserAppeal,
            Color = EmbedColors.Normal,
        }.Build());
    }
    
    [RequireButtonPermission(GuildPermission.Administrator)]
    [ComponentInteraction("warning_appeal_accept_btn:*", true)]
    public async Task AcceptWarningModal(string warningId) =>
        await Context.Interaction.RespondWithModalAsync<AcceptModal>($"warning_appeal_accept_modal:{warningId}");
    
    [ModalInteraction("warning_appeal_accept_modal:*", true)]
    public async Task AcceptWarningAppealMessage(long warningId, AcceptModal modal)
    {
        var warning = await db.Warnings
            .Include(w => w.Offender)
            .FirstAsync(w => w.Id == warningId);
        
        db.Warnings.Remove(warning);
        await db.SaveChangesAsync();
        
        var embed = new EmbedBuilder
        {
            Title = "Warning Appeal Accepted",
            Fields = [
                new EmbedFieldBuilder
                {
                    Name = "Warning Reason",
                    Value = warning.Reason,
                },
                new EmbedFieldBuilder
                {
                    Name = "Accept Reason",
                    Value = modal.AcceptReason,
                },
            ],
            Color = EmbedColors.Success,
        };
        
        var offender = Context.Client.GetUser(warning.Offender.Snowflake);
        await offender.SendMessageAsync(embed: embed.Build());
        
        
        await RespondAsync(embed: new EmbedBuilder
        {
            Title = "Warning Appeal Accepted",
            Description = modal.AcceptReason,
            Color = EmbedColors.Success,
        }.Build());
    }
    
    [SlashCommand("history", "Warning history a server member")]
    public async Task History(IUser user)
    {
        var userWarnings = db.Warnings.Where(w => w.Offender.Snowflake == user.Id && w.Offender.Server.Snowflake == Context.Guild.Id)
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