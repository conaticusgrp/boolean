# Contributing
## Introduction
Hello and thank you for checking out the contributing guidelines for Boolean! Please read these carefully before contributing.

## Issues
All tasks for the project are listed in the repo's [issues](https://github.com/conaticusgrp/boolean-revamp). Feel free to work on any issue that does not have an assignee. There is no guarantee this issue has not already been worked on - it's good to check the PRs page first.

## Code Style
For this project we will be using common C# coding conventions. Please follow these as much as possible to keep our codebase consistent and readable.
### Naming Conventions
- Types identifiers (classes, structs, enums etc) are pascal case
- Private members are prefixed with an underscore in camel case
- Public members are pascal case
- Methods and functions are pascal case
- Local variables and arguments are camel case
```csharp
class MyClass
{
    private int _myNumber = 5;
    public string MyString = "Hello World";
    
    void MyMethod(int myArgument)
    {
        int localVariable = myArgument;
    }
}
```

### Code Blocks
All control statements (e.g `if`, `while`, `for`) have the curly brace on the same line. All other code blocks have the curly brace on the next line.
```csharp
class MyClass
{
    void MyMethod()
    {
        bool x = true;
        if (x) {
            Console.WriteLine("x is true");
        }
    }
}
```

## Code Spacing
Please avoid writing code without any spaces between as it becomes difficult to read. It's better to group pieces of similar functionality together.
Please also avoid adding long lines of code without being separated into multiple lines.

**Good:**
```csharp
_client = new DiscordSocketClient();
_config = new BotConfig();
_interactionService = new InteractionService(_client.Rest);

var collection = new ServiceCollection()
    .AddSingleton(_interactionService)
    .AddSingleton(_client)
    .AddSingleton(_config)
    .AddSingleton<DiscordSocketConfig>()
    .AddSingleton<EventHandlers>();

return collection.BuildServiceProvider();
```

**Bad:**
```csharp
_client = new DiscordSocketClient();
_config = new BotConfig();
_interactionService = new InteractionService(_client.Rest);
var collection = new ServiceCollection().AddSingleton(_interactionService).AddSingleton(_client).AddSingleton(_config).AddSingleton<DiscordSocketConfig>().AddSingleton<EventHandlers>();
return collection.BuildServiceProvider();
```

## Coding Conventions
- Write code with re-usability in mind, if something is likely to be re-used, abstract it into its own class(es)
- Write performant and well-optimised code, some good examples are:
  - Avoiding for loops where they are not needed
  - Using Hashmaps and Hashsets instead of arrays when finding elements
  - Fetching data from the Discord.NET cache instead of the API
- Place all new commands inside the `/Modules` folder
- Avoid writing long functions, and instead split functionality up into multiple functions

## DOs and DON'Ts
Please do:
- Mention any new packages added and their purpose in the PR description
- Write well thought-out code that is easy to read
- Use meaningful commit messages

Please don't:
- Add forked packages that are not by the official creator
- Push code that you did not write yourself
- Use external packages where they are not necessary/reputable

If you are unsure about anything, please feel free to contact any of the maintainers in our [Discord Server](https://discord.gg/nhdq8Hp33B)!