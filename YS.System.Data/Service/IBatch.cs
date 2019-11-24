using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public interface IBatch<EntityType>
    {
        Task<ResultData<int>> BatchOperation(BatchData<EntityType> batchInfos);
    }
    public class BatchData<EntityType>
    {
        public EntityType[] Inserts { get; set; }
        public EntityType[] Updates { get; set; }
        public EntityType[] Deletes { get; set; }
    }
}
