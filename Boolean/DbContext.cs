using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Boolean;

public class DbContext(BotConfig config) : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Server> Servers { get; set; }
    public DbSet<Member> Members { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql($"Host=localhost:5432;Database={config.Database};Username={config.Username};Password={config.Password}");
}

[Table("servers")]
public class Server
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Column("id")] public UInt64 Id { get; set; }
    [Column("snowflake")] public UInt64 Snowflake { get; set; }
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

