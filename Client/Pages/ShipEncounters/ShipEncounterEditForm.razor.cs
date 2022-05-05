using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using Timer = System.Timers.Timer;

namespace DMAdvantage.Client.Pages.ShipEncounters
{
    public partial class ShipEncounterEditForm
    {
        private bool _loading;
        private List<ShipInitativeDataModel> _initatives = new();
        private ShipInitativeDataModel? _selectedInitative;

        private ShipInitativeDataModel? _currentPlayer;
        private List<PlayerShip> _playerShips;
        private List<EnemyShip> _enemyShips;
        private PlayerShip? _selectedPlayerShip;
        private EnemyShip? _selectedEnemyShip;
        private int _hullEdit;
        private int _shieldEdit;
        private bool _initativeEditing;
        private bool _autoSave = false;
        private bool _autoLoad = false;
        private Timer _timer;
        private bool _saving = false;
        private MudForm _form;
        private readonly object _saveLock = new();

        private ShipEncounter _model = new();

        [Parameter] public string? Id { get; set; }
        [Parameter] public bool IsView { get; set; }


        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            _playerShips = await ApiService.GetAllEntities<PlayerShip>() ?? new();
            _enemyShips = await ApiService.GetAllEntities<EnemyShip>() ?? new();

            if (Id != null)
            {
                await ReloadEncounter();
            }
            
            await base.OnInitializedAsync();
            _loading = false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            _timer = new Timer(3000);
            _timer.Elapsed += async (_, _) => await AutoWork();
            _timer.Start();
        }

        private async Task AutoWork()
        {
            if ((!_autoSave && !_autoLoad) || Id == null) return;

            if (_autoSave)
                await OnValidSubmit();
            if (_autoLoad)
                await ReloadEncounter();
        }

        private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _timer.Dispose();
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }

        private async Task OnValidSubmit()
        {
            lock (_saveLock)
            {
                if (_saving)
                    return;
                _saving = true;
            }
            _loading = true;
            try
            {
                _model.InitativeData.Clear();
                foreach (var init in _initatives)
                {
                    _model.InitativeData.Add(init);
                }
                _model.CurrentPlayer = _currentPlayer?.Ship?.Id ?? Guid.Empty;
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    Snackbar.Add("Ship Encounter added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                    NavigationManager.NavigateTo("shipencounters");
                }
                else
                {
                    await ApiService.UpdateEntity(Guid.Parse(Id ?? string.Empty), _model);
                    Snackbar.Add("Update Successful", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error submitting change: {ex}", Severity.Error);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
                lock (_saveLock)
                {
                    _saving = false;
                }
            }
        }

        private void ApplyHullPoints()
        {
            _selectedInitative?.ApplyHP(_hullEdit);
            _hullEdit = 0;
        }

        private void ApplyHullDamage()
        {
            _selectedInitative?.ApplyHP(-1 * _hullEdit);
            _hullEdit = 0;
        }

        private void ApplyShieldPoints()
        {
            _selectedInitative?.ApplySP(_shieldEdit);
            _shieldEdit = 0;
        }

        private void ApplyShieldDamage()
        {
            _selectedInitative?.ApplySP(-1 * _shieldEdit);
            _shieldEdit = 0;
        }

        private Task<IEnumerable<PlayerShip>> PlayerShipSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<PlayerShip>>(Array.Empty<PlayerShip>());
            return Task.FromResult(_playerShips
                .Where(x => x.ToString()!.ToLower().Contains(value.ToLower()) && x != _selectedPlayerShip));
        }

        private Task<IEnumerable<EnemyShip>> EnemyShipSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<EnemyShip>>(Array.Empty<EnemyShip>());
            return Task.FromResult(_enemyShips
                .Where(x => x.ToString()!.ToLower().Contains(value.ToLower()) && x != _selectedEnemyShip));
        }
        
        private void InitativeRowClickEvent(TableRowClickEventArgs<ShipInitativeDataModel> e)
        {
            _selectedInitative = e.Item;
        }

        private async Task OnAddPlayerShip()
        {
            if (_selectedPlayerShip == null) return;
            _selectedPlayerShip = await ApiService.GetEntityById<PlayerShip>(_selectedPlayerShip.Id);
            var data = new ShipInitativeDataModel(_selectedPlayerShip);
            _initatives.Insert(0, data);
        }

        private async Task OnAddEnemyShip()
        {
            if (_selectedEnemyShip == null) return;
            _selectedEnemyShip = await ApiService.GetEntityById<EnemyShip>(_selectedEnemyShip.Id);
            var data = new ShipInitativeDataModel(_selectedEnemyShip);
            _initatives.Insert(0, data);
        }

        private void InitativeEditStart()
        {
            _initativeEditing = true;
        }

        private void InitativeEditDone()
        {
            _initativeEditing = false;
            var sorted = new List<ShipInitativeDataModel>(_initatives);
            sorted.Sort((data1, data2) => data2.Initative.CompareTo(data1.Initative));
            _initatives = new List<ShipInitativeDataModel>(sorted);
        }

        private void InitativeNext()
        {
            int index;
            if (_currentPlayer == null || _initatives.Last() == _currentPlayer)
                index = -1;
            else
                index = _initatives.IndexOf(_currentPlayer);
            _currentPlayer = _initatives[++index];
        }

        private void InitativePrevious()
        {
            int index;
            if (_currentPlayer == null || _initatives.First() == _currentPlayer)
                index = _initatives.Count;
            else
                index = _initatives.IndexOf(_currentPlayer);
            _currentPlayer = _initatives[--index];
        }

        private void RemoveInitative(ShipInitativeDataModel data)
        {
            if (data == _selectedInitative)
                _selectedInitative = null;
            _initatives.Remove(data);
            if (data == _currentPlayer)
                _currentPlayer = _initatives.First();
        }

        private async Task ReloadEncounter()
        {
            _model = await ApiService.GetShipEncounterView(Guid.Parse(Id!)) ?? new();

            if (_model != null)
            {
                _initatives.Clear();
                foreach (var initative in _model.InitativeData)
                {
                    if (initative.Ship == null) continue;
                    _initatives.Add(new ShipInitativeDataModel(initative.Ship, initative) { });
                }
                var sorted = new List<ShipInitativeDataModel>(_initatives);
                sorted.Sort((data1, data2) => data2.Initative.CompareTo(data1.Initative));
                _initatives = new List<ShipInitativeDataModel>(sorted);

                _currentPlayer = _initatives.FirstOrDefault(x => x.Ship.Id == _model.CurrentPlayer);
                _selectedInitative ??= _initatives.FirstOrDefault();
            }
            StateHasChanged();
        }

        private string GetRowClass(ShipInitativeDataModel data)
        {
            return data == _selectedInitative ? "indigo lighten-1" : string.Empty;
        }

        private string HealthBackground(ShipInitativeDataModel data)
        {
            return data == _selectedInitative ? Colors.Indigo.Lighten1 : string.Empty;
        }

        private string InitativeDisplayName(ShipInitativeDataModel data)
        {
            var sameShip = _initatives.FindAll(x => x.Ship.Id == data.Ship.Id);
            if (sameShip.Count == 1) return data.Name;
            return $"{data.Name} {sameShip.IndexOf(data) + 1}";
        }
    }
}
