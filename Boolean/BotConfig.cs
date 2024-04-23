using Microsoft.Extensions.Configuration;

namespace Boolean;

public class BotConfig
{
    public string? Token;
    
    private readonly IConfigurationRoot _config;
    
    public BotConfig()
    {
        _config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        Token = _config["botToken"];
    }
}