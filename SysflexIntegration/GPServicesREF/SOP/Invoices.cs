using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPClass;
using eConnectIntegration;
using eConnectIntegration.CLASS;
using Microsoft.Dynamics.GP.eConnect;

namespace GPServicesREF.SOP
{
    public class Invoices
    {
        public Response CreateInvoice(SOPHeader header, SOPDetail[] detail, SOPDistribution[] distribution, SOPCommissions[] commissions, SOPTax[] tax, SopType soptype, string company)
        {
            var gpservices = new SOPTransactionClient();
            Response response;
            response = gpservices.CreateSOPTransaction(header, detail, distribution, commissions, tax, soptype, company);
            gpservices.Close();

            return response;
        }
    }
}
