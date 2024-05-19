using Discord;

namespace Boolean;

public class Config
{
    public required string DiscordToken { get; set; }
    public required DatabaseCredentials DbCredentials { get; set; }
    
    // Discord flattens whitespace, use "\u2800" to create whitespace
    // https://www.compart.com/en/unicode/U+2800
    public class Strings
    {   // might be a good idea to switch this to a Pageination when issue #32 is complete 
        public static string  HelpMsg = "__**/set channel <purpose>**__\n" +
                                 "\u2800Marks a channel for a specific purpose, such as for welcoming messages, starboard, server logs, etc\n" +
                                 "__**/unset channel <purpose>**__\n" +
                                 "\u2800Unmarks a channel from a specific purpose\n"; // send by /help
        // Written by dyslexic Kventis, might be an idea to rewrite before merge
        public static string JoinMsg = "Before we get to the good bits, may I suggest taking a look at the /help command for help with configuration? " +
                                       "Here are some I think you should definitely take a look at.\n\n" +
                                       "__**/set channel <purpose>**__\n" +
                                       "\u2800Marks a channel for a specific purpose, such as for welcoming messages, starboard, server logs, etc\n" +
                                       ""; // this message is sent when Boolean joins a guild
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
