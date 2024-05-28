using Microsoft.EntityFrameworkCore;

namespace Boolean.Util;

public static class MemberTools
{
    // Finds a member from the database, if one does not exist a member object is created, this DOES NOT insert the member into the database
    public static async Task<Member> FindOrCreate(DataContext db, ulong memberId, ulong serverId)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Snowflake == memberId && m.Guild.Snowflake == serverId);
        if (member != null)
            return member;
        
        member = new Member
        {
            Snowflake = memberId,
            GuildId = serverId
        };
        
        await db.Members.AddAsync(member);
        await db.SaveChangesAsync();
        return member;
    }
}

public static class SpecialChannelTools
{
    public static Task<SpecialChannel?> GetSpecialChannel(DataContext db, ulong guildId, SpecialChannelType specialChannelType)
    {
        return db.SpecialChannels.FirstOrDefaultAsync(sc =>
             sc.Guild.Snowflake == guildId && sc.Type == specialChannelType);
    }
}

public static class GuildTools
{
    public static async Task<Guild> FindOrCreate(DataContext db, ulong id)
    {
        var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Snowflake == id);
        if (guild != null)
            return guild;
        
        guild = new Guild
        {
            Snowflake = id,
        };
        
        await db.Guilds.AddAsync(guild);
        
        await db.SaveChangesAsync();
        return guild;
    }
}