using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using PolicySync.DAL;
using RMClass;
using eConnectIntegration.CLASS;
using GPServicesREF.RM;
using SysflexIntegrationUTIL;
using Log4NetMR;


namespace PolicySync.BL
{
    public class PolicyBL
    {
        classClsRegistrarLog log = new classClsRegistrarLog();
        public List<SYSFLEX_POLICY> GetPolicy()
        {
            var policyList = new PolicyDAL();
            return policyList.GetPolicy();
        }

        public Response CreateCustomer(RMCustomer customer, RMParentIDChild[] children, string company)
        {
            var cust = new Customers();
            return cust.CreateUpdatePolicy(customer, children, company);
        }

        public SYSFLEX_CUSTOMER GetPolicyParent(int custnmbr)
        {
            var cust = new PolicyDAL();
            return cust.GetPolicyParent(custnmbr);
        }

        public ST_CUSTOMER_ADDITIONAL SetCustomerAdditional(SYSFLEX_POLICY pol)
        {
            var custAdd = new ST_CUSTOMER_ADDITIONAL();

            try
            {
                custAdd.CUSTNMBR = pol.POLICY_NUMBER;
                custAdd.ZONE = pol.ZONE;
                custAdd.VEHICLE_TYPE = pol.VEHICLE_TYPE;
                custAdd.POLICY_TYPE = pol.POLICY_TYPE;
                custAdd.COUNT_VEHICLE = pol.COUNT_VEHICLE;
                custAdd.INITAL_DATE = pol.INITAL_DATE;
                custAdd.DUE_DATE = pol.DUE_DATE;
                custAdd.CLIENT_ID = Convert.ToInt32(pol.CLIENT_ID.GetValueOrDefault());
                custAdd.SUPERVISOR = pol.SUPERVISOR;
                custAdd.AGENT_ID = Convert.ToInt16(pol.AGENT_ID.GetValueOrDefault());
                custAdd.AGENT_CODE = pol.AGENT_CODE.ToString();
                custAdd.AGENT = pol.AGENT;
                custAdd.MPV_total_bruto = pol.MPV_total_bruto.GetValueOrDefault();
                custAdd.MPV_Descuento_flota = pol.MPV_Descuento_flota;
                custAdd.MPV_Descuento_Experiencia = pol.MPV_Descuento_Experiencia.GetValueOrDefault();
                custAdd.MPV_ITBIS = pol.MPV_ITBIS.GetValueOrDefault();
                custAdd.POLICY_STATUS = pol.POLICY_STATUS;
                custAdd.CREATED_USER = pol.CREATED_USER;
                custAdd.MODIFIED_USER = pol.MODIFIED_USER;
                custAdd.COMMENT = pol.COMMENT;
                custAdd.CHANNEL = pol.CHANNEL;
                custAdd.CORE_CREATION_DATE = pol.CREATE_DATE;

                return custAdd;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        public string GetCustomerClass(string line_of_business, string policy_type,
                               string vehicle_type, string zone,
                               string country, string channel,
                               string supervisor, string GPProductID)
        {
            var policydal = new PolicyDAL();
            return policydal.GetCustomerClass(line_of_business, policy_type, vehicle_type, zone, country, channel, supervisor, GPProductID);
        }

        public bool CheckCustomerClassExists(string custclass)
        {
            var policydal = new PolicyDAL();

            return policydal.CheckCustomerClassExists(custclass);
        }

        public Response CreateCustomerClass(string line_of_business, string policy_type, string vehicle_type, string zone,
                                            string country, string channel, string supervisor, int supervisor_code, string custclass, string GpProductID)
        {
            RMCustomerClass rmcustclass = new RMCustomerClass();
            AccountBuilder aBuilder = new AccountBuilder();
            Response response;
            Customers custom = new Customers();

            string taxid = ConfigKey.ReadSetting("TAXID");
            string accountREC = ConfigKey.ReadSetting("IDREC");
            string company = ConfigKey.ReadSetting("Company");
            
            try
            {


                rmcustclass.CLASSID = custclass;
                rmcustclass.CLASDSCR = custclass;
                rmcustclass.CRLMTTYP = 1;
                rmcustclass.TAXSCHID = taxid;
                rmcustclass.STMTCYCL = 5;
                rmcustclass.CUSTPRIORITY = 1;
                rmcustclass.ORDERFULFILLDEFAULT = 1;
                rmcustclass.ACCTRECACCT = aBuilder.BuildARAccount(Convert.ToInt32(accountREC), line_of_business, policy_type, vehicle_type, country, channel, supervisor, supervisor_code, GpProductID);

                response = custom.CreateCustomerClass(rmcustclass, company);
                return response;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }
        public bool CheckCustomerExists(string CUSTNMBR, string CPRCSTNM)
        {
            var policydal = new PolicyDAL();

            return policydal.CheckCustomerExists(CUSTNMBR, CPRCSTNM);
        }
    }
}
