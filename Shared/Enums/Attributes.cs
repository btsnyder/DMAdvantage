namespace DMAdvantage.Shared.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute(string stringValue)
        {
            this.stringValue = stringValue;
        }
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
