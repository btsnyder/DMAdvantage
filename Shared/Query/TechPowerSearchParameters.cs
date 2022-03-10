using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Enums;

namespace DMAdvantage.Shared.Query
{
    public class TechPowerSearchParameters : ISearchParameters<TechPower>, ISearchQuery
    {
        public string Search { get; set; }
        public int[] Levels { get; set; } = Array.Empty<int>();
        public CastingPeriod[] CastingPeriods { get; set; } = Array.Empty<CastingPeriod>();
        public PowerRange[] Ranges { get; set; } = Array.Empty<PowerRange>();

        public bool IsFound(TechPower power)
        {
            if (!string.IsNullOrWhiteSpace(Search) && power.Name?.ToLower().Contains(Search.ToLower()) != true)
                return false;
            if (Levels.Any() && !Levels.Contains(power.Level))
                return false;
            if (CastingPeriods.Any() && !CastingPeriods.Contains(power.CastingPeriod))
                return false;
            if (Ranges.Any() && !Ranges.Contains(power.Range))
                return false;
            return true;
        }

        public string GetQuery()
        {
            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
                query += $"{nameof(Search).ToLower()}={Search}";
            query = Levels.Aggregate(query, (current, level) => current + $"&{nameof(Levels).ToLower()}={level}");
            query = CastingPeriods.Aggregate(query, (current, casting) => current + $"&{nameof(CastingPeriods).ToLower()}={casting}");
            query = Ranges.Aggregate(query, (current, range) => current + $"&{nameof(Ranges).ToLower()}={range}");
            query = query.TrimStart('&');
            return query;
        }
    }
}
