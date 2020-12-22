using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json.Serialization
{

    public class JsonMaskAttribute : JsonConverterAttribute
    {
        public char MaskChar { get; set; } = '*';
        public int MaskLength { get; set; } = 6;

        public JsonMaskAttribute(int maskLength = 6, char maskChar = '*') : base(typeof(MaskPropertyConverter))
        {
            this.MaskLength = maskLength;
            this.MaskChar = maskChar;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return new MaskPropertyConverter { MaskChar = MaskChar, MaskLength = MaskLength };
        }
    }

    public class MaskPropertyConverter : JsonConverter<string>
    {
        public char MaskChar { get; set; } = '*';
        public int MaskLength { get; set; }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            string maskString = MaskLength > 0 ? new string(MaskChar, MaskLength) : string.Empty;
            writer.WriteStringValue(maskString);
        }
    }
}
