using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    public interface ILimitInfo
    {
        int Offset { get; }
        int Limit { get; }
    }
}
