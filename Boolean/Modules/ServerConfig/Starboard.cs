using Boolean.Util;
using Boolean.Util.Preconditions;
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

[RequireGuild]
public partial class GuildSet
{
    [SlashCommand("starboard-threshold",
        "Set the number of star reactions a message needs to be eligible for the starboard")]
    public async Task ThresholdSet(int threshold)
    {
        EmbedBuilder embed;
        
        if (threshold is < 2 or > 500) {
            embed = new EmbedBuilder
            {
                Description = "Starboard threshold must be 2-500",
                Color = EmbedColors.Fail,
            };
            
            await RespondAsync(embed: embed.Build(), ephemeral: true);
            return;
        }
        
        var guild = await db.Guilds.FirstAsync(g => g.Snowflake == Context.Guild.Id);
        guild.StarboardThreshold = (uint)threshold;
        await db.SaveChangesAsync();
        
        embed = new EmbedBuilder
        {
            Description = $"Successfully set starboard threshold to {threshold} stars",
            Color = EmbedColors.Success,
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}

public partial class GuildGet
{
    [SlashCommand("starboard-threshold",
        "Get the number of star reactions a message needs to be eligible for the starboard")]
    public async Task ThresholdSet()
    {
        EmbedBuilder embed;
        
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Snowflake == Context.Guild.Id);
        if (guild is not { StarboardThreshold: not null }) {
            embed = new EmbedBuilder
            {
                Description = "No starboard threshold has been set",
                Color = EmbedColors.Fail,
            };
            
            await RespondAsync(embed: embed.Build(), ephemeral: true);
            return;
        }
        
        embed = new EmbedBuilder
        {
            Description = $"The current starboard threshold is {guild.StarboardThreshold}",
            Color = EmbedColors.Normal,
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}

public partial class GuildUnset
{
    [SlashCommand("starboard-threshold",
        "Get the number of star reactions a message needs to be eligible for the starboard")]
    public async Task ThresholdUnset()
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Snowflake == Context.Guild.Id);
        
        if (guild is { StarboardThreshold: not null }) {
            guild.StarboardThreshold = null;
            await db.SaveChangesAsync();
        }
        
        var embed = new EmbedBuilder
        {
            Description = $"Starboard threshold has been unset",
            Color = EmbedColors.Success,
        };
        
        await RespondAsync(embed: embed.Build(), ephemeral: true);
    }
}