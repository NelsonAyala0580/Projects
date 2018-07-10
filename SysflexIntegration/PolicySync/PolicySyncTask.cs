using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolicySync.BL;
using MODEL;
using RMClass;
using SysflexIntegrationUTIL;
using Log4NetMR;

namespace PolicySync
{
    public class PolicySyncTask
    {
        classClsRegistrarLog log = new classClsRegistrarLog();
        public void Process()
        {
            var policy = new PolicyBL();
            List<SYSFLEX_POLICY> policyList = policy.GetPolicy();
            string company = ConfigKey.ReadSetting("Company");
            string integrationid = ConfigKey.ReadSetting("INTEGRATIONID");
            eConnectIntegration.CLASS.Response response;

            try
            {
                foreach (SYSFLEX_POLICY pol in policyList)
                {
                    log.WriteLog(0, "Inicia sincronización de Poliza: " + pol.POLICY_NUMBER.Trim());
                    var custAdd = policy.SetCustomerAdditional(pol);
                    var syscustomer = policy.GetPolicyParent(Convert.ToInt32(pol.CLIENT_ID));
                    var customerexists = policy.CheckCustomerExists(pol.POLICY_NUMBER, syscustomer.CUSTNMBR.ToString());
                    try
                    {
                        var custclass = policy.GetCustomerClass(pol.LINE_OF_BUSINESS.Trim(), pol.POLICY_TYPE, pol.VEHICLE_TYPE, pol.ZONE, syscustomer.COUNTRY.Trim(), pol.CHANNEL, pol.SUPERVISOR,pol.GpProductID);
                        var classexists = policy.CheckCustomerClassExists(custclass);
                        if (!classexists)
                        {
                            response = policy.CreateCustomerClass(pol.LINE_OF_BUSINESS.Trim(),
                                                                  pol.POLICY_TYPE.Trim(),
                                                                  pol.VEHICLE_TYPE.Trim(),
                                                                  pol.ZONE.Trim(),
                                                                  syscustomer.COUNTRY.Trim(),
                                                                  pol.CHANNEL.Trim(),
                                                                  pol.SUPERVISOR.Trim(),
                                                                  pol.SUPERVISOR_CODE.GetValueOrDefault(), custclass, pol.GpProductID);

                            if (response.SUCCESS)
                            {
                                log.WriteLog(0, "Clase creada: " + custclass);
                            }
                            else
                            {
                                log.WriteLog(2, "Ocurrio un error al crear la clase: " + custclass + ". " + response.MESSAGE);
                            }

                        }
                        var gpcustomer = new RMClass.RMCustomer();

                        if (string.IsNullOrEmpty(syscustomer.CUSTNAME))
                        {
                            syscustomer.CUSTNAME = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.SHRTNAME))
                        {
                            syscustomer.SHRTNAME = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.STMTNAME))
                        {
                            syscustomer.STMTNAME = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.CNTCPRSN))
                        {
                            syscustomer.CNTCPRSN = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.ADDRESS1))
                        {
                            syscustomer.ADDRESS1 = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.CITY))
                        {
                            syscustomer.CITY = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.COUNTRY))
                        {
                            syscustomer.COUNTRY = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.PHNUMBR1))
                        {
                            syscustomer.PHNUMBR1 = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.PHNUMBR2))
                        {
                            syscustomer.PHNUMBR2 = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.PHNUMBR3))
                        {
                            syscustomer.PHNUMBR3 = "";
                        }

                        if (string.IsNullOrEmpty(syscustomer.fax))
                        {
                            syscustomer.fax = "";
                        }

                        gpcustomer.CUSTNMBR = pol.POLICY_NUMBER;
                        gpcustomer.CUSTNAME = syscustomer.CUSTNAME.Trim() + " " + syscustomer.SHRTNAME.Trim() + " " + syscustomer.STMTNAME.Trim();
                        gpcustomer.SHRTNAME = syscustomer.CUSTNAME.Trim() + " " + syscustomer.STMTNAME;
                        gpcustomer.STMTNAME = syscustomer.CUSTNAME.Trim() + " " + syscustomer.STMTNAME;
                        gpcustomer.CUSTCLASS = custclass;
                        gpcustomer.CNTCPRSN = syscustomer.CNTCPRSN;
                        gpcustomer.ADRSCODE = "PRIMARY        ";
                        gpcustomer.ADDRESS1 = syscustomer.ADDRESS1;
                        gpcustomer.ADDRESS2 = syscustomer.ADDRESS2.GetValueOrDefault().ToString();
                        gpcustomer.ADDRESS3 = syscustomer.ADDRESS3.GetValueOrDefault().ToString();
                        gpcustomer.CITY = syscustomer.CITY;
                        gpcustomer.COUNTRY = syscustomer.COUNTRY;
                        gpcustomer.PHNUMBR1 = syscustomer.PHNUMBR1;
                        gpcustomer.PHNUMBR2 = syscustomer.PHNUMBR2;
                        gpcustomer.PHNUMBR3 = syscustomer.PHNUMBR3;
                        gpcustomer.FAX = syscustomer.fax;
                        gpcustomer.INTEGRATIONID = integrationid;
                        gpcustomer.CreateAddress = 1;
                        gpcustomer.UpdateIfExists = 1;
                        gpcustomer.UseCustomerClass = 1;
                        gpcustomer.SLPRSNID = pol.AGENT_CODE.ToString();

                        var rmchildren = new RMParentIDChild();
                        List<RMParentIDChild> children = new List<RMParentIDChild>();
                        if (!customerexists)
                        {

                            rmchildren.CPRCSTNM = syscustomer.CUSTNMBR.ToString();
                            rmchildren.CUSTNMBR = pol.POLICY_NUMBER.Trim();
                            children.Add(rmchildren);
                        }
                         

                        response = policy.CreateCustomer(gpcustomer, children.ToArray(), company);

                        if (response.SUCCESS)
                        {
                            log.WriteLog(0, "Poliza sincronizada correctamente: " + pol.POLICY_NUMBER.Trim());
                        }
                        else
                        {
                            log.WriteLog(2, "Ocurrió un error al sincronizar la póliza: " + pol.POLICY_NUMBER.Trim() + ". " + response.MESSAGE);
                        }
                        //log.escribirLog(0, "Finaliza la sincronización de Poliza: " + pol.POLICY_NUMBER.Trim());
                    }
                    catch (Exception ex)
                    {
                        log.LogExeption("Error al sincronizar la poliza: " + pol.POLICY_NUMBER.Trim(), 2, ex);
                    }

                }
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                ///throw;
            }
        }
    }
}
