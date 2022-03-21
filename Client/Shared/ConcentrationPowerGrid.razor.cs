using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class ConcentrationPowerGrid
    {
        private Dictionary<string, ForcePowerResponse> _concentrationPowers = new();
        [Parameter] public Dictionary<string, ForcePowerResponse> ConcentrationPowers
        {
            get => _concentrationPowers;
            set
            {
                if (_concentrationPowers == value) return;

                _concentrationPowers = value;
                ConcentrationPowersChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<Dictionary<string, ForcePowerResponse>> ConcentrationPowersChanged { get; set; }
        [Parameter] public bool FromView { get; set; }= false;
    }
}
