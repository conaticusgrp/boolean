using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boolean.Util.Preconditions;

// We could handle special channel deletions and send an error if so, however this should be down to the competence of the administrators.
// It's a bit icky fetching the channel twice, but it should be cached by EF - if preconditions has a way to save this in the class that would be nice
public class RequireSpecialChannelAttribute(SpecialChannelType type) : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var db = services.GetRequiredService<DataContext>();
        
        if (context.Guild == null)
            return PreconditionResult.FromSuccess();
        
        var appealsChannel = await db.SpecialChannels.FirstOrDefaultAsync(s =>
            s.Guild.Snowflake == context.Guild.Id && s.Type == type);
        
        if (appealsChannel != null)
            return PreconditionResult.FromSuccess();
        
        var embed = new EmbedBuilder
        {
            Description = "You need to set up the appeals channel with `/set channel` to use the warnings commands.",
            Color = EmbedColors.Fail,
        };
            
        await context.Interaction.RespondAsync(embed: embed.Build());
        return PreconditionResult.FromError("No appeals channel provided");
    }
}

public class RequireButtonPermission(GuildPermission permission) : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        if (context.User is SocketGuildUser guildUser && guildUser.GuildPermissions.Has(permission))
            return PreconditionResult.FromSuccess();
        
        var embed = new EmbedBuilder
        {
            Title = "Permission Denied",
            Description = "Only administrators can press this button",
            Color = EmbedColors.Fail,
        };
        
        await context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);
        return PreconditionResult.FromError("User does not have permission to use button.");
    }
}