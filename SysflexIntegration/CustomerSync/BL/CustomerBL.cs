using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using CustomerSync.DAL;
using RMClass;
using eConnectIntegration.CLASS;
using GPServicesREF.RM;
using Log4NetMR;

namespace CustomerSync.BL
{
    public class CustomerBL
    {
        classClsRegistrarLog log = new classClsRegistrarLog();
        public List<SYSFLEX_CUSTOMER> GetCustomer()
        {

            var customerlist = new CustomerDAL();
            return customerlist.GetCustomer();

        }

        public Response CreateUpdateCustomer(RMCustomer customer, RMParentID parent, string company)
        {

            var cust = new Customers();
            return cust.CreateUpdateCustomer(customer, parent, company);

        }

        public int Customer_UpdateLastUpdate(ST_CLIENT_TABLE_SYNC customer)
        {

            var customerlist = new CustomerDAL();
            return customerlist.Customer_UpdateLastUpdate(customer);

        }
    }
}