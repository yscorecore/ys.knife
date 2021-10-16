namespace System.Text.Json.Serialization
{
    public class JsonMaskAttribute : JsonConverterAttribute
    {

        public object MaskValue { get; private set; }

        public JsonMaskAttribute(object maskValue)
        {
            this.MaskValue = maskValue;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            var type = typeof(MaskPropertyConverter<>).MakeGenericType(typeToConvert);
            var converter = ComponentModel.TypeDescriptor.GetConverter(typeToConvert);
            var targetValue = converter.ConvertTo(MaskValue, typeToConvert);
            return Activator.CreateInstance(type, new object[] { targetValue }) as JsonConverter;
        }
        class MaskPropertyConverter<T> : JsonConverter<T>
        {
            public T MaskValue { get; private set; }
            public MaskPropertyConverter(T maskValue)
            {
                this.MaskValue = maskValue;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;
                return converter.Read(ref reader, typeToConvert, options);
            }


            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;
                converter.Write(writer, this.MaskValue, options);
            }
        }
    }
}
