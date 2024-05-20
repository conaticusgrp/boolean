using Discord;
using Discord.Interactions;

namespace Boolean;

// Used to set configuration options
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("set", "Set configuration options")]
public partial class ServerConfig(DataContext db) : InteractionModuleBase<SocketInteractionContext> { }