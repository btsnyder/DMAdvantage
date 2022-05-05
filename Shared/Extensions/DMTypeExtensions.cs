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
    }
}
