using Discord;

namespace Boolean;

public class Config
{
    public required string DiscordToken { get; set; }
    public required DatabaseCredentials DbCredentials { get; set; }
    
    #if DEBUG
        public required ulong TestGuildId { get; set; }
    #endif
    
    public sealed class DatabaseCredentials
    {
        public required string Host { get; set; }
        public required string Port { get; set; }
        public required string Database { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    
    public string GetConnectionString()
    {
        return
            $"Host={DbCredentials.Host}:{DbCredentials.Port};Database={DbCredentials.Database};Username={DbCredentials.Username};Password={DbCredentials.Password}";
    }
}
