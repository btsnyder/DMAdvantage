using DMAdvantage.Shared.Enums;
using DMAdvantage.Shared.Models;
using System.Text.Json;

namespace DMAdvantage.Shared.Entities
{
    public class Character : Being
    {
        public string? PlayerName { get; set; }
        public int Level { get; set; }
        public string? Class { get; set; }
        public bool? StrengthSave { get; set; } = false;
        public bool? DexteritySave { get; set; } = false;
        public bool? ConstitutionSave { get; set; } = false;
        public bool? IntelligenceSave { get; set; } = false;
        public bool? WisdomSave { get; set; } = false;
        public bool? CharismaSave { get; set; } = false;
        public bool? Athletics { get; set; } = false;
        public bool? Acrobatics { get; set; } = false;
        public bool? SleightOfHand { get; set; } = false;
        public bool? Stealth { get; set; } = false;
        public bool? Investigation { get; set; } = false;
        public bool? Lore { get; set; } = false;
        public bool? Nature { get; set; } = false;
        public bool? Piloting { get; set; } = false;
        public bool? Technology { get; set; } = false;
        public bool? AnimalHandling { get; set; } = false;
        public bool? Insight { get; set; } = false;
        public bool? Medicine { get; set; } = false;
        public bool? Perception { get; set; } = false;
        public bool? Survival { get; set; } = false;
        public bool? Deception { get; set; } = false;
        public bool? Intimidation { get; set; } = false;
        public bool? Performance { get; set; } = false;
        public bool? Persuasion { get; set; } = false;
        public string? WeaponsCache { get; set; }
        public List<Weapon> Weapons => JsonSerializer.Deserialize<List<Weapon>>(WeaponsCache ?? string.Empty) ?? new();

        public override string OrderBy()
        {
            return Name ?? string.Empty;
        }
    }
}
