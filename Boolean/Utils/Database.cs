using Microsoft.EntityFrameworkCore;

namespace Boolean.Util;

public static class MemberTools
{
    // Finds a member from the database, if one does not exist a member object is created, this DOES NOT insert the member into the database
    public static async Task<Member> FindOrCreateMember(DataContext db, ulong memberId, ulong serverId)
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
