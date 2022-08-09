using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class ConcentrationPowerGrid
    {
        private Dictionary<string, Power> _concentrationPowers = new();
        [Parameter] public Dictionary<string, Power> ConcentrationPowers
        {
            get => _concentrationPowers;
            set
            {
                if (_concentrationPowers == value) return;

                _concentrationPowers = value;
                ConcentrationPowersChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<Dictionary<string, Power>> ConcentrationPowersChanged { get; set; }
        [Parameter] public bool FromView { get; set; }= false;
    }
}
