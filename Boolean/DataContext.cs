using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Boolean;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Server> Servers { get; set; }
    public DbSet<Member> Members { get; set; }
}

[Table("servers")]
public class Server
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Column("id")] public UInt64 Id { get; set; } // not sure why this is here?
    [Column("snowflake")] public UInt64 Snowflake { get; set; }
    [Column("logchannel")] public UInt64? LogChannel { get; set; }
    [Column("starboard")] public UInt64? Starboard { get; set; }
    [Column("welcome")] public UInt64? Welcome { get; set; }

    
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

