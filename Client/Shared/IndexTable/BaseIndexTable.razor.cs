using DMAdvantage.Shared.Extensions;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared.IndexTable
{
    public partial class BaseIndexTable<T>
    {
        private bool _loading = true;
        private string _search = "";
        private List<T> _entities = new();
        private IEnumerable<string> _columnNames = Array.Empty<string>();

        [Parameter] public string? ViewRouteProperty { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _columnNames = DMTypeExtensions.GetColumns<T>();
            await RefreshEntities();
            await base.OnInitializedAsync();
            _loading = false;
        }

        public async Task RemoveEntity(T entity)
        {
            if (_entities == null)
                return;
            _entities.Remove(entity);
            await ApiService.RemoveEntity<T>(entity.Id);
        }

        private async Task RefreshEntities()
        {
            _entities = await ApiService.GetAllEntities<T>() ?? new();
        }

        public void EditEntity(Guid id)
        {
            NavigationManager.NavigateTo($"{DMTypeExtensions.GetPath<T>()}/edit/{id}");
        }

        [Parameter] public EventCallback<bool> SelectedEntitiesChanged { get; set; }


        private bool SearchEntity(T entity)
        {
            if (string.IsNullOrWhiteSpace(_search))
                return true;
            return entity.IsFound(_search);
        }

        private string GetDisplayName()
        {
            var name = DMTypeExtensions.GetPath<T>();
            if (name.Length < 2)
                return name.ToUpper();
            return name[0].ToString().ToUpper() + name[1..];
        }
    }
}
