using System.Collections.Generic;
using System.ComponentModel;

namespace YS.Knife.Data
{
    public class ApiResult
    {
        public string Code { get; set; }
        public string Message{get;set;}
    }
    public class ApiResult<T>:ApiResult
    {
        public T Result{get;set;}

    }


}