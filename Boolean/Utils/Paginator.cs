using Boolean.Util;
using Discord;
using Discord.WebSocket;

namespace Boolean.Utils;

// In the future, we may wish to store paginators in the db with an expiry - to save memory & allow use after bot restarts
public static class PaginatorCache
{
    public static readonly Dictionary<string, IPaginator> Paginators = new();
    
    public static Task Add(string id, IPaginator paginator)
    {
        Paginators.Add(id, paginator);
        
        // Remove paginator from cache after 2 minutes
        return Task.Delay(TimeSpan.FromMinutes(2))
            .ContinueWith(t => Paginators.Remove(id));
    }
}

public static class PaginatorComponentIds
{
    public const string NextId = "pagination_next_btn";
    public const string PrevId = "pagination_prev_btn";
}

public interface IPaginator
{
    Task HandleChange(bool isNext, SocketMessageComponent component);
}

public delegate EmbedBuilder PageFunc<T>(List<T> data, EmbedBuilder embed);

public class Paginator<T>(
    string title,
    List<T> data,
    PageFunc<T> pageChangeFunc,
    int itemsPerPage,
    DiscordSocketClient client,
    SocketInteraction interaction)
    : IPaginator
{
    private int _currentPage = 0;
    private readonly string _id = Guid.NewGuid().ToString();
    
    public async Task SendAsync()
    {
        var embed = pageChangeFunc(SlicePage(), CreateDefaultEmbed());
        await interaction.RespondAsync(embed: embed.Build(), components: CreateButtons(true), ephemeral: true);
        await PaginatorCache.Add(_id, this);
    }
    
    private MessageComponent CreateButtons(bool isPreviousDisabled = false, bool isNextDisabled = false)
    {
        return new ComponentBuilder()
            .WithButton("Previous", $"{_id}_{PaginatorComponentIds.PrevId}", disabled: isPreviousDisabled)
            .WithButton("Next", $"{_id}_{PaginatorComponentIds.NextId}", disabled: isNextDisabled)
            .Build();
    }
    
    private EmbedBuilder CreateDefaultEmbed()
    {
        return new EmbedBuilder
        {
            Title = title,
            Color = EmbedColors.Normal,
        };
    }
    
    private List<T> SlicePage()
    {
        return data
            .Skip(_currentPage * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();
    }
    
    public async Task HandleChange(bool isNext, SocketMessageComponent component)
    {
        if (isNext)
            _currentPage++;
        else
            _currentPage--;
        
        var embed = pageChangeFunc(SlicePage(), CreateDefaultEmbed());
        await interaction.ModifyOriginalResponseAsync(m =>
        {
            m.Embed = embed.Build();
            
            if (_currentPage + itemsPerPage + 1 > data.Count)
                m.Components = CreateButtons(false, true);
            else if (_currentPage == 0)
                m.Components = CreateButtons(true);
            else
                m.Components = CreateButtons();
        });
        
        await component.DeferAsync();
    }
}

public class PaginatorBuilder<T>
{
    private string _title;
    private List<T> _data;
    private PageFunc<T> _pageChangeFunc;
    private DiscordSocketClient _client;
    private int _itemsPerPage;
    private SocketInteraction _interaction;
    
    public PaginatorBuilder<T> WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public PaginatorBuilder<T> WithData(List<T> data)
    {
        _data = data;
        return this;
    }
    
    public PaginatorBuilder<T> WithPageChangeHandler(PageFunc<T> pageChangeFunc)
    {
        _pageChangeFunc = pageChangeFunc;
        return this;
    }
    
    public PaginatorBuilder<T> WithItemsPerPage(int items)
    {
        _itemsPerPage = items;
        return this;
    }
    
    public PaginatorBuilder<T> WithClient(DiscordSocketClient client)
    {
        _client = client;
        return this;
    }
    
    public PaginatorBuilder<T> WithInteraction(SocketInteraction interaction)
    {
        _interaction = interaction;
        return this;
    }
    
    public Paginator<T> Build()
    {
        return new Paginator<T>(_title, _data, _pageChangeFunc, _itemsPerPage, _client, _interaction);
    }
}
