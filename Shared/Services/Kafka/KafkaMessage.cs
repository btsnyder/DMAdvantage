using System.Text;

namespace DMAdvantage.Shared.Services.Kafka
{
    public class KafkaMessage
    {
        public string Topic { get; set; }
        public string User { get; set; }
        public string Value { get; set; }

        public byte[] Serialize()
        {
            return Encoding.ASCII.GetBytes($"{Topic}|{User}|{Value}");
        }

        public static KafkaMessage Deserialize(string data)
        {
            var split = data.TrimEnd('\0').Split('|');
            return new KafkaMessage
            { 
                Topic = split[0], 
                User = split[1], 
                Value = split[2] 
            };
        }
    }
}
