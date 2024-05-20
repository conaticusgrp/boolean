using Boolean.Util;
using Boolean.Utils;
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

   public async Task InteractionCreated(SocketInteraction interaction)
   {
       var ctx = new SocketInteractionContext(client, interaction);
       await interactionService.ExecuteCommandAsync(ctx, serviceProvider);
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
   
   public async Task ButtonExecuted(SocketMessageComponent component)
   {
       string customId = component.Data.CustomId;
       
       // Return if not a pagination event - later this will need to handle more button events
       bool isNext = customId.EndsWith(PaginatorComponentIds.NextId);
       if (!isNext && !customId.EndsWith(PaginatorComponentIds.PrevId))
           return;
       
       var id = component.Data.CustomId.Split('_').First();
       var paginator = PaginatorsCache.Paginators[id];
       await paginator.HandleChange(isNext, component);
   }
}