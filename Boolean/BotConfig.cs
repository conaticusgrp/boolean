using Discord;
using Microsoft.Extensions.Configuration;

namespace Boolean;

public class BotConfig
{
    public readonly string? Token, Database, Username, Password;
    public readonly Color BotTheme = Color.Gold;
    
    #if DEBUG
        public readonly ulong TestGuildId;
    #endif
    
    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        // Forgive me father for I have sinned.
        .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
        .AddJsonFile("secrets.json")
        .Build();
    
    public BotConfig()
    {
        Token = _config["discordToken"];
        Database = _config["dbName"];
        Username = _config["dbUsername"];
        Password = _config["dbPassword"];
        
        #if DEBUG
            string? testGuildId = _config["testGuildId"];
        
            if (testGuildId == null) {
                throw new Exception("'testGuildId' not found in user secrets. Please add a user secret for the test server.");
            }
            
            TestGuildId = ulong.Parse(testGuildId);
        #endif
    }
}