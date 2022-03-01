using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using Timer = System.Timers.Timer;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class EncounterEditForm
    {
        private bool _loading;
        private List<InitativeDataModel> _initatives = new();
        private InitativeDataModel? _selectedInitative;
        private CharacterResponse? _selectedInitativeCharacter => _selectedInitative?.Being as CharacterResponse;
        private CreatureResponse? _selectedInitativeCreature => _selectedInitative?.Being as CreatureResponse;

        private InitativeDataModel? _currentPlayer;
        IList<ForcePowerResponse> SelectedForcePowers { get; set; } = new List<ForcePowerResponse>();
        private List<CharacterResponse> _characters;
        private List<CreatureResponse> _creatures;
        private List<ForcePowerResponse> _forcePowers;
        private CharacterResponse? _selectedCharacter;
        private CreatureResponse? _selectedCreature;
        private int _healthEdit;
        private bool _initativeEditing;
        private Dictionary<string, ForcePowerResponse> _concentrationPowers = new();
        private bool _autoSave = false;
        private Timer _timer;
        private bool _saving = false;
        private MudForm _form;
        private readonly object _saveLock = new();

        private EncounterRequest _model = new();

        [Parameter] public string? Id { get; set; }

        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            if (Id != null)
            {
                _model = await ApiService.GetEntityById<EncounterResponse>(Guid.Parse(Id)) ?? new();

                List<IBeingResponse> beings = new();
                var characters = await ApiService.GetViews<CharacterResponse>(_model.Data.Select(x => x.BeingId));
                var creatures = await ApiService.GetViews<CreatureResponse>(_model.Data.Select(x => x.BeingId));
                beings.AddRange(characters);
                beings.AddRange(creatures);

                foreach (var being in beings)
                {
                    _initatives.Add(new InitativeDataModel(being, _model.Data.First(x => x.BeingId == being.Id)));
                }

                var sorted = new List<InitativeDataModel>(_initatives);
                sorted.Sort(delegate (InitativeDataModel data1, InitativeDataModel data2) { return data2.Initative.CompareTo(data1.Initative); });
                _initatives = new(sorted);
            }

            _selectedInitative = _initatives.FirstOrDefault();

            _characters = await ApiService.GetAllEntities<CharacterResponse>() ?? new();
            _creatures = await ApiService.GetAllEntities<CreatureResponse>() ?? new();
            _forcePowers = await ApiService.GetAllEntities<ForcePowerResponse>() ?? new();

            if (_selectedInitative?.Being != null)
                SelectedForcePowers = _forcePowers.Where(x => _selectedInitative.Being.ForcePowerIds.Contains(x.Id)).OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();

            _currentPlayer = _initatives.FirstOrDefault(x => x.BeingId == _model.CurrentPlayer);

            foreach (var concentration in _model.ConcentrationPowers)
            {
                var power = _forcePowers.FirstOrDefault(x => x.Id == concentration.Value);
                if (power != null)
                    _concentrationPowers.Add(concentration.Key, power);
            }

            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;
            _timer = new Timer(3000);
            _timer.Elapsed += async (_, _) => await AutoSave();
            _timer.Start();
        }

        private async Task AutoSave()
        {
            if (Id == null || !_autoSave)
                return;

            await OnValidSubmit();
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
                _model.Data.Clear();
                foreach (var init in _initatives)
                {
                    _model.Data.Add(init);
                }
                _model.CurrentPlayer = _currentPlayer?.BeingId ?? Guid.Empty;
                _model.ConcentrationPowers.Clear();
                foreach (var power in _concentrationPowers)
                {
                    _model.ConcentrationPowers.Add(power.Key, power.Value.Id);
                }
                if (Id == null)
                {
                    await ApiService.AddEntity(_model);
                    Snackbar.Add("Encounter added successfully", Severity.Success, cfg => { cfg.CloseAfterNavigation = false; });
                    NavigationManager.NavigateTo("encounters");
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

        private void ApplyHealth()
        {
            _selectedInitative?.ApplyHP(_healthEdit);
            _healthEdit = 0;
        }

        private void ApplyDamage()
        {
            _selectedInitative?.ApplyHP(-1 * _healthEdit);
            _healthEdit = 0;
        }

        private Task<IEnumerable<CharacterResponse>> CharacterSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<CharacterResponse>>(Array.Empty<CharacterResponse>());
            return Task.FromResult(_characters
                .Where(x => x.Display.ToLower().Contains(value.ToLower()) && x != _selectedCharacter));
        }

        private Task<IEnumerable<CreatureResponse>> CreatureSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<CreatureResponse>>(Array.Empty<CreatureResponse>());
            return Task.FromResult(_creatures
                .Where(x => x.Display.ToLower().Contains(value.ToLower()) && x != _selectedCreature));
        }
        
        private void InitativeRowClickEvent(TableRowClickEventArgs<InitativeDataModel> e)
        {
            _selectedInitative = e.Item;
            if (_selectedInitative?.Being != null)
                SelectedForcePowers = _forcePowers.Where(x => _selectedInitative.Being.ForcePowerIds.Contains(x.Id)).OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
        }

        private void OnAddCharacter()
        {
            if (_selectedCharacter == null) return;
            var data = new InitativeDataModel(_selectedCharacter);
            _initatives.Insert(0, data);
        }

        private void OnAddCreature()
        {
            if (_selectedCreature == null) return;
            var data = new InitativeDataModel(_selectedCreature);
            _initatives.Insert(0, data);
        }

        private void InitativeEditStart()
        {
            _initativeEditing = true;
        }

        private void InitativeEditDone()
        {
            _initativeEditing = false;
            var sorted = new List<InitativeDataModel>(_initatives);
            sorted.Sort((data1, data2) => data2.Initative.CompareTo(data1.Initative));
            _initatives = new List<InitativeDataModel>(sorted);
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

        private void ForcePowerClicked(ForcePowerResponse? power)
        {
            if (_selectedInitative == null || power == null || power.Level < 1)
                return;
            if (_selectedInitative.CurrentFP < power.Level + 1)
            {
                Snackbar.Add("Not enough force points!", Severity.Error);
            }
            else
            {
               _selectedInitative.CurrentFP -= power.Level + 1;
               if (power.Concentration)
               {
                   _concentrationPowers.Add(_selectedInitative.Name ?? $"Player {_initatives.IndexOf(_selectedInitative)}", power);
               }
               StateHasChanged();
            }
        }
    }
}
