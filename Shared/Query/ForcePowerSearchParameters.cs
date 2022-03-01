using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Query
{
    public class ForcePowerSearchParameters : ISearchParameters<ForcePower>, ISearchQuery
    {
        public string? Search { get; set; }
        public int[] Levels { get; set; } = Array.Empty<int>();
        public ForceAlignment[] Alignments { get; set; } = Array.Empty<ForceAlignment>();
        public CastingPeriod[] CastingPeriods { get; set; } = Array.Empty<CastingPeriod>();
        public PowerRange[] Ranges { get; set; } = Array.Empty<PowerRange>();

        public IQueryable<ForcePower> AddToQuery(IQueryable<ForcePower> query)
        {
            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(f => f.Name != null && f.Name.ToLower().Contains(Search.ToLower()));
            if (Levels.Any())
                query = query.Where(f => Levels.Contains(f.Level));
            if (Alignments.Any())
                query = query.Where(f => Alignments.Contains(f.Alignment));
            if (CastingPeriods.Any())
                query = query.Where(f => CastingPeriods.Contains(f.CastingPeriod));
            if (Ranges.Any())
                query = query.Where(f => Ranges.Contains(f.Range));
            return query;
        }

        public string GetQuery()
        {
            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
                query += $"{nameof(Search).ToLower()}={Search}";
            query = Levels.Aggregate(query, (current, level) => current + $"&{nameof(Levels).ToLower()}={level}");
            query = Alignments.Aggregate(query, (current, alignment) => current + $"&{nameof(Alignments).ToLower()}={alignment}");
            query = CastingPeriods.Aggregate(query, (current, casting) => current + $"&{nameof(CastingPeriods).ToLower()}={casting}");
            query = Ranges.Aggregate(query, (current, range) => current + $"&{nameof(Ranges).ToLower()}={range}");
            query = query.TrimStart('&');
            return query;
        }
    }
}
