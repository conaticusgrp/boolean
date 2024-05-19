using Boolean.Util;
using Boolean.Util.Preconditions;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean.Warnings;

public partial class Warnings
{
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
        
        var dbAppealsChannel = await db.SpecialChannels.FirstAsync(s =>
            s.Server.Snowflake == warning.Offender.Server.Snowflake && s.Type == SpecialChannelType.Appeals);
        
        var appealsChannel = await Context.Client.GetChannelAsync(dbAppealsChannel.Snowflake);
        if (appealsChannel is IMessageChannel msgChannel) {
            var acceptBtn = new ComponentBuilder()
                .WithButton("Accept", $"warning_appeal_accept_btn:{warningId}", ButtonStyle.Success);
            
            await msgChannel.SendMessageAsync(embed: embed.Build(), components: acceptBtn.Build());
        }
        
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
}