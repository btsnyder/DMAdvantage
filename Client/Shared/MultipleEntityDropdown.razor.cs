using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleEntityDropdown<TEntity> where TEntity : IEntityResponse
    {
        private List<TEntity> _data;
        private IEnumerable<string> _names;
        private IEnumerable<string> _selected;

        [Inject]
        IApiService ApiService { get; set; }

        private List<Guid> _selectedIds = new();
        [Parameter]
        public List<Guid> SelectedIds
        {
            get => _selectedIds;
            set
            {
                if (_selectedIds == value) return;

                _selectedIds = value;
                SelectedIdsChanged.InvokeAsync(value);
            }
        }
        [Parameter]
        public EventCallback<List<Guid>> SelectedIdsChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _data = await ApiService.GetAllEntities<TEntity>() ?? new List<TEntity>();
            _names = _data.Select(x => x.Display).ToList();

            if (SelectedIds.Count != 0)
                _selected = _data.Where(x => SelectedIds.Contains(x.Id)).Select(x => x.Display);

            await base.OnInitializedAsync();
        }

        void OnChange()
        {
            SelectedIds = _data.Where(x => _selected.Contains(x.Display)).Select(x => x.Id).ToList();
        }
    }
}
