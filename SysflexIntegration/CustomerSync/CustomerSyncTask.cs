using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerSync.BL;
using MODEL;
using RMClass;
using SysflexIntegrationUTIL;
using Log4NetMR;


namespace CustomerSync
{
    public class CustomerSyncTask
    {
        /// <summary>
        /// clase para registrar eventos
        /// </summary>
        classClsRegistrarLog log = new classClsRegistrarLog();

        /// <summary>
        /// Procesar Customers
        /// </summary>
        public void Process()
        {
            var customer = new CustomerBL();
            string integrationid = ConfigKey.ReadSetting("INTEGRATIONID");
            List<SYSFLEX_CUSTOMER> customerlist = customer.GetCustomer();
            var company = ConfigKey.ReadSetting("Company");

            eConnectIntegration.CLASS.Response response;

            try
            {
                foreach (SYSFLEX_CUSTOMER cust in customerlist)
                {
                    try
                    {

                        log.escribirLog(0, "Sincronización de Cliente " + cust.CUSTNMBR);
                        var rmcustomer = new RMCustomer();

                        if (string.IsNullOrEmpty(cust.CUSTNAME))
                        {
                            cust.CUSTNAME = "";
                        }

                        if (string.IsNullOrEmpty(cust.SHRTNAME))
                        {
                            cust.SHRTNAME = "";
                        }

                        if (string.IsNullOrEmpty(cust.STMTNAME))
                        {
                            cust.STMTNAME = "";
                           }

                        if (string.IsNullOrEmpty(cust.CUSTCLAS))
                        {
                            cust.CUSTCLAS = "";
                        }

                        if (string.IsNullOrEmpty(cust.CNTCPRSN))
                        {
                            cust.CNTCPRSN = "";
                        }

                        if (string.IsNullOrEmpty(cust.ADDRESS1))
                        {
                            cust.ADDRESS1 = "";
                        }

                        if (string.IsNullOrEmpty(cust.CITY))
                        {
                            cust.CITY = "";
                        }

                        if (string.IsNullOrEmpty(cust.COUNTRY))
                        {
                            cust.COUNTRY = "";
                        }

                        if (string.IsNullOrEmpty(cust.PHNUMBR1))
                        {
                            cust.PHNUMBR1 = "";
                        }

                        if (string.IsNullOrEmpty(cust.PHNUMBR2))
                        {
                            cust.PHNUMBR2 = "";
                        }

                        if (string.IsNullOrEmpty(cust.PHNUMBR3))
                        {
                            cust.PHNUMBR3 = "";
                        }

                        if (string.IsNullOrEmpty(cust.fax))
                        {
                            cust.fax = "";
                        }

                        rmcustomer.CUSTNMBR = cust.CUSTNMBR.ToString();
                        //rmcustomer.HOLD = cust.HOLD.GetValueOrDefault();
                        rmcustomer.INACTIVE = cust.INACTIVE.GetValueOrDefault();
                        rmcustomer.CUSTNAME = cust.CUSTNAME.Trim() + " " + cust.SHRTNAME.Trim() + " " + cust.STMTNAME.Trim();
                        rmcustomer.SHRTNAME = cust.CUSTNAME.Trim() + " " + cust.STMTNAME;
                        rmcustomer.STMTNAME = cust.CUSTNAME.Trim() + " " + cust.STMTNAME;
                        rmcustomer.CUSTCLASS = cust.CUSTCLAS;
                        rmcustomer.CNTCPRSN = cust.CNTCPRSN;
                        rmcustomer.ADRSCODE = "PRIMARY        ";
                        rmcustomer.ADDRESS1 = cust.ADDRESS1;
                                                   rmcustomer.ADDRESS2 = cust.ADDRESS2.GetValueOrDefault().ToString();
                        rmcustomer.ADDRESS3 = cust.ADDRESS3.GetValueOrDefault().ToString();
                        rmcustomer.CITY = cust.CITY;
                        rmcustomer.COUNTRY = cust.COUNTRY;
                        rmcustomer.PHNUMBR1 = cust.PHNUMBR1;
                        rmcustomer.PHNUMBR2 = cust.PHNUMBR2;
                        rmcustomer.PHNUMBR3 = cust.PHNUMBR3;
                        rmcustomer.FAX = cust.fax;
                        rmcustomer.INTEGRATIONID = integrationid;
                        rmcustomer.CreateAddress = 1;
                        rmcustomer.UpdateIfExists = 1;

                        var rmparent = new RMParentID();
                        rmparent.CPRCSTNM = cust.CUSTNMBR.ToString();
                        rmparent.UpdateIfExists = 1;
                        rmparent.NAALLOWRECEIPTS = 1;

                        response = customer.CreateUpdateCustomer(rmcustomer, rmparent, company);

                        if (response.SUCCESS)
                        {
                            var customerupdate = new ST_CLIENT_TABLE_SYNC();
                            customerupdate.CUSTNMBR = rmcustomer.CUSTNMBR;
                            customer.Customer_UpdateLastUpdate(customerupdate);
                            log.escribirLog(0, response.MESSAGE);
                        }
                        else
                        {
                            log.escribirLog(1, response.MESSAGE);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.escribirLog(2, "Ocurrió un error con el customer " + cust.CUSTNMBR);
                        log.LogExeption("Detalle del error: ", 2, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                //throw;
            }

        }
    }
}
