using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Shared
{
    public partial class ForcePowerGrid
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public List<ForcePowerResponse> Powers { get; set; }

        private ForcePowerResponse? _selectedForcePower;

        void ShowForcePower(ForcePowerResponse power)
        {
            _selectedForcePower = power;
        }

        void HideForcePower()
        {
            _selectedForcePower = null;
        }
    }
}
