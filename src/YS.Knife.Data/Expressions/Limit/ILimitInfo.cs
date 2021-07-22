using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Expressions
{
    public interface ILimitInfo
    {
        int Offset { get; }
        int Limit { get; }
    }
}
