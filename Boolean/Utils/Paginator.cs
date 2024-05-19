using Boolean.Util;
using Discord;
using Discord.WebSocket;

namespace Boolean.Utils;

public delegate EmbedBuilder PageFunc<T>(List<T> data, EmbedBuilder embed);

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

public class Paginator<T>
{
    private int _currentPage = 0;
    private readonly string _id = Guid.NewGuid().ToString();
    
    private readonly string _title;
    private readonly List<T> _data;
    private readonly PageFunc<T> _pageChangeFunc;
    private readonly int _itemsPerPage;
    private readonly DiscordSocketClient _client;
    private readonly SocketInteraction _interaction;
    
    public Paginator(
        string title,
        List<T> data,
        PageFunc<T> pageChangeFunc,
        int itemsPerPage,
        DiscordSocketClient client,
        SocketInteraction interaction)
    {
        (_title, _data, _pageChangeFunc, _itemsPerPage, _client, _interaction) = (title, data, pageChangeFunc, itemsPerPage, client, interaction);
        
        // WARNING: This is temporary and should be changed as it will cause signficant performance issues later if we have a lot of users
        _client.ButtonExecuted += PageButtonClicked;
    }
    
    public async Task SendAsync()
    {
        var embed = _pageChangeFunc(SlicePage(), CreateDefaultEmbed());
        await _interaction.RespondAsync(embed: embed.Build(), components: CreateButtons(true), ephemeral: true);
    }
    
    private MessageComponent CreateButtons(bool isPreviousDisabled = false, bool isNextDisabled = false)
    {
        return new ComponentBuilder()
            .WithButton("Previous", $"prev_btn_{_id}", disabled: isPreviousDisabled)
            .WithButton("Next", $"next_btn_{_id}", disabled: isNextDisabled)
            .Build();
    }
    
    private EmbedBuilder CreateDefaultEmbed()
    {
        return new EmbedBuilder
        {
            Title = _title,
            Color = EmbedColors.Normal,
        };
    }
    
    private List<T> SlicePage()
    {
        return _data
            .Skip(_currentPage * _itemsPerPage)
            .Take(_itemsPerPage)
            .ToList();
    }
    
    private async Task PageButtonClicked(SocketMessageComponent component)
    {
        if (!component.Data.CustomId.EndsWith(_id))
            return;
        
        switch (component.Data.CustomId) {
            case var id when id.StartsWith("next_btn"):
                _currentPage++;
                break;
            case var id when id.StartsWith("prev_btn"):
                _currentPage--;
                break;
            default:
                return;
        }
        
        var embed = _pageChangeFunc(SlicePage(), CreateDefaultEmbed());
        await _interaction.ModifyOriginalResponseAsync(m =>
        {
            m.Embed = embed.Build();
            
            if (_currentPage + _itemsPerPage + 1 > _data.Count)
                m.Components = CreateButtons(false, true);
            else if (_currentPage == 0)
                m.Components = CreateButtons(true);
            else
                m.Components = CreateButtons();
        });
        
        await component.DeferAsync();
    }
    
    ~Paginator()
    {
        _client.ButtonExecuted -= PageButtonClicked;
    }
}