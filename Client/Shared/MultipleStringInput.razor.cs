using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public partial class MultipleStringInput
    {
        private string? _value;


        private List<string> _selectedValues = new();
        [Parameter] public List<string> SelectedValues
        {
            get => _selectedValues;
            set
            {
                if (_selectedValues == value) return;

                _selectedValues = value;
                SelectedValuesChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<List<string>> SelectedValuesChanged { get; set; }
        [Parameter] public string Label { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private void ValueAdded()
        {
            if (string.IsNullOrEmpty(_value)) return;
            SelectedValues.Add(_value);
            _value = null;
        }

        private void EntityRemoved(MudChip chip)
        {
            SelectedValues.Remove(chip.Text);
        }
    }
}
