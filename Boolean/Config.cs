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
        
        public const string ContributeMsg = "Hello and thank you for your interest in contributing to Boolean!\n\n" +
                                            "To get started, please visit our open source [repository](https://github.com/conaticusgrp/boolean) and check out our [contributing guide](https://github.com/conaticusgrp/boolean/blob/main/CONTRIBUTING.md). " +
                                            "Once completed, you may review our [issues](https://github.com/conaticusgrp/boolean/issues) and choose whichever task you are interested in.";
        
        public static string WelcomeMsg(string userDisplay, string serverName) =>
            $"Hello {userDisplay}, thank you for joining the **{serverName}** server!";
    }
    
    // Each represents the unicode for that emoji
    // This should be compared with IEmote.Name
    public static class Emojis
    {
        public const string Star = "\u2b50";
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
