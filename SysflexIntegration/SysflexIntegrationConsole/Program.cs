using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysflexIntegrationConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            ///Sync Invoices
            var invoiceTask = new InvoiceSync.InvoiceSyncTask();
            invoiceTask.Process();

        }
    }
}
