using Boolean.Util;
using Discord;
using Discord.Interactions;

namespace Boolean;

public class FunUtils : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("execute", "Executes a fiendish individual.")]
    public async Task Execute(IUser user, string? reason = null)
    {
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = $"{user.Mention} has been executed by {Context.User.Mention}"
                          + (reason != null ? $"\nReason: `{reason}`" : ""),
            Color = EmbedColors.Normal,
        }.Build());
    }
}