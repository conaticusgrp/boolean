using Discord;

namespace Boolean;

public class Config
{
    public required string DiscordToken { get; set; }
    public required DatabaseCredentials DbCredentials { get; set; }
    
    public Color BotTheme = new(255, 123, 0);
    
    #if DEBUG
        public required ulong TestGuildId { get; set; }
    #endif
    
    public sealed class DatabaseCredentials
    {
        public required string Database { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    
    public string GetConnectionString()
    {
        return
            $"Host=localhost:5432;Database={DbCredentials.Database};Username={DbCredentials.Username};Password={DbCredentials.Password}";
    }
}