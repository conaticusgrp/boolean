using Discord;
using Microsoft.Extensions.Configuration;

namespace Boolean;

public class BotConfig
{
    public readonly string? Token;
    public readonly Color BotTheme = Color.Gold;
    
    #if DEBUG
        public readonly ulong TestGuildId;
    #endif
    
    private readonly IConfigurationRoot _config;
    
    public BotConfig()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Token = _config["botToken"];
        
        #if DEBUG
            string? testGuildId = _config["testGuildId"];
        
            if (testGuildId == null) {
                throw new Exception("'testGuildId' not found in user secrets. Please add a user secret for the test server.");
            }
            
            TestGuildId = ulong.Parse(testGuildId);
        #endif
    }
}