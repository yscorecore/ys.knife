using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MapsterMapper;
using Mapster;

namespace YS.Knife.Mapper
{
   public static class QueryableExtenstions
    {
        static QueryableExtenstions()
        {
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        }
    }
}
