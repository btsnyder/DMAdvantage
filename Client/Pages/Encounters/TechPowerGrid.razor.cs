using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace DMAdvantage.Client.Pages.Encounters
{
    public partial class TechPowerGrid
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public List<TechPower> Powers { get; set; }
        [Parameter] public bool ShowCast { get; set; } = true;

        private TechPower? _selectedTechPower;

        private void ShowTechPower(TechPower power)
        {
            _selectedTechPower = power;
        }

        private void HideTechPower()
        {
            _selectedTechPower = null;
        }

        private async Task TechClicked(TechPower power)
        {
            await Click.InvokeAsync(power);
        }
        
        [Parameter] public EventCallback<TechPower> Click { get; set; }
    }
}
