# Boolean
This is the Boolean Discord bot created by the [conaticus](https://discord.gg/nhdq8Hp33B) community. It works on multiple servers and provides a variety of useful features.

## Tech Stack
- [Discord.Net](https://docs.discordnet.dev/) wrapper for Discord API
- [Entity Framework](https://learn.microsoft.com/en-us/ef/) with Postgres for database

## Contributing
Before contributing, please read carefully through our [Contributing Guidelines](https://github.com/conaticusgrp/boolean-revamp/blob/develop/CONTRIBUTING.md) so that your pull requests are more likely to be accepted into the codebase.

## Development Setup
Ensure first that you have .NET 8.0 installed.

- Clone the repository
- CD into `/Boolean`
- Copy the `appsettings.example.json` and rename it to `appsettings.json`
- Enter the necessary values into the `appsettings.json` file
    - Note that `TestGuildId` is only required in the debug build
- Run `dotnet tool install --global dotnet-ef` to install entity framework tool
- Run `dotnet ef database update` (you might need to build the program first)