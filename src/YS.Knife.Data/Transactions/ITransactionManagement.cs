using System;
using System.Collections.Generic;


namespace YS.Knife.Data.Transactions
{
    public interface ITransactionManagement
    {
        bool StartTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void ResetTransaction();
    }
}