using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleEntityIdDropdown<TEntity> where TEntity : BaseEntity
    {
        private List<TEntity> _data;
        private string _value;
        private readonly List<TEntity> _selectedEntities = new();
        private List<Guid> _selectedIds = new();

        [Inject] private IApiService ApiService { get; set; }

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
        [Parameter] public string Label { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _data = await ApiService.GetAllEntities<TEntity>() ?? new List<TEntity>();

            if (SelectedIds.Count != 0)
                _selectedEntities.AddRange(_data.Where(x => SelectedIds.Contains(x.Id)));

            await base.OnInitializedAsync();
        }

        private async Task<IEnumerable<string>> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<string>();
            await Task.Yield();
            return _data
                .Where(x => x.ToString()!.ToLower().Contains(value.ToLower()) && !_selectedEntities.Contains(x))
                .Select(x => x.ToString()!);
        }

        private void Selected(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return;
            var selectedEntity = _data.FirstOrDefault(x => x.ToString() == val);
            if (selectedEntity == null) return;
            _selectedEntities.Add(selectedEntity);
            SelectedIds.Add(selectedEntity.Id);
            _value = "";
        }

        private void EntityRemoved(MudChip chip)
        {
            var entityToRemove = _selectedEntities.FirstOrDefault(x => x.ToString() == chip.Text);
            if (entityToRemove == null) return;
            _selectedEntities.Remove(entityToRemove);
            SelectedIds.Remove(SelectedIds.FirstOrDefault(x => x == entityToRemove.Id));
        }
    }
}
