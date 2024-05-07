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
    public DbSet<Member> Warnings { get; set; }
    public DbSet<SpecialChannel> SpecialChannels { get; set; }
}

[Table("servers")]
public class Server
{
    [Key]
    [Column("id")] public UInt64 Snowflake { get; set; }
}

public enum SpecialChannelType
{
    Logs,
    Starboard,
    Welcome
}

// For server channels configuration
[Table("special_channels")]
public class SpecialChannel
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
    [Column("server")] public Server Server { get; set; }
    [Column("snowflake")] public UInt64 Snowflake { get; set; }
    
    [Column("type")] public SpecialChannelType Type { get; set; } // "welcome", "starboard", etc
}

[Table("members")]
public class Member
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Column("id")] public UInt64 Id { get; set; }
    
    [Column("snowflake")] public UInt64 Snowflake { get; set; }
    
    [Column("server")] public Server Server { get; set; }
}

[Table("warnings")]
public class Warning
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
    
    [Column("member")] public Member Offender { get; set; }
    
    [Column("moderator")] public Member Moderator { get; set; }
    
    [Column("reason")] public string Reason { get; set; }
}