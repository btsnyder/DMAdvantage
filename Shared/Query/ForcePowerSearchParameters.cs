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
            if (Levels.Length != 0)
                query = query.Where(f => Levels.Contains(f.Level));
            if (Alignments.Length != 0)
                query = query.Where(f => Alignments.Contains(f.Alignment));
            if (CastingPeriods.Length != 0)
                query = query.Where(f => CastingPeriods.Contains(f.CastingPeriod));
            if (Ranges.Length != 0)
                query = query.Where(f => Ranges.Contains(f.Range));
            return query;
        }

        public string GetQuery()
        {
            string query = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
                query += $"{nameof(Search).ToLower()}={Search}";
            foreach (var level in Levels)
            {
                query += $"&{nameof(Levels).ToLower()}={level}";
            }
            foreach (var alignment in Alignments)
            {
                query += $"&{nameof(Alignments).ToLower()}={alignment}";
            }
            foreach (var casting in CastingPeriods)
            {
                query += $"&{nameof(CastingPeriods).ToLower()}={casting}";
            }
            foreach (var range in Ranges)
            {
                query += $"&{nameof(Ranges).ToLower()}={range}";
            }
            query = query.TrimStart('&');
            return query;
        }
    }
}
