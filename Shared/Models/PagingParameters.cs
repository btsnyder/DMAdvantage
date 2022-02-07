namespace DMAdvantage.Shared.Models
{
    public class PagingParameters
    {
        private const int _maxPageSize = 100;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 25;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
        }
    }
}
