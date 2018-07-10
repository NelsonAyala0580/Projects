using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using Log4NetMR;


namespace CustomerSync.DAL
{

    public class CustomerDAL
    {
        /// <summary>
        /// Clase para registrar eventos
        /// </summary>
        classClsRegistrarLog log = new classClsRegistrarLog();

        /// <summary>
        /// Devuelve listado de clientes
        /// </summary>
        /// <returns></returns>
        public List<SYSFLEX_CUSTOMER> GetCustomer()
        {
            var contexto = new ATLANEntities();
            List<SYSFLEX_CUSTOMER> customerlist;
            try
            {
                customerlist = contexto.usp_ST_SYSFLEX_CUSTOMER_GET_PARENTS().ToList();

                return customerlist;
            }
            catch(EntityException ex)
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
        /// Guarda la ultima actualización sobre el customer
        /// </summary>
        /// <param name="customer">codigo de cliente</param>
        /// <returns></returns>
        public int Customer_UpdateLastUpdate(ST_CLIENT_TABLE_SYNC customer)
        {
            var contexto = new ATLANEntities();
            int updated;
            try
            {
                var customerORI = contexto.ST_CLIENT_TABLE_SYNC.Find(customer.CUSTNMBR);
                //customerORI.LAST_GP_UPDATE = DateTime.Now;


                if (customerORI != null)
                {
                    contexto.ST_CLIENT_TABLE_SYNC.Attach(customerORI);
                    contexto.Entry(customerORI).State = System.Data.Entity.EntityState.Modified;
                    contexto.Entry(customerORI).Property(x => x.LAST_GP_UPDATE).IsModified = true;
                    updated = contexto.SaveChanges();
                    log.escribirLog(0, "Customer Actualizado " + customer.CUSTNMBR);
                }
                else
                {
                    customerORI = new ST_CLIENT_TABLE_SYNC();
                    customerORI.CUSTNMBR = customer.CUSTNMBR;
                    customerORI.LAST_GP_UPDATE = DateTime.Now;
                    contexto.ST_CLIENT_TABLE_SYNC.Add(customerORI);
                    updated = contexto.SaveChanges();
                    log.escribirLog(0, "Customer Agregado " + customer.CUSTNMBR);
                }

                return updated;
            }
            catch (EntityException ex)
            {
                log.LogExeption("Ocurrió un error: EntityException ", 2, ex);
                throw;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: Exception", 2, ex);
                throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }
    }
}
