using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Boolean;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Guild> Guilds { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<SpecialChannel> SpecialChannels { get; set; }
    
    public DbSet<StarReaction> StarReactions { get; set; }
}

public class Guild
{
    [Key] public ulong Snowflake { get; set; }
    
    public ICollection<Member> Members { get; } = new List<Member>();
    
    public ICollection<SpecialChannel> SpecialChannels { get; } = new List<SpecialChannel>();
    
    public ICollection<StarReaction> StarReactions { get; } = new List<StarReaction>();
    
    public uint? StarboardThreshold { get; set; }
    
    public ulong? JoinRoleSnowflake { get; set; }
}

public enum SpecialChannelType
{
    Logs,
    Starboard,
    Welcome,
    Appeals,
}

// For server channels configuration
public class SpecialChannel
{
    // Id type should always be a different type to Discord.NETs snowflakes (ulong) so that we don't get confused between "Id" and "Snowflake"
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;
    
    public ulong Snowflake { get; set; }
    
    public SpecialChannelType Type { get; set; } // "welcome", "starboard", etc
}

public class Member
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public ulong Snowflake { get; set; }
    
    public ulong GuildId { get; set; }
    public Guild Guild { get; set; } = null!;
}

public class Warning
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
    
    public Member Offender { get; set; }
    
    public Member Moderator { get; set; }
    
    public string Reason { get; set; }
    
    public bool HasAppealed { get; set; }
}

// In future we should set expiry dates for these to be removed from the database
public class StarReaction
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
    public Guild Guild { get; set; }
    
    public ulong GuildId { get; set; }
    
    public bool IsOnStarboard { get; set; }
    
    public uint ReactionCount { get; set; } = 1;
    
    public ulong MessageSnowflake { get; set; }
}