using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

public class UserInfo(DiscordSocketClient client, Config config) : InteractionModuleBase<SocketInteractionContext>
{

    [DefaultMemberPermissions(GuildPermission.ModerateMembers | GuildPermission.Administrator)]
    [SlashCommand("whois", "Get information about a user.")]
    public async Task Whois(IUser user)
    {
        user ??= Context.User;

        var guildMember = Context.Guild.GetUser(user.Id);

        var embed = new EmbedBuilder
        {
            Title = $"**Whois:** {user.Username}",
            ThumbnailUrl = user.GetAvatarUrl(),
            Fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Username",
                    Value = user.Username,
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Created At",
                    Value = user.CreatedAt.ToString("dd/MM/yyyy"),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Server Nickname",
                    Value = guildMember?.Nickname ?? "None",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Joined At",
                    Value = guildMember?.JoinedAt?.ToString("dd/MM/yyyy") ?? "Could not find join date",
                    IsInline = true
                },
            }
        }.WithColor(config.BotTheme);
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}