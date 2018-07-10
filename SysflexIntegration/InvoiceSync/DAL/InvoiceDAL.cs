using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Core;
using MODEL;
using Log4NetMR;

namespace InvoiceSync.DAL
{
    public class InvoiceDAL
    {
        classClsRegistrarLog log = new classClsRegistrarLog();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TRANSACTION_INVOICE> GetTransactions()
        {
            var contexto = new ATLANEntities();
            List<TRANSACTION_INVOICE> transactionList;

            //contexto.Database.Connection.Open();

            try
            {
                transactionList = contexto.usp_ST_SYSFLEX_GET_TRANSACTIONS().ToList();
                return transactionList;
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
        /// <param name="policy"></param>
        /// <param name="transaction_secuence"></param>
        /// <returns></returns>
        public List<TRANSACTION_BY_COVERAGE> GetCoverageDistribution(string policy, int transaction_secuence)
        {
            var contexto = new ATLANEntities();
            List<TRANSACTION_BY_COVERAGE> coveragelist;

            try
            {
                contexto.Database.CommandTimeout = 600;
                coveragelist = contexto.usp_ST_SYSFLEX_GET_TRANSACTION_BY_COVERAGE(policy, transaction_secuence).ToList();

                return coveragelist;
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
        /// <param name="policy"></param>
        /// <returns></returns>
        public SYSFLEX_POLICY GetPolicyInfo(string policy)
        {
            var contexto = new ATLANEntities();
            SYSFLEX_POLICY policyInfo = new SYSFLEX_POLICY();

            try
            {
                policyInfo = contexto.usp_ST_SYSFLEX_CUSTOMER_GET_POLICY_INFO(policy.Trim()).FirstOrDefault();
                return policyInfo;
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
        public void InsertTransactionFromSysflex()
        {
            var contexto = new ATLANEntities();

            try
            {
                contexto.Database.CommandTimeout = 2400;
                contexto.usp_ST_SYSFLEX_INSERT_TRANSACTIONS();
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
        /// <param name="product"></param>
        /// <returns></returns>
        public IV00101 GetItemLEY(string product)
        {
            var contexto = new ATLANEntities();
            IV00101 itemnumbr;
            try
            {
                itemnumbr = (from items in contexto.IV00101
                             where items.ITEMNMBR.Contains(product) || (items.ITEMNMBR.Contains(product) && items.ITEMNMBR.Contains("LEY"))
                             select items).FirstOrDefault();

                return itemnumbr;
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
        /// <param name="product"></param>
        /// <returns></returns>
        public IV00101 GetItemPROPIOS(string product)
        {
            var contexto = new ATLANEntities();
            IV00101 itemnumbr;
            try
            {
                itemnumbr = (from items in contexto.IV00101
                             where (items.ITEMNMBR.Contains(product) && items.ITEMNMBR.Contains("PROPIO"))
                             select items).FirstOrDefault();

                return itemnumbr;
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
        /// <param name="product"></param>
        /// <returns></returns>
        public IV00101 GetItemSERVICIOS(string product)
        {
            var contexto = new ATLANEntities();
            IV00101 itemnumbr;
            try
            {
                itemnumbr = (from items in contexto.IV00101
                             where (items.ITEMNMBR.Contains(product) && items.ITEMNMBR.Contains("SERVICIO"))
                             select items).FirstOrDefault();

                return itemnumbr;
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
        /// <param name="salesperson"></param>
        /// <param name="itemnmbr"></param>
        /// <param name="channel"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public decimal GetCommission(string salesperson, string itemnmbr, string channel, decimal amount)
        {
            decimal commissionAmount = 0;
            ST_ITEM_SALESPERSON_COMMISSION conteoSalesPersonItem;
            ST_ITEM_CHANNEL_COMMISSION conteoItemChannel;
            string channelcode = string.Empty;

            var contexto = new ATLANEntities();

            try
            {
                //verifica si existe en item Salesperson
                conteoSalesPersonItem = (from itemsalesperson in contexto.ST_ITEM_SALESPERSON_COMMISSION
                                         where itemsalesperson.SLPRSNID == salesperson && itemsalesperson.ITEMNMBR.Trim() == itemnmbr.Trim()
                                         select itemsalesperson).FirstOrDefault();

                if (conteoSalesPersonItem == null)
                {
                    //channelcode = (from channels in contexto.ST_GL_MAPPING_CHANNEL
                    //               where channels.CHANNEL.Trim() == channel.Trim()
                    //               select channels.GL_MAPPING.Trim()).FirstOrDefault();

                    conteoItemChannel = (from itemchannel in contexto.ST_ITEM_CHANNEL_COMMISSION
                                         where itemchannel.ITEMNMBR.Trim() == itemnmbr.Trim() && itemchannel.CHANNEL_GL_MAPPING.Trim() == channel.Trim()
                                         select itemchannel).FirstOrDefault();
                    if (conteoItemChannel == null)
                    {
                        log.WriteLog(1, "No se encontró configuracion para el canal " + channel.Trim() + " e Itemnumbr: " + itemnmbr.Trim());
                    }
                    commissionAmount = Math.Round(amount * (conteoItemChannel.COMMISSION_PERCENT.GetValueOrDefault() / 100), 2);
                    //commissionAmount = Math.tru (amount * (conteoItemChannel.COMMISSION_PERCENT.GetValueOrDefault() / 100), 2, MidpointRounding.ToEven);
                }
                else
                {
                    commissionAmount = Math.Round(amount * (conteoSalesPersonItem.COMMISSION_PERCENT / 100), 2);
                }
                return commissionAmount;
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
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public List<SYSFLEX_REINSURANCE_BY_COVERAGE> GetReinsuranceByCoverage(string policyNumber, int transaction_secuence)
        {
            var contexto = new ATLANEntities();
            List<SYSFLEX_REINSURANCE_BY_COVERAGE> reinsuranceCoverageList;
            try
            {
                contexto.Database.CommandTimeout = 600;
                reinsuranceCoverageList = contexto.usp_ST_SYSFLEX_GET_REINSURANCE_BY_COVERAGE(policyNumber.Trim(), transaction_secuence).ToList();

                return reinsuranceCoverageList;
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
        /// <param name="doctype"></param>
        /// <param name="module"></param>
        /// <param name="policy_number"></param>
        /// <param name="transaction_Sequence"></param>
        /// <returns></returns>
        public int UpdateTransactionInvoce(int doctype, string module, string policy_number, int transaction_Sequence)
        {
            var contexto = new ATLANEntities();

            int actualizado;
            try
            {
                actualizado = contexto.usp_ST_SYSFLEX_UPDATE_TRANSACTION_SYNC(doctype, module, policy_number, transaction_Sequence);

                return actualizado;
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
        /// <param name="GPProductID"></param>
        /// <returns></returns>
        public IV00101 GetItemNumberByProductCode(string GPProductID)
        {
            var itemnumbr = string.Empty;
            var contexto = new ATLANEntities();
            IV00101 itemGP;

            try
            {
                itemnumbr = (from items in contexto.ST_ITEMNMBR_ACCOUNT_CODE
                             where items.ACCOUNT_CODE == GPProductID.Trim()
                             select items.ITEMNMBR).FirstOrDefault();

                itemGP = (from itemsGP in contexto.IV00101
                          where itemsGP.ITEMNMBR == itemnumbr.Trim()
                          select itemsGP).FirstOrDefault();

                return itemGP;
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
        /// <param name="GPProductID"></param>
        /// <param name="GrupoCobertura"></param>
        /// <returns></returns>
        public IV00101 GetItemNumberByProductCode(string GPProductID, string GrupoCobertura)
        {
            var itemnumbr = string.Empty;
            var contexto = new ATLANEntities();
            IV00101 itemGP;

            try
            {
                itemnumbr = (from items in contexto.ST_ITEMNMBR_ACCOUNT_CODE
                             where items.ACCOUNT_CODE == GPProductID.Trim() && items.ITEMNMBR.Contains(GrupoCobertura)
                             select items.ITEMNMBR).FirstOrDefault();

                itemGP = (from itemsGP in contexto.IV00101
                          where itemsGP.ITEMNMBR == itemnumbr.Trim()
                          select itemsGP).FirstOrDefault();

                return itemGP;
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

        //<SOPTYPE>3</SOPTYPE>
        //<SOPNUMBE>INV2389053</SOPNUMBE>
        //<DISTTYPE>11</DISTTYPE>
        //<DistRef>Comisiones intermediarios</DistRef>
        //<ACTNUMST>5302-06-201-0600-00-05-175-10002-00</ACTNUMST>
        //<DEBITAMT>547.23</DEBITAMT>
        //<CUSTNMBR>1-05-423658    </CUSTNMBR>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="INITIAL_DATE"></param>
        /// <param name="transaction"></param>
        /// <param name="DistributionList"></param>
        public void SaveRenewDistribution(DateTime INITIAL_DATE, TRANSACTION_INVOICE transaction, SOPClass.SOPDistribution[] DistributionList)
        {
            var contexto = new ATLANEntities();
            try
            {
                foreach (SOPClass.SOPDistribution item in DistributionList)
                {
                    var distrib = new ST_SYSFLEX_POLICY_RENEW();

                    distrib.INITIAL_DATE = INITIAL_DATE;
                    distrib.TRANSACTION_SEQ = transaction.TRANSACTION_SEQUENCE.GetValueOrDefault();
                    distrib.DOCUMENT_NUMBER = Convert.ToDecimal(transaction.DOCUMENT_NUMBER);
                    distrib.CUSTNMBR = item.CUSTNMBR;
                    distrib.SOPNUMBE = item.SOPNUMBE;
                    distrib.DISTTYPE = item.DISTTYPE;
                    distrib.DistRef = item.DistRef;
                    distrib.ACTNUMST = item.ACTNUMST;
                    distrib.DEBITAMT = item.DEBITAMT.GetValueOrDefault();
                    distrib.CRDTAMNT = item.CRDTAMNT.GetValueOrDefault();
                    distrib.CREATE_DATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    distrib.PROCESS = false;

                    contexto.ST_SYSFLEX_POLICY_RENEW.Add(distrib);

                    contexto.SaveChanges();
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

        public void SaveRenewDistribution(DateTime INITIAL_DATE, TRANSACTION_INVOICE transaction, RMClass.RMTransactionDist[] DistributionList)
        {
            var contexto = new ATLANEntities();
            try
            {
                foreach (RMClass.RMTransactionDist item in DistributionList)
                {
                    var distrib = new ST_SYSFLEX_POLICY_RENEW_NC();

                    distrib.INITIAL_DATE = INITIAL_DATE;
                    distrib.TRANSACTION_SEQ = transaction.TRANSACTION_SEQUENCE.GetValueOrDefault();
                    distrib.DOCUMENT_NUMBER = Convert.ToDecimal(transaction.DOCUMENT_NUMBER);
                    distrib.CUSTNMBR = item.CUSTNMBR;
                    distrib.NCNUMBER = item.DOCNUMBR;
                    distrib.DISTTYPE = item.DISTTYPE;
                    distrib.DistRef = item.DistRef;
                    distrib.ACTNUMST = item.ACTNUMST;
                    distrib.DEBITAMT = item.DEBITAMT.GetValueOrDefault();
                    distrib.CRDTAMNT = item.CRDTAMNT.GetValueOrDefault();
                    distrib.CREATE_DATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    distrib.PROCESS = false;

                    contexto.ST_SYSFLEX_POLICY_RENEW_NC.Add(distrib);

                    contexto.SaveChanges();
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
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="state"></param>
        public void ChangeCustomerState(string customer, byte state)
        {
            var contexto = new ATLANEntities();
            try
            {
                contexto.usp_SYSFLEX_CHANGE_STATE_CUSTOMER(customer, state);
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error al intentar cambiar el estado del cliente: ", 1, ex);
                //throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sopnumbe"></param>
        public void DeleteReserve(string sopnumbe)
        {
            var contexto = new ATLANEntities();

            try
            {
                contexto.usp_SYSFLEX_DELETE_RESERVE(sopnumbe);
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error al intentar eliminar la reserva de correlativo ", 1, ex);
                //throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        /// <summary>
        /// Verfica si al canal de venta se le debe realizar calculo de comision
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool ChannelNoComm(string channel)
        {
            var contexto = new ATLANEntities();
            var existe = false;

            try
            {
                var conteo = (from canales in contexto.ST_CHANNEL_NO_COMMISSION
                              where  canales.CHANNEL.ToUpper().Trim() == channel.ToUpper().Trim()
                              select canales).Count();
                if (conteo > 0)
                {
                    existe = true;
                }
                else
                {
                    existe = false;
                }

                return existe;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                contexto.Dispose();
            }

        }

        
        public void UpdateCommission()
        {
            var contexto = new ATLANEntities();

            try
            {
                contexto.usp_ST_SYSFLEX_UPDATE_COMMISSION();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error al intentar actualizar la comision ", 1, ex);
                //throw;
            }
            finally
            {
                contexto.Dispose();
            }
        }

        public void DeleteExistingRenewDistribution(string sopnumbe)
        {
            var context = new ATLANEntities();
            try
            {
                var deletelist = (from renew in context.ST_SYSFLEX_POLICY_RENEW
                                  where renew.SOPNUMBE == sopnumbe.Trim()
                                  select renew).ToList();

                context.ST_SYSFLEX_POLICY_RENEW.RemoveRange(deletelist);

                context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }
        }

        public void DeleteExistingRenewDistributionNC(string ncnumber)
        {
            var context = new ATLANEntities();
            try
            {
                var deletelist = (from renew in context.ST_SYSFLEX_POLICY_RENEW_NC
                                  where renew.NCNUMBER == ncnumber.Trim()
                                  select renew).ToList();

                context.ST_SYSFLEX_POLICY_RENEW_NC.RemoveRange(deletelist);

                context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
