using DMAdvantage.Client.Services;
using DMAdvantage.Client.Validators;
using DMAdvantage.Shared.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Pages.TechPowers
{
    public partial class TechEditForm
    {
        private List<TechPower> _techPowers;
        private List<string> _startingDurations = new();
        private readonly TechPowerValidator _techPowerValidator = new();

        protected override async Task OnInitializedAsync()
        {
            _techPowers = await ApiService.GetAllEntities<TechPower>() ?? new List<TechPower>();
            _startingDurations = _techPowers.Select(x => x.Duration ?? string.Empty).Distinct().ToList();

            await base.OnInitializedAsync();
        }

        private Task<IEnumerable<string>> DurationSearch(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
            return Task.FromResult(_startingDurations
                .Where(x => x.ToLower().Contains(value.ToLower())));
        }
    }
}