using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    public static class ResultInfos
    {

        public readonly static ResultInfo True = new ResultInfo { Message = string.Empty, Code = 0, Success = true };

        public readonly static ResultInfo False = new ResultInfo { Message = string.Empty, Code = -1, Success = true };

        public readonly static ResultInfo Success = new ResultInfo { Message = string.Empty, Code=0, Success = true };

        public readonly static ResultInfo UnknowFailure = new ResultInfo { Message = string.Empty, Code=99999, Success = false };

        public readonly static ResultInfo ExceptionFailure= new ResultInfo { Message = string.Empty, Code = 10009, Success = false };

        public readonly static ResultInfo NotFound = new ResultInfo { Message = nameof(NotFound), Code=10001, Success = false };

        public readonly static ResultInfo OperaterFailure = new ResultInfo { Message = nameof(OperaterFailure), Code = 10002, Success = false };
    }
}
