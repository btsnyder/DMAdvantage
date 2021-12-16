using System.Reflection;

namespace DMAdvantage.Shared.Enums
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo? fieldInfo = type.GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();
            EnumStringAttribute[] attribs = fieldInfo.GetCustomAttributes(
                    typeof(EnumStringAttribute), false) as EnumStringAttribute[] ??
                    Array.Empty<EnumStringAttribute>();
            return attribs.Length > 0 ? attribs[0].StringValue : value.ToString();
        }

        public static IEnumerable<T> GetEnumValues<T>(IEnumerable<string> values) where T : struct, Enum
        {
            var enums = Enum.GetValues<T>();
            return enums.Where(e => values.Contains(e.GetStringValue()));
        }
    }
}
