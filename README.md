# Boolean
This is the Boolean Discord bot created by the [conaticus](https://discord.gg/nhdq8Hp33B) community. It works on multiple servers and provides a variety of useful features.

## Contributing
Before contributing, please read carefully through our [Contributing Guidelines](https://github.com/conaticusgrp/boolean-revamp/blob/develop/CONTRIBUTING.md) so that your pull requests are more likely to be accepted into the codebase.

## Development Setup
Ensure first that you have .NET 8.0 installed.

- Clone the repository, and CD into `/Boolean`
- Copy the `secrets.example.json` and rename it to `secrets.json`
- Enter the necessary values into the `secrets.json` file
- Run `dotnet ef database update` (you might need to build the program first)