using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Server> Servers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Channel> Channels { get; set; }
}

[Table("servers")]
public class Server
{
    [Key]
    [Column("id")] public UInt64 Id { get; set; }
}
// Channels configuration
[Table("channels")]
public class Channel
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; } // must be long for bigint type, UInt64 does not support Identity generation
    [Column("server")] public Server Server { get; set; }
    [Column("snowflake")] public UInt64 Snowflake { get; set; }

    [Column("purpose")] public string Purpose { get; set; } // "welcome", "starboard", etc 
}

[Table("members")]
public class Member
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Column("id")] public UInt64 Id { get; set; }
    [Column("snowflake")] public UInt64 Snowflake { get; set; }
    
    [Column("server")] public Server Server { get; set; }
    
    [Column("administrator")] public bool Administrator { get; set; }
}

