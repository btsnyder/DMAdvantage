using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleEntityDropdown<TEntity> where TEntity : IEntityResponse
    {
        private List<TEntity> _data;
        private IEnumerable<string> _names;
        private readonly List<string> _nameDisplay = new();

        private IEnumerable<string> _selectedNames = Array.Empty<string>();
        [Parameter]
        public IEnumerable<string> SelectedNames
        {
            get => _selectedNames;
            set
            {
                if (_selectedNames == value) return;

                _selectedNames = value;
                SelectedNamesChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<IEnumerable<string>> SelectedNamesChanged { get; set; }

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
                SelectedNames = _data.Where(x => SelectedIds.Contains(x.Id)).Select(x => x.Display);

            await base.OnInitializedAsync();
        }

        void OnChange()
        {
            SelectedIds = _data.Where(x => SelectedNames.Contains(x.Display)).Select(x => x.Id).ToList();
            if (typeof(TEntity) == typeof(ForcePowerResponse) || typeof(TEntity) == typeof(TechPowerResponse))
                UpdateSelectedDisplay();
        }

        private void UpdateSelectedDisplay()
        {
            _nameDisplay.Clear();
            for (int i = 0; i < 10; i++)
            {
                AddLevelDisplay(i);
            }
        }

        private void AddLevelDisplay(int level)
        {
            var powers = SelectedNames.Where(x => x.StartsWith(level.ToString())).Select(x => x[4..]);
            if (powers.Any())
            {
                var prefix = $"Lvl {level}";
                if (level == 0)
                    prefix = "At-will";
                _nameDisplay.Add($"{prefix}: {string.Join(',', powers)}");
            }
            
        }
    }
}
