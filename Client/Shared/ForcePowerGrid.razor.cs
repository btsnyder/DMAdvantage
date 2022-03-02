using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class ForcePowerGrid
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public List<ForcePowerResponse> Powers { get; set; }
        [Parameter] public bool ShowCast { get; set; } = true;

        private ForcePowerResponse? _selectedForcePower;

        private void ShowForcePower(ForcePowerResponse power)
        {
            _selectedForcePower = power;
        }

        private void HideForcePower()
        {
            _selectedForcePower = null;
        }

        private async Task ForceClicked(ForcePowerResponse power)
        {
            await Click.InvokeAsync(power);
        }
        
        [Parameter] public EventCallback<ForcePowerResponse> Click { get; set; }
    }
}
