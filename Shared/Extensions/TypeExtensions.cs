namespace DMAdvantage.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static string GetPath(this Type t)
        {
            var path = t.Name;
            if (path.Contains("Ability"))
                return "abilities";
            if (path.Contains("Class"))
                return "classes";
            if (path.Contains("WeaponProperty"))
                return "weaponproperties";
            path = path.Replace("Request", "");
            path = path.Replace("Response", "");
            path += "s";
            return path.ToLower();
        }
    }
}
