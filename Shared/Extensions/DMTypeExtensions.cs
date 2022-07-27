using DMAdvantage.Shared.Entities;

namespace DMAdvantage.Shared.Extensions
{
    public static class DMTypeExtensions
    {
        public static string GetPath<T>()
        {
            return typeof(T).Name switch
            {
                nameof(Ability) => "abilities",
                nameof(DMClass) => "classes",
                nameof(WeaponProperty) => "weaponproperties",
                nameof(ShipWeaponProperty) => "shipweaponproperties",
                _ => $"{typeof(T).Name}s".ToLower(),
            };
        }

        public static string[] GetColumns<T>()
        {
            return typeof(T).Name switch
            {
                nameof(Character) => new string[] { nameof(Character.Name), nameof(Character.PlayerName) },
                nameof(Creature) => new string[] { nameof(Creature.Name), nameof(Creature.ChallengeRating) },
                _ => new string[] { nameof(BaseEntity.Name) },
            };
        }
    }
}
