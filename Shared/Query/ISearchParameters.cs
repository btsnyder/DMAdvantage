namespace DMAdvantage.Shared.Query
{
    public interface ISearchParameters<T>
    {
        IQueryable<T> AddToQuery(IQueryable<T> query);
    }
}
