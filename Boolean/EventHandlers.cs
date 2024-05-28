using Boolean.Util;
using Boolean.Utils;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boolean;

public class EventHandlers(
    IServiceProvider serviceProvider,
    Config config,
    DiscordSocketClient client,
    InteractionService interactionService)
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
       var customId = component.Data.CustomId;
       
       // Return if not a pagination event - later this will need to handle more button events
       var isNext = customId.EndsWith(PaginatorComponentIds.NextId);
       if (!isNext && !customId.EndsWith(PaginatorComponentIds.PrevId))
           return;
       
       var id = component.Data.CustomId.Split('_').First();
       PaginatorCache.Paginators.TryGetValue(id, out var paginator);
       
       if (paginator != null) {
           await paginator.HandleChange(isNext, component);
           return;
       }
       
       await component.RespondAsync(embed: new EmbedBuilder
       {
           Title = "Paginator Timed Out",
           Description = "This paginator timed out, please use the command again to create a new one.",
           Color = EmbedColors.Fail,
       }.Build(), ephemeral: true);
   }
   
   public async Task UserJoined(IGuildUser user)
   {
       if (user.IsBot)
           return;
       
       // We can't pass in data context to the class because EventHandlers is a singleton and data context is scoped
       var db = serviceProvider.GetRequiredService<DataContext>();
       
       var guild = await db.Guilds.FirstOrDefaultAsync(g => g.Snowflake == user.Guild.Id);
       if (guild?.JoinRoleSnowflake != null) 
           await user.AddRoleAsync(guild.JoinRoleSnowflake ?? 0);
       
       var dbWelcomeChannel = await SpecialChannelTools.GetSpecialChannel(db, user.Guild.Id, SpecialChannelType.Welcome);
       if (dbWelcomeChannel == null)
           return;
       
       var welcomeChannel = await user.Guild.GetChannelAsync(dbWelcomeChannel.Snowflake) as IMessageChannel;
       await welcomeChannel?.SendMessageAsync(text: user.Mention, embed: new EmbedBuilder
       {
           Description = Config.Strings.WelcomeMsg(user.DisplayName, user.Guild.Name),
           Color = EmbedColors.Normal,
       }.Build())!;
   }
   
   public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage,
       Cacheable<IMessageChannel, ulong> originChannel, SocketReaction reaction)
   {
       if (reaction.Emote.Name != Config.Emojis.Star)
           return;
       
       // Message IDs are unique across discord yay
       var db = serviceProvider.GetRequiredService<DataContext>();
       var starReaction = await db.StarReactions
           .Include(sr => sr.Guild.SpecialChannels)
           .FirstOrDefaultAsync(sr => sr.MessageSnowflake == cachedMessage.Id);
       
       if (starReaction != null) {
           var message = await cachedMessage.GetOrDownloadAsync();
           await HandleExistingStarReaction(db, message, starReaction);
           return;
       }
       
       var channel = await originChannel.GetOrDownloadAsync();
       if (channel is not SocketTextChannel textChannel)
           return;
       
       var guild = await GuildTools.FindOrCreate(db, textChannel.Guild.Id);
       if (guild.StarboardThreshold == null)
           return;
       
       await db.StarReactions.AddAsync(new StarReaction
       {
           GuildId = textChannel.Guild.Id,
           MessageSnowflake = cachedMessage.Id
       });
       
       await db.SaveChangesAsync();
   }
   
   private async Task HandleExistingStarReaction(DataContext db, IUserMessage message, StarReaction starReaction)
   {
       if (starReaction.IsOnStarboard || starReaction.Guild.StarboardThreshold == null)
           return;
       
       var dbStarboardChannel = starReaction.Guild.SpecialChannels
           .FirstOrDefault(sc => sc.Type == SpecialChannelType.Starboard);
       
       if (dbStarboardChannel == null)
           return;
       
       starReaction.ReactionCount++;
       
       if (starReaction.ReactionCount != starReaction.Guild.StarboardThreshold) {
           await db.SaveChangesAsync();
           return;
       }
       
       var starboardChannel = await client.GetChannelAsync(dbStarboardChannel.Snowflake) as ITextChannel;
       var embed = new EmbedBuilder
       {
           Color = EmbedColors.Normal,
           Description = message.Content,
           Url = message.GetJumpUrl(),
       }.WithAuthor(message.Author);
       
       await starboardChannel.SendMessageAsync(embed: embed.Build());
       
       starReaction.IsOnStarboard = true;
       await db.SaveChangesAsync();
   }
   
   public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cachedMessage,
       Cacheable<IMessageChannel, ulong> originChannel, SocketReaction reaction)
   {
       if (reaction.Emote.Name != Config.Emojis.Star)
           return;
       
       var db = serviceProvider.GetRequiredService<DataContext>();
       var starReaction = await db.StarReactions
           .FirstOrDefaultAsync(sr => sr.MessageSnowflake == cachedMessage.Id);
       
       if (starReaction == null || starReaction.IsOnStarboard == true)
           return;
       
       starReaction.ReactionCount--;
       await db.SaveChangesAsync();
   }
}