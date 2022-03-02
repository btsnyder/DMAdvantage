using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Query
{
    public class NamedSearchParameters<T> : ISearchParameters<T>, ISearchQuery where T : INamedEntity
    {
        public string Search { get; set; }

        public IQueryable<T> AddToQuery(IQueryable<T> query)
        {
            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(f => f.Name != null && f.Name.ToLower().Contains(Search.ToLower()));
            return query;
        }

        public string GetQuery()
        {
            return string.IsNullOrWhiteSpace(Search) ? string.Empty : $"{nameof(Search).ToLower()}={Search}";
        }
    }
}
