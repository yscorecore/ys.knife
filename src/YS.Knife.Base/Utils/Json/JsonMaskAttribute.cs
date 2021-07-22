namespace System.Text.Json.Serialization
{
    public class JsonMaskAttribute : JsonConverterAttribute
    {
        public char MaskChar { get; set; }
        public int MaskLength { get; set; }

        public JsonMaskAttribute(int maskLength = 6, char maskChar = '*')
        {
            this.MaskLength = maskLength;
            this.MaskChar = maskChar;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            var type = typeof(MaskPropertyConverter<>).MakeGenericType(typeToConvert);
            return Activator.CreateInstance(type, new object[] { MaskLength, MaskChar }) as JsonConverter;
        }

        class MaskPropertyConverter<T> : JsonConverter<T>
        {
            public MaskPropertyConverter(int maskLength, char maskChar)
            {
                this.MaskLength = maskLength;
                this.MaskChar = maskChar;
            }
            public char MaskChar { get; set; }
            public int MaskLength { get; set; }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (typeof(string) == typeof(T))
                {
                    return (T)(object)reader.GetString();
                }
                return default;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                string maskString = MaskLength > 0 ? new string(MaskChar, MaskLength) : string.Empty;
                writer.WriteStringValue(maskString);
            }
        }
    }
}
