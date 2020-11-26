using System;
using System.Collections.Generic;

namespace YS.Knife.Data
{
    [Serializable]
    public class ApiResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> ErrorData { get; set; }

        public void Assert()
        {
            if (Code != default)
            {
                var exception = new KnifeException(Code, Message);
                if (ErrorData != null)
                {
                    foreach (var kv in ErrorData)
                    {
                        exception.Data[kv.Key] = kv.Value;
                    }
                }
                throw exception;
            }
        }
    }
    [Serializable]
    public class ApiResult<T> : ApiResult
    {
        public T Result { get; set; }

        public T Extract()
        {
            this.Assert();
            return this.Result;
        }
    }


}
