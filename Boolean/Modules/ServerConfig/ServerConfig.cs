using Discord;
using Discord.Interactions;

namespace Boolean;

// Used to set configuration options
[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("set", "Set configuration options")]
public partial class ServerSet(DataContext db) : InteractionModuleBase<SocketInteractionContext> { }

[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("get", "Get configuration options")]
public partial class ServerGet(DataContext db) : InteractionModuleBase<SocketInteractionContext> { }

[DefaultMemberPermissions(GuildPermission.Administrator)]
[Group("unset", "Unset configuration options")]
public partial class ServerUnset(DataContext db) : InteractionModuleBase<SocketInteractionContext> { }
