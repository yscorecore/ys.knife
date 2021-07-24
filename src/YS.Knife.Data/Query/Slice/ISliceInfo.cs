using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Query
{
    public interface ISliceInfo<TCursor>
    {
        TCursor Start { get; }

        int Length { get; }
    }
}
