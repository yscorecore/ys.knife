using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity
{
    public interface IEntitySeedData<T>
    {
        IEnumerable<T> GetSeedData();
    }
}
