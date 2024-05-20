using Boolean.Util;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public class UserInfo(DataContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.ModerateMembers | GuildPermission.Administrator)]
    [SlashCommand("whois", "Get moderator information about a user")]
    public async Task Whois(IGuildUser user)
    {
        var warningCount = await db.Warnings.CountAsync(w => w.Offender.Snowflake == user.Id);
        
        var embed = new EmbedBuilder
        {
            Title = user.Username,
            Color = EmbedColors.Normal,
            ThumbnailUrl = user.GetAvatarUrl(),
            Fields =
            [
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
                    Value = user.Nickname ?? "None",
                    IsInline = true
                },
                
                new EmbedFieldBuilder
                {
                    Name = "Joined At",
                    Value = user.JoinedAt?.ToString("dd/MM/yyyy") ?? "Could not find join date",
                    IsInline = true
                },
                
                new EmbedFieldBuilder
                {
                    Name = "Warning Count",
                    Value = warningCount,
                    IsInline = true
                }
            ]
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}