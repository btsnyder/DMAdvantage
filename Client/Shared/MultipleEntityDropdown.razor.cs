using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleEntityDropdown<TEntity> where TEntity : BaseEntity
    {
        private List<TEntity> _data;
        private TEntity? _value;

        [Inject] private IApiService ApiService { get; set; }

        private ICollection<TEntity> _selectedEntities = new List<TEntity>();
        [Parameter] public ICollection<TEntity> SelectedEntities
        {
            get => _selectedEntities;
            set
            {
                if (_selectedEntities == value) return;

                _selectedEntities = value;
                SelectedEntitiesChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<ICollection<TEntity>> SelectedEntitiesChanged { get; set; }
        [Parameter] public string Label { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _data = await ApiService.GetAllEntities<TEntity>() ?? new List<TEntity>();
            await base.OnInitializedAsync();
        }

        private async Task<IEnumerable<TEntity>> Search(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<TEntity>();
            await Task.Yield();
            return _data
                .Where(x => x.ToString() != null &&  x.ToString()!.ToLower().Contains(value.ToLower()) && _selectedEntities.All(y => y.Id != x.Id));
        }

        private void Selected(TEntity? val)
        {
            if (val == null) return;
            _selectedEntities.Add(val);
            _value = null;
        }

        private void EntityRemoved(MudChip chip)
        {
            var entityToRemove = _selectedEntities.FirstOrDefault(x => x.ToString() == chip.Text);
            if (entityToRemove == null) return;
            _selectedEntities.Remove(entityToRemove);
        }
    }
}
