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

   // Tries to send to the default channel, if lacking permissions search all channels until you find one
   public async Task GuildCreate(SocketGuild guild)
   {
       ITextChannel? target = guild.DefaultChannel;
       var permissions = guild.CurrentUser.GetPermissions(target);
       if (permissions.SendMessages == false) 
           // Find first channel that is text and permission to send messages
           target = guild.Channels.FirstOrDefault(c =>
               c.GetChannelType() == ChannelType.Text && guild.CurrentUser.GetPermissions(c).SendMessages) as ITextChannel;
       if (target == null) return; // give up :(
       EmbedBuilder embed = new EmbedBuilder()
       {
           Color = EmbedColors.Normal,
           Title = $"Thanks for adding me to {guild.Name}!",
           Description = Config.Strings.JoinMsg,
       };
       await target.SendMessageAsync(embed: embed.Build());
   }
}