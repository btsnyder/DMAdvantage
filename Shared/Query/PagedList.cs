using DMAdvantage.Shared.Models;

namespace DMAdvantage.Shared.Query
{
    public class PagedList<T> : List<T> where T : class
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            TotalCount = totalCount;
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            AddRange(items);
        }

        public static PagedList<T> ToPagedList(List<T> source, PagingParameters paging)
        {
            var count = source.Count;
            var skipped = (paging.PageNumber - 1) * paging.PageSize;
            while (count != 0 && skipped >= count)
            {
                skipped = Math.Max(0, skipped - paging.PageSize);
                paging.PageNumber--;
            }
            var items = source.Skip(skipped).Take(paging.PageSize).ToList();
            return new PagedList<T>(items, count, paging.PageNumber, paging.PageSize);
        }
    }
}
