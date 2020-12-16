using System;
using System.Collections.Generic;


namespace YS.Knife.Data.Transactions
{
    public interface ITransactionManagerProvider
    {
        ITransactionManagement GetTransactionManagement();
    }
}