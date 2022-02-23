using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DMAdvantage.Client.Shared
{
    public partial class ForcePowerGrid
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public List<ForcePowerResponse> Powers { get; set; }
        [Parameter] public bool ShowCast { get; set; } = true;

        private ForcePowerResponse? _selectedForcePower;

        void ShowForcePower(ForcePowerResponse power)
        {
            _selectedForcePower = power;
        }

        void HideForcePower()
        {
            _selectedForcePower = null;
        }

        async Task ForceClicked(ForcePowerResponse power)
        {
            await Click.InvokeAsync(power);
        }
        
        [Parameter] public EventCallback<ForcePowerResponse> Click { get; set; }
    }
}
