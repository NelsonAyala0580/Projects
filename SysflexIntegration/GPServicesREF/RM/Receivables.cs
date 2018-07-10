using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMClass;
using eConnectIntegration.CLASS;
using eConnectIntegration;

namespace GPServicesREF.RM
{
    public class Receivables
    {
        public Response CreateRMInvoice(RMTransactionHeader Header,  RMTransactionDist[] Distrib, RMDocumentTaxes[] Taxes, string company)
        {
            var gpservices = new RMTransactionClient();
            Response response;
            response = gpservices.CreateRMTransaction(Header, Distrib, Taxes, company);
            gpservices.Close();

            return response;
        }
    }
}
