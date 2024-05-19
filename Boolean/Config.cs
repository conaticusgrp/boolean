using Discord;

namespace Boolean;

public class Config
{
    public required string DiscordToken { get; set; }
    public required DatabaseCredentials DbCredentials { get; set; }
    
    // Discord flattens whitespace, use "\u2800" to create whitespace
    // https://www.compart.com/en/unicode/U+2800
    public static class Strings
    {
        // Might be a good idea to switch this to pagination when issue #32 is complete
        public const string HelpMsg = "__**/set channel <purpose>**__\n" +
                                                "\u2800Marks a channel for a specific purpose, such as for welcoming messages, starboard, server logs, etc.\n" +
                                                "__**/unset channel <purpose>**__\n" +
                                                "\u2800Unmarks a channel from a specific purpose\n";
        
        public const string JoinMsg = "Before we get to the good bits, may I suggest taking a look at the /help command for help with configuration? " +
                                                "Here are some I think you should definitely take a look at:\n\n" +
                                                "__**/set channel <purpose>**__\n" +
                                                "\u2800Marks a channel for a specific purpose, such as for welcoming messages, starboard, server logs, etc.\n";
    }
    
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
