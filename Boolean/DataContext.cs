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
    
    public DbSet<Server> Servers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<SpecialChannel> SpecialChannels { get; set; }
}

public class Server
{
    [Key] public ulong Snowflake { get; set; }
    public ICollection<Member> Members { get; } = new List<Member>();
}

public enum SpecialChannelType
{
    Logs,
    Starboard,
    Welcome
}

// For server channels configuration
public class SpecialChannel
{
    // Id type should always be a different type to Discord.NETs snowflakes (ulong) so that we don't get confused between "Id" and "Snowflake"
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public Server Server { get; set; }
    
    public ulong Snowflake { get; set; }
    
    public SpecialChannelType Type { get; set; } // "welcome", "starboard", etc
}

public class Member
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public ulong Snowflake { get; set; }
    
    public ulong ServerId { get; set; }
    public Server Server { get; set; } = null!;
}

public class Warning
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
    
    public Member Offender { get; set; }
    
    public Member Moderator { get; set; }
    
    public string Reason { get; set; }
}
