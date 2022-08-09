using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class ForcePowerGrid
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public List<ForcePower> Powers { get; set; }
        [Parameter] public bool ShowCast { get; set; } = true;

        private ForcePower? _selectedForcePower;

        private void ShowForcePower(ForcePower power)
        {
            _selectedForcePower = power;
        }

        private void HideForcePower()
        {
            _selectedForcePower = null;
        }

        private async Task ForceClicked(ForcePower power)
        {
            await Click.InvokeAsync(power);
        }
        
        [Parameter] public EventCallback<ForcePower> Click { get; set; }
    }
}
