using DMAdvantage.Shared.Query;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class PagingFooter<T> where T : class
    {
        private PagedList<T>? _data;
        [Parameter]
        public PagedList<T>? Data
        {
            get => _data;
            set
            {
                if (_data == value) return;

                _data = value;
                if (_data != null)
                    DataChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<PagedList<T>> DataChanged { get; set; }

        private int _currentPage = 1;
        [Parameter]
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value) return;

                _currentPage = value;
                CurrentPageChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }

        private void UpdateCurrentPage(int newPage)
        {
            if (Data == null)
                return;
            if (newPage == -1 && Data.HasPrevious)
                CurrentPage--;
            else
                CurrentPage = newPage;
        }

        public string GetDisplay()
        {
            if (_data == null || _data.Count == 0)
                return string.Empty;
            var startingIndex = (_data.CurrentPage - 1) * _data.PageSize + 1;
            if (_data.Count == 1)
                return $"{startingIndex} of {_data.TotalCount}";
            var endingIndex = Math.Min(_data.CurrentPage * _data.PageSize, startingIndex + _data.Count - 1);
            return $"{startingIndex} - {endingIndex} of {_data.TotalCount}";
        }
    }
}
