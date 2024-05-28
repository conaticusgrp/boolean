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
