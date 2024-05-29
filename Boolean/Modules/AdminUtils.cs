using System.Runtime.InteropServices.ObjectiveC;
using Boolean.Util;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

[DefaultMemberPermissions(GuildPermission.Administrator)]
public class AdminUtils(DiscordSocketClient client) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("repeat", "Repeats the message you send into the channel")]
    public async Task Repeat(string msg)
    {
        await ReplyAsync(msg, allowedMentions: AllowedMentions.None);
        
        // Annoying we have to do this to end the interaction.
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = "Repeated your message",
            Color = EmbedColors.Normal
        }.Build(), ephemeral: true);
    }
    
    [SlashCommand("clear", "Clears a specified number of messages in a channel")]
    public async Task Clear(int messageCount)
    {
        if (messageCount is < 1 or > 500) {
            await RespondAsync(embed: new EmbedBuilder
            {
                Description = "Number of messages to delete must be between 1-500.",
                Color = EmbedColors.Fail,
            }.Build(), ephemeral: true);
            
            return;
        }
        
        if (Context.Channel is not SocketTextChannel textChannel) {
            await RespondAsync(embed: new EmbedBuilder
            {
                Description = "Can only delete messages in a text channel.",
                Color = EmbedColors.Fail,
            }.Build(), ephemeral: true);
            
            return;
        }
        
        var messages = await Context.Channel.GetMessagesAsync(messageCount).FlattenAsync();
        await textChannel.DeleteMessagesAsync(messages);
        
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = $"Deleted `{messages.Count()}` messages.",
            Color = EmbedColors.Success,
        }.Build());
    }
    
}