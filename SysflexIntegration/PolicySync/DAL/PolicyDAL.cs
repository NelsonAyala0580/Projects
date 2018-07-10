using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using RMClass;
using eConnectIntegration.CLASS;
using GPServicesREF.RM;
using Log4NetMR;


namespace PolicySync.DAL
{
    public class PolicyDAL
    {
        classClsRegistrarLog log = new classClsRegistrarLog();

        /// <summary>
        /// Get Sysflex Policy
        /// </summary>
        /// <returns></returns>
        public List<SYSFLEX_POLICY> GetPolicy()
        { 
            var contexto = new ATLANEntities();
            List<SYSFLEX_POLICY> policyList;
            try
            {
                policyList = contexto.usp_ST_SYSFLEX_CUSTOMER_GET_POLICY().ToList();
               
                return policyList;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custnmbr"></param>
        /// <returns></returns>
        public  SYSFLEX_CUSTOMER GetPolicyParent(int custnmbr)
        {
            var contexto = new ATLANEntities();
            SYSFLEX_CUSTOMER policyParent;
            try
            {
                policyParent = contexto.usp_ST_SYSFLEX_POLICY_GET_PARENT(custnmbr).FirstOrDefault();
                return policyParent;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Get Customer Class
        /// </summary>
        /// <param name="line_of_business"></param>
        /// <param name="policy_type"></param>
        /// <param name="vehicle_type"></param>
        /// <param name="zone"></param>
        /// <param name="country"></param>
        /// <param name="channel"></param>
        /// <param name="supervisor"></param>
        /// <param name="GPProductID"></param>
        /// <returns></returns>
        public string GetCustomerClass(string line_of_business, string policy_type, string vehicle_type, string zone,string country, string channel,string supervisor,string GPProductID)
        {
            var contexto = new ATLANEntities();
            var custclass = string.Empty;
            try
            {
                custclass = contexto.usp_ST_SYSFLEX_CUSTOMER_GET_CLASS(line_of_business, policy_type, vehicle_type, zone, country, channel, supervisor, GPProductID).FirstOrDefault();

                return custclass;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Check Customer Class Exists
        /// </summary>
        /// <param name="custclass"></param>
        /// <returns></returns>
        public bool CheckCustomerClassExists(string custclass)
        {
            var contexto = new ATLANEntities();
            try
            {
                var counter = (from customerclas in contexto.RM00201
                              where customerclas.CLASSID.Trim() == custclass.Trim()
                              select custclass).Count();

                if (counter > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }  
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }

            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// Get GP Customer
        /// </summary>
        /// <param name="custnmbr"></param>
        /// <returns></returns>
        public RM00101 GetGPCustomer(string custnmbr)
        {
            var contexto = new ATLANEntities();
            RM00101 customer;

            try
            {
                customer = (from customers in contexto.RM00101
                            where customers.CUSTNMBR.Contains(custnmbr)
                            select customers).FirstOrDefault();
                
                return customer;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        public RM00301 GetGPSalesPerson(string salesnumber)
        {
            var contexto = new ATLANEntities();
            RM00301 salesperson;

            try
            {
                salesperson = (from salespersons in contexto.RM00301
                               where salespersons.SLPRSNID.Trim() == salesnumber.Trim()
                               select salespersons).FirstOrDefault();

                return salesperson;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CUSTNMBR"></param>
        /// <param name="CPRCSTNM"></param>
        /// <returns></returns>
        public bool CheckCustomerExists(string CUSTNMBR, string CPRCSTNM)
        {
            var contexto = new ATLANEntities();
            try
            {
                var conteo = (from customers in contexto.RM00101
                              where customers.CUSTNMBR.Trim() == CUSTNMBR.Trim() && customers.CPRCSTNM.Trim() == CPRCSTNM.Trim()
                              select customers).Count();
                if (conteo > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

    }
}
