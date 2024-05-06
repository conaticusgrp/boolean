using Discord;
using Microsoft.Extensions.Configuration;

namespace Boolean;

public class BotConfig
{
    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("secrets.json")
        .Build();
    
    public readonly Color BotTheme = Color.Gold;
    
#if DEBUG
    public readonly ulong TestGuildId;
#endif
    public readonly string? Token, Database, Username, Password;
    
    public BotConfig()
    {
        Token = _config["discordToken"];
        Database = _config["dbName"];
        Username = _config["dbUsername"];
        Password = _config["dbPassword"];
        
#if DEBUG
        var testGuildId = _config["testGuildId"];
        
        if (testGuildId == null)
            throw new Exception(
                "'testGuildId' not found in user secrets. Please add a user secret for the test server.");
        
        TestGuildId = ulong.Parse(testGuildId);
#endif
    }
}