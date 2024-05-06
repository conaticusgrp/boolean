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
            Log($"[Command/{message.Severity}] {exception.Command.Aliases.First()} " 
                              + $"failed to execute in {exception.Context.Channel}");
            Log(exception.Message);
            return Task.CompletedTask;
       } 
        
       Log($"[General/{message.Severity}] {message}");
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
    public Task Log(string message)
    {
        if(GlobalVars.config.LogMode == "stdout")
        {
            Console.WriteLine(message);
             return Task.CompletedTask;
        }
        else
        {
            if(File.Exists(GlobalVars.config.LogMode))
            {
                File.WriteAllText(GlobalVars.config.LogMode, message + "\n");
            }
            File.AppendAllText(GlobalVars.config.LogMode, message + "\n");
            return Task.CompletedTask; 
        }
    }
   public Task InteractionCreated(SocketInteraction interaction)
   {
       var ctx = new SocketInteractionContext(client, interaction);
       return interactionService.ExecuteCommandAsync(ctx, serviceProvider);
   }
}