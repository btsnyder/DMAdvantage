namespace DMAdvantage.Shared.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute(string stringValue)
        {
            StringValue = stringValue;
        }

        public string StringValue { get; set; }
    }
}
