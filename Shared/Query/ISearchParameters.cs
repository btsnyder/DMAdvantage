namespace DMAdvantage.Shared.Query
{
    public interface ISearchParameters<T>
    {
        string Search { get; }

        bool IsFound(T entity);
    }
}
