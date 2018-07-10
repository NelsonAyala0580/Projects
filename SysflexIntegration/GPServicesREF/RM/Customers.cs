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
    public class Customers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="parent"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public Response CreateUpdateCustomer(RMCustomer customer, RMParentID parent, string company)
        {
            var gpservices = new RMTransactionClient();
            Response response;

            RMParentIDChild[] children = new RMParentIDChild[0];
            response =  gpservices.CreateUpdateCustomer(customer, parent, children, company);

            gpservices.Close();

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="children"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public Response CreateUpdatePolicy (RMCustomer customer, RMParentIDChild[] children, string company)
        {
            var gpservices = new RMTransactionClient();
            Response response;

            response = gpservices.CreateUpdateCustomer(customer, new RMParentID(), children, company);

            gpservices.Close();

            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="custclass"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public Response CreateCustomerClass (RMCustomerClass custclass, string company)
        {
            var gpservices = new RMTransactionClient();
            Response response;

            response = gpservices.CreateCustomerClass(custclass, company);
            gpservices.Close();

            return response;
        }
    }
}
