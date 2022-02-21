using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleEntityDropdown<TEntity> where TEntity : class, IEntityResponse
    {
        private List<TEntity> _data;
        private IEnumerable<string> _names;
        private readonly List<string> _nameDisplay = new();
        private readonly List<TEntity> _selectedEntities = new();
        private string _value;

        [Inject] private IApiService ApiService { get; set; }

        private List<Guid> _selectedIds = new();
        [Parameter] public List<Guid> SelectedIds
        {
            get => _selectedIds;
            set
            {
                if (_selectedIds == value) return;

                _selectedIds = value;
                SelectedIdsChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<List<Guid>> SelectedIdsChanged { get; set; }

        private string _label;
        [Parameter] public string Label
        {
            get => _label;
            set
            {
                if (_label == value) return;

                _label = value;
                LabelChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<string> LabelChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _data = await ApiService.GetAllEntities<TEntity>() ?? new List<TEntity>();
            _names = _data.Select(x => x.Display).ToList();

            if (SelectedIds.Count != 0)
                _selectedEntities.AddRange(_data.Where(x => SelectedIds.Contains(x.Id)));

            await base.OnInitializedAsync();
        }

        private async Task<IEnumerable<string>> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();
            await Task.Yield();
            return _data
                .Where(x => x.Display.ToLower().Contains(value.ToLower()) && !_selectedEntities.Contains(x))
                .Select(x => x.Display);
        }

        private void Selected(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return;
            var selectedEntity = _data.FirstOrDefault(x => x.Display == val);
            if (selectedEntity == null) return;
            _selectedEntities.Add(selectedEntity);
            SelectedIds.Add(selectedEntity.Id);
            _value = "";
        }

        private void EntityRemoved(MudChip chip)
        {
            var entityToRemove = _selectedEntities.FirstOrDefault(x => x.Display == chip.Text);
            if (entityToRemove == null) return;
            _selectedEntities.Remove(entityToRemove);
            SelectedIds.Remove(SelectedIds.FirstOrDefault(x => x == entityToRemove.Id));
        }
    }
}
