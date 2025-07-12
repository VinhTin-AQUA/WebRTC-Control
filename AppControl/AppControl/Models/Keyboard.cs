using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppControl.Models
{
    public class Keyboard
    {
        [JsonConverter(typeof(HexToUShortConverter))]
        public ushort KeyCode { get; set; }
    }

    public class HexToUShortConverter : JsonConverter<ushort>
    {
        public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? hexString = reader.GetString();

            if (string.IsNullOrEmpty(hexString))
            {
                return 0;
            }

            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToUInt16(hexString, 16);
            }

            return ushort.Parse(hexString);
        }

        public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("0x" + value.ToString("X"));
        }
    }
}
