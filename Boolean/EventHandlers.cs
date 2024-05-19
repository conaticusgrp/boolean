using Boolean.Util;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Boolean;

public class EventHandlers(IServiceProvider serviceProvider, Config config, DiscordSocketClient client, InteractionService interactionService)
{
    public Task LogMessage(LogMessage message)
   {
       if (message.Exception is CommandException exception) {
            Console.WriteLine($"[Command/{message.Severity}] {exception.Command.Aliases.First()} " 
                              + $"failed to execute in {exception.Context.Channel}");
            Console.WriteLine(exception);
            return Task.CompletedTask;
       } 
        
       Console.WriteLine($"[General/{message.Severity}] {message}");
       return Task.CompletedTask;
   }

   public Task Ready()
   {
        #if DEBUG
            return interactionService.RegisterCommandsToGuildAsync(config.TestGuildId);
        #else
            return interactionService.RegisterCommandsGloballyAsync();
        #endif
   }

   public Task InteractionCreated(SocketInteraction interaction)
   {
       var ctx = new SocketInteractionContext(client, interaction);
       return interactionService.ExecuteCommandAsync(ctx, serviceProvider);
   }

   // Tries to send join message to the default channel, if lacking permissions search all channels until permission found
   public async Task GuildCreate(SocketGuild guild)
   {
       await guild.DefaultChannel.SendMessageAsync(embed: new EmbedBuilder
       {
           Color = EmbedColors.Normal,
           Title = $"Thanks for adding me to {guild.Name}!",
           Description = Config.Strings.JoinMsg,
       }.Build());
   }
}