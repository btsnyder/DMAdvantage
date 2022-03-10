using Microsoft.AspNetCore.Mvc;
using DMAdvantage.Shared.Query;
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
    }
}