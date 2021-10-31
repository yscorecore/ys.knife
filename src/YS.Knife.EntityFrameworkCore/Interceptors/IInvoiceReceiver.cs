using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.EntityFrameworkCore.Interceptors
{
    public interface IInvoiceReceiver
    {
        void ReceiveInvoices(ReadOnlyCollection<InvoiceModel> invoiceModels);
    }
}
