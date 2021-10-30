using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace YS.Knife.EntityFrameworkCore.Interceptors
{
    public class InvoiceInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var invoiceModels = new List<InvoiceModel>();

            foreach (var entry in
                     eventData.Context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    invoiceModels.Add(new InvoiceModel()
                    {
                        Action = nameof(EntityState.Added),
                        Before = null,
                        After = entry.CurrentValues.ToObject(),
                    });
                }
                else if (entry.State == EntityState.Deleted)
                {
                    invoiceModels.Add(new InvoiceModel()
                    {
                        Action = nameof(EntityState.Deleted),
                        Before = entry.OriginalValues.ToObject(),
                        After = null,
                    });
                }
                else if (entry.State == EntityState.Modified)
                {
                    invoiceModels.Add(new InvoiceModel()
                    {
                        Action = nameof(EntityState.Modified),
                        Before = entry.OriginalValues.ToObject(),
                        After = entry.CurrentValues.ToObject(),
                    });
                }

            }

            var saveChangeResult = base.SavingChanges(eventData, result);
            PublishChangeList(invoiceModels.AsReadOnly());
            return saveChangeResult;
        }
        private void PublishChangeList(ReadOnlyCollection<InvoiceModel> invoiceModels)
        {

        }

    }

    public class InvoiceModel
    {
        public string Action { get; set; }

        public object Before { get; set; }

        public object After { get; set; }
    }


}
