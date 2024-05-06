using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Server> Servers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<SpecialChannel> SpecialChannels { get; set; }
}

[Table("servers")]
public class Server
{
    [Key]
    [Column("id")] public UInt64 Snowflake { get; set; }
}

// For server channels configuration
[Table("special_channels")]
public class SpecialChannel
{
    [Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long Id { get; set; }
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

