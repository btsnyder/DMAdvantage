using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Query
{
    public class NamedSearchParameters<T> : ISearchParameters<T>, ISearchQuery where T : INamedEntity
    {
        public string Search { get; set; }

        public bool IsFound(T entity)
        {
            if (!string.IsNullOrWhiteSpace(Search) && entity.Name?.ToLower().Contains(Search.ToLower()) != true)
                return false;
            return true;
        }

        public string GetQuery()
        {
            return string.IsNullOrWhiteSpace(Search) ? string.Empty : $"{nameof(Search).ToLower()}={Search}";
        }
    }
}
