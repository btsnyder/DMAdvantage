using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using DMAdvantage.Shared.Services.Kafka;
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

        private InitativeDataModel? _currentPlayer;
        IList<ForcePower> SelectedForcePowers { get; set; } = new List<ForcePower>();
        private List<Character> _characters;
        private List<Creature> _creatures;
        private List<ForcePower> _forcePowers;
        private Character? _selectedCharacter;
        private Creature? _selectedCreature;
        private int _healthEdit;
        private bool _initativeEditing;
        private Dictionary<string, ForcePower> _concentrationPowers = new();
        private bool _autoSave = false;
        private bool _autoLoad = false;
        private Timer _timer;
        private bool _saving = false;
        private MudForm _form;
        private readonly object _saveLock = new();

        private Encounter _model = new();

        [Parameter] public string? Id { get; set; }
        [Parameter] public bool IsView { get; set; }


        [Inject] private IApiService ApiService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private IKafkaConsumer Consumer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            _characters = await ApiService.GetViews<Character>() ?? new();
            _creatures = await ApiService.GetViews<Creature>() ?? new();
            _forcePowers = await ApiService.GetViews<ForcePower>() ?? new();

            if (Id != null)
            {
                await ReloadEncounter();
            }

            if (IsView)
            {
                await Consumer.ConnectAsync(Topics.Encounters);
                Consumer.OnMessageReceived += MessageReceived;
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

        private async void MessageReceived(object? sender, string? data)
        {
            if (data != null)
            {
                var message = KafkaMessage.Deserialize(data);
                if (message.Value == "updated")
                {
                    await ReloadEncounter();
                    Snackbar.Add("Encounter reloaded!", Severity.Info);
                }
            }
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
            StateHasChanged();
            try
            {
                _model.InitativeData.Clear();
                foreach (var init in _initatives)
                {
                    _model.InitativeData.Add(init);
                }
                _model.CurrentPlayer = _currentPlayer?.Being?.Id ?? Guid.Empty;
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

        private Task<IEnumerable<Character>> CharacterSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<Character>>(Array.Empty<Character>());
            return Task.FromResult(_characters
                .Where(x => x.ToString()!.ToLower().Contains(value.ToLower()) && x != _selectedCharacter));
        }

        private Task<IEnumerable<Creature>> CreatureSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<Creature>>(Array.Empty<Creature>());
            return Task.FromResult(_creatures
                .Where(x => x.ToString()!.ToLower().Contains(value.ToLower()) && x != _selectedCreature));
        }
        
        private void InitativeRowClickEvent(TableRowClickEventArgs<InitativeDataModel> e)
        {
            _selectedInitative = e.Item;
            if (_selectedInitative?.Being != null)
                SelectedForcePowers = _forcePowers.Where(x => _selectedInitative.Being.ForcePowers.Contains(x)).OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
        }

        private async Task OnAddCharacter()
        {
            if (_selectedCharacter == null) return;
            _selectedCharacter = await ApiService.GetEntityById<Character>(_selectedCharacter.Id);
            var data = new InitativeDataModel(_selectedCharacter);
            _initatives.Insert(0, data);
        }

        private async Task OnAddCreature()
        {
            if (_selectedCreature == null) return;
            _selectedCreature = await ApiService.GetEntityById<Creature>(_selectedCreature.Id);
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

        private void ForcePowerClicked(ForcePower? power)
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

        private void RemoveInitative(InitativeDataModel data)
        {
            if (data == _selectedInitative)
                _selectedInitative = null;
            _initatives.Remove(data);
            var removedConcetrations = _concentrationPowers.Where(x => x.Key == data.Being.Name);
            foreach (var power in removedConcetrations)
            {
                _concentrationPowers.Remove(power.Key);
            }
            if (data == _currentPlayer)
                _currentPlayer = _initatives.First();
        }

        private async Task ReloadEncounter()
        {
            _model = await ApiService.GetEncounterView(Guid.Parse(Id!)) ?? new();

            if (_model != null)
            {
                _initatives.Clear();
                foreach (var initative in _model.InitativeData)
                {
                    if (initative.Being == null) continue;
                    _initatives.Add(new InitativeDataModel(initative.Being, initative) { });
                }
                var sorted = new List<InitativeDataModel>(_initatives);
                sorted.Sort((data1, data2) => data2.Initative.CompareTo(data1.Initative));
                _initatives = new List<InitativeDataModel>(sorted);

                _currentPlayer = _initatives.FirstOrDefault(x => x.Being.Id == _model.CurrentPlayer);
                _selectedInitative ??= _initatives.FirstOrDefault();

                if (_selectedInitative?.Being != null)
                {
                    if (_selectedInitative.Being.ForcePowers.Any())
                    {
                        SelectedForcePowers = _forcePowers.Where(x => _selectedInitative.Being.ForcePowers.Contains(x))
                            .OrderBy(x => x.Level).ThenBy(x => x.Name).ToList();
                    }
                    else
                    {
                        SelectedForcePowers = new List<ForcePower>();
                    }
                }

                _concentrationPowers.Clear();
                foreach (var (key, value) in _model.ConcentrationPowers)
                {
                    var power = _forcePowers.FirstOrDefault(x => x.Id == value);
                    if (power != null)
                        _concentrationPowers.Add(key, power);
                }
            }
            StateHasChanged();
        }

        private string GetRowClass(InitativeDataModel data)
        {
            return data == _selectedInitative ? "indigo lighten-1" : string.Empty;
        }

        private string HealthBackground(InitativeDataModel data)
        {
            return data == _selectedInitative ? Colors.Indigo.Lighten1 : string.Empty;
        }

        private string InitativeDisplayName(InitativeDataModel data)
        {
            var sameBeing = _initatives.FindAll(x => x.Being.Id == data.Being.Id);
            if (sameBeing.Count == 1) return data.Name;
            return $"{data.Name} {sameBeing.IndexOf(data) + 1}";
        }
    }
}
