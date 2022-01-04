using DMAdvantage.Client.Services;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class EncounterView
    {
        private EncounterResponse? _model;
        private bool _loading;
        private readonly List<InitativeDataModel> _initatives = new();
        private object updateLock = new object();

        [Inject]
        IAlertService AlertService { get; set; }
        [Inject]
        IApiService ApiService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReloadEncounter();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                var timer = new Timer(new TimerCallback(async _ =>
                {
                    await ReloadEncounter();
                }), null, 3000, 3000);
            }
        }

        private async Task ReloadEncounter()
        {
            _model = await ApiService.GetEncounterView(Guid.Parse(Id));

            if (_model != null)
            {
                List<IBeingResponse> beings = new();
                var characters = await ApiService.GetCharacterViews(_model.Data.Select(x => x.BeingId));
                var creatures = await ApiService.GetCreatureViews(_model.Data.Select(x => x.BeingId));
                beings.AddRange(characters);
                beings.AddRange(creatures);

                _initatives.Clear();
                foreach (var being in beings)
                {
                    _initatives.Add(new InitativeDataModel(being, _model.Data.First(x => x.BeingId == being.Id)));
                }
            }

            StateHasChanged();
        }
    }
}
