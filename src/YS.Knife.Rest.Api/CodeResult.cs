using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Api
{
    public class CodeResult
    {

        public string Code { get; set; }
        public string Message { get; set; }
        public static CodeResult FromCode(string code, string message)
        {
            return new CodeResult
            {
                Code = code,
                Message = message
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
        public static CodeResult<T> FromData<T>(T data)
        {
            return FromData("0", "success", data);
        }
    }

    public class CodeResult<T> : CodeResult
    {

        public T Data { get; set; }
    }
}
