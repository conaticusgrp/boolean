using Boolean.Util;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean;
        

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
        // User DM
        var embed = new EmbedBuilder().WithColor(EmbedColors.Fail);
        var moderator = Context.Interaction.User;
        
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
}