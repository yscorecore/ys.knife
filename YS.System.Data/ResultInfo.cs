using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data
{
    [Serializable]
    public class ResultInfo
    {
       
        public static implicit operator bool (ResultInfo b)
        {
            if (b == null) return false;
            return b.Success;
        }
        /// <summary>
        /// 获取或设置一个值，该值表示是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 获取或设置附加消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 获取或设置消息的代码
        /// </summary>
        public int Code { get; set; }

        public ResultData<T> WrapData<T>(T data)
        {
            return new ResultData<T>()
            {
                Success = this.Success,
                Data = data,
                Message = this.Message,
                Code = this.Code
            };
        }
    }

    [Serializable]
    public class ResultData<T> : ResultInfo
    {
        /// <summary>
        /// 获取或设置附加数据
        /// </summary>
        public T Data { get; set; }

        public ResultInfo UnWrapData()
        {
            return new ResultInfo()
            {
                Message = this.Message,
                Code = this.Code,
                Success = this.Success
            };
        }
      
    }


}
