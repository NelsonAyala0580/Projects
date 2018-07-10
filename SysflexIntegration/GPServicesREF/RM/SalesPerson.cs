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
    public class SalesPerson
    {
        public Response CreateUpdateSalesPerson(RMSalesPerson salesperson,string company)
        {
            var gpservices = new RMTransactionClient();
            Response response = gpservices.CreateSalesPerson(salesperson, company);
            gpservices.Close();

            return response;
        }
    }
}
