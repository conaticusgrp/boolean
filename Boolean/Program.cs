using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Boolean;

class Program
{
    public static async Task Main()
    {
        await Bot.Start();
    }
}