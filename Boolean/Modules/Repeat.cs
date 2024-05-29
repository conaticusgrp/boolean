using System.Runtime.InteropServices.ObjectiveC;
using Boolean.Util;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

public class Repeat(DiscordSocketClient client) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("repeat", "Repeats the message you send into the channel")]
    public async Task RepeatMsg(string msg)
    {
        await ReplyAsync(msg, allowedMentions: AllowedMentions.None);
        
        // Annoying we have to do this to end the interaction.
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = "Repeated your message",
            Color = EmbedColors.Normal
        }.Build(), ephemeral: true);
    }
}