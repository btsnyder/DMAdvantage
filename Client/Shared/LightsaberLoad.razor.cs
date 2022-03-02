using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DMAdvantage.Client.Shared
{
    public partial class LightsaberLoad
    {
        private readonly Random _randomGenerator = new();

        private Color _color;
        [Parameter] public Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                ColorChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<Color> ColorChanged { get; set; }

        public LightsaberLoad()
        {
            var choice = _randomGenerator.Next(0, 7);
            Color = (Color)choice;
        }
    }
}