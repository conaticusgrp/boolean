# Boolean
This is the Boolean Discord bot created by the [conaticus](https://discord.gg/nhdq8Hp33B) community. It works on multiple servers and provides a variety of useful features.

## Development Setup
Ensure first that you have .NET 8.0 installed.

- Clone the repository, and CD into `/Boolean`
- Run the `dotnet user-secrets init` command to initialise a separate file for the bot details
- Run `dotnet user-secrets set botToken [token]` to set the bot token
- Run `dotnet user-secrets set testGuildId [guildId]` so that new commands will quickly be registered for your test server.