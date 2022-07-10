using System.Collections.Generic;

namespace YS.Knife
{
    public class CodeResult
    {
        public Dictionary<string, object> Errors { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public static CodeResult FromCode(string code, string message, System.Collections.IDictionary errors = null)
        {
            return new CodeResult
            {
                Code = code,
                Message = message,
                Errors = ConvertToStringDictionary(errors)
            };
        }

        public static CodeResult<T> FromData<T>(string code, string message, T data)
        {
            return new CodeResult<T>
            {
                Code = code,
                Message = message,
                Data = data
            };
        }
        public static CodeResult FromCodeException(CodeException codeException)
        {
            return new CodeResult
            {
                Code = codeException.Code,
                Message = codeException.Message,
                Errors = ConvertToStringDictionary(codeException.Data)
            };
        }

        static Dictionary<string, object> ConvertToStringDictionary(System.Collections.IDictionary dictionary)
        {
            if (dictionary == null) return null;
            if (dictionary is Dictionary<string, object> dic)
            {
                return dic;
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (object key in dictionary.Keys)
            {
                result[key?.ToString() ?? ""] = dictionary[key];
            }
            return result;
        }
    }

    public class CodeResult<T> : CodeResult
    {

        public T Data { get; set; }
    }
}
