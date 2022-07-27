using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Query;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DMAdvantage.Server
{
    public static class Extensions
    {
        public static void SetPagedHeader<T>(this HttpResponse response, PagedList<T> pagedResults) where T : class
        {
            var metadata = new PagedData
            {
                TotalCount = pagedResults.TotalCount,
                PageSize = pagedResults.PageSize,
                CurrentPage = pagedResults.CurrentPage,
                TotalPages = pagedResults.TotalPages,
                HasNext = pagedResults.HasNext,
                HasPrevious = pagedResults.HasPrevious
            };

            response.Headers.Add(PagedData.Header, JsonSerializer.Serialize(metadata));
        }

        public static IQueryable<T> AsNoTrackingWithUser<T>(this IQueryable<T> queryable, string username) where T : BaseEntity, new()
        {
            return queryable.AsNoTracking().Where(c => c.User != null && c.User.UserName == username);
        }

        public static T? GetEntityByIdAndUser<T>(this IQueryable<T> queryable, string username, Guid id, bool tracked = true) where T : BaseEntity, new()
        {
            if (tracked)
                return queryable.FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);
            return queryable.AsNoTracking().FirstOrDefault(c => c.Id == id && c.User != null && c.User.UserName == username);
        }
    }
}