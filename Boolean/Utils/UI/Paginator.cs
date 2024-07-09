using Boolean.Util;
using Discord;
using Discord.WebSocket;

namespace Boolean.Utils;

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
        await interaction.RespondAsync(embed: embed.Build(), components: CreateButtonsForCurrentPage(), ephemeral: true);
        await PaginatorCache.Add(_id, this);
    }
    

    private MessageComponent CreateButtonsForCurrentPage()
    {
        bool previousButtonDisabled = _currentPage == 0;

        int numberOfPages = (int) MathF.Ceiling(data.Count / (float) itemsPerPage);
        bool nextButtonDisabled = numberOfPages - 1 <= _currentPage;

        return CreateButtons(previousButtonDisabled, nextButtonDisabled);
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
            m.Components = CreateButtonsForCurrentPage();
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
