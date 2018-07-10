using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using RMClass;
using SOPClass;
using SysflexIntegrationUTIL;
using eConnectIntegration.CLASS;
using eConnectIntegration;
using InvoiceSync;
using InvoiceSync.BL;
using System.Diagnostics;
using Log4NetMR;


namespace InvoiceSync
{
    public class InvoiceSyncTask
    {
        classClsRegistrarLog log = new classClsRegistrarLog();
        public void Process()
        {
            var transactions = new InvoiceBL();
            transactions.InsertTransactionFromSysflex();
            var gpInvoice = new GPServicesREF.SOP.Invoices();
            var rminvoice = new GPServicesREF.RM.Receivables();
            List<TRANSACTION_INVOICE> transactionlist = transactions.GetTransactions();
            var company = ConfigKey.ReadSetting("Company");
            Response response;

            SYSFLEX_POLICY policyInfo;
            List<TRANSACTION_BY_COVERAGE> TransactionCoverage;

            try
            {
                log.WriteLog(0, "Total de Transacciones: " + transactionlist.Count.ToString());
                foreach (TRANSACTION_INVOICE policyTransaction in transactionlist)
                {
                    policyInfo = transactions.GetPolicyInfo(policyTransaction.POLICY_NUMBER.Trim());
                    if (policyInfo == null)
                    {
                        /////Si la poliza se encuentra en estado "IN TRANSIT" pasa al siguiente item
                        continue;
                    }
                    else
                    {
                        TransactionCoverage = transactions.GetCoverageDistribution(policyTransaction.POLICY_NUMBER.Trim(), policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());
                        //var channel_no_comm = ConfigKey.ReadSetting("CHANNEL_NO_COMM");
                        //var arrChannelNoComm = String.Join(",", channel_no_comm.ToArray());
                        if (policyTransaction.TRANS_TYPE == "D")
                        {
                            SOPHeader header;
                            SOPDetail[] detail;
                            SOPCommissions[] commission;
                            SOPTax[] taxes;
                            SOPDistribution[] distribution;
                            SOPDistribution[] distributionRenew;
                            Microsoft.Dynamics.GP.eConnect.SopType soptype;
                            try
                            {

                                header = transactions.CreateHeader(policyInfo, policyTransaction, TransactionCoverage);

                                if (header.SOPTYPE == 3)
                                {
                                    soptype = Microsoft.Dynamics.GP.eConnect.SopType.SOPInvoice;
                                }
                                else if (header.SOPTYPE == 4)
                                {
                                    soptype = Microsoft.Dynamics.GP.eConnect.SopType.SOPReturn;
                                }
                                else if (header.SOPTYPE == 1)
                                {
                                    soptype = Microsoft.Dynamics.GP.eConnect.SopType.SOPQuote;
                                }
                                else if (header.SOPTYPE == 2)
                                {
                                    soptype = Microsoft.Dynamics.GP.eConnect.SopType.SOPOrder;
                                }
                                else
                                {
                                    soptype = Microsoft.Dynamics.GP.eConnect.SopType.SOPBackOrder;
                                }
                                //if (policyTransaction.TRANS_DESC.Trim() == "RENOVACION" && (policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year >= policyTransaction.TRANS_DATE.GetValueOrDefault().Year))
                                if (policyTransaction.TRANS_DESC.Trim() == "RENOVACION" &&
                                   ((policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year == policyTransaction.TRANS_DATE.GetValueOrDefault().Year) ||
                                    (policyInfo.INITAL_DATE.GetValueOrDefault().Month <= policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year > policyTransaction.TRANS_DATE.GetValueOrDefault().Year)))
                                {
                                    detail = transactions.CreateUnitDetail(policyInfo, policyTransaction, TransactionCoverage, header);
                                    taxes = transactions.CreateUnitTax(policyInfo, policyTransaction, header, TransactionCoverage);

                                    //if (!channel_no_comm.Contains(policyInfo.CHANNEL.Trim()))
                                    if (!transactions.ChannelNoComm(policyInfo.CHANNEL.Trim()))
                                    {
                                        commission = transactions.CreateCommissions(policyInfo, policyTransaction, header, detail, TransactionCoverage);
                                    }
                                    else
                                    {
                                        commission = new List<SOPCommissions>().ToArray();
                                    }

                                    distribution = transactions.CreateUnitDistribution(policyInfo, policyTransaction, TransactionCoverage, detail, commission, taxes, header);

                                }
                                else
                                {
                                    detail = transactions.CreateDetail(policyInfo, policyTransaction, TransactionCoverage, header);
                                    taxes = transactions.CreateTax(policyInfo, policyTransaction, header, TransactionCoverage);

                                    //if (!channel_no_comm.Contains(policyInfo.CHANNEL.Trim()))
                                    if (!transactions.ChannelNoComm(policyInfo.CHANNEL.Trim()))
                                    {
                                        commission = transactions.CreateCommissions(policyInfo, policyTransaction, header, detail, TransactionCoverage);
                                    }
                                    else
                                    {
                                        commission = new List<SOPCommissions>().ToArray();
                                    }


                                    distribution = transactions.CreateDistribution(policyInfo, policyTransaction, TransactionCoverage, detail, commission, taxes, header);

                                }
                                //commission = transactions.CreateCommissions(policyInfo, policyTransaction, header, detail, TransactionCoverage);
                                header.SUBTOTAL = detail.Sum(X => X.XTNDPRCE);
                                header.USINGHEADERLEVELTAXES = 1;
                                header.TAXAMNT = taxes.Sum(x => x.STAXAMNT);
                                header.DOCAMNT = header.SUBTOTAL.GetValueOrDefault() + header.TAXAMNT.GetValueOrDefault();
                                header.COMMAMNT = commission.Sum(x => x.COMMAMNT.GetValueOrDefault());
                                if (policyTransaction.TRANS_DESC.Trim() == "REHABILITACION DE POLIZA")
                                {
                                    transactions.ChangeCustomerState(header.CUSTNMBR, 0);
                                }

                                ////Eliminar Reserva de Correlativo
                                transactions.DeleteReserve(header.SOPNUMBE);

                                response = gpInvoice.CreateInvoice(header, detail, distribution, commission, taxes, soptype, company);

                                if (response.SUCCESS)
                                {
                                    transactions.UpdateTransactionInvoce(header.SOPTYPE, "SOP", header.CUSTNMBR, policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());
                                    log.WriteLog(0, "Invoice Creado:" + response.MESSAGE);
                                    //if (policyTransaction.TRANS_DESC.Trim() == "RENOVACION" && (policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year >= policyTransaction.TRANS_DATE.GetValueOrDefault().Year))
                                    if (policyTransaction.TRANS_DESC.Trim() == "RENOVACION" &&
                                        ((policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year == policyTransaction.TRANS_DATE.GetValueOrDefault().Year) ||
                                         (policyInfo.INITAL_DATE.GetValueOrDefault().Month <= policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year > policyTransaction.TRANS_DATE.GetValueOrDefault().Year)))
                                    {
                                        ///Generar partida Normal y guardarla en temporal
                                        var detailRenew = transactions.CreateDetail(policyInfo, policyTransaction, TransactionCoverage, header);
                                        var taxesRenew = transactions.CreateTax(policyInfo, policyTransaction, header, TransactionCoverage);
                                        distributionRenew = transactions.CreateDistribution(policyInfo, policyTransaction, TransactionCoverage, detailRenew, commission, taxesRenew, header);
                                        transactions.SaveRenewDistribution(policyInfo.INITAL_DATE.GetValueOrDefault(), policyTransaction, distributionRenew);
                                    }
                                }
                                else
                                {
                                    log.WriteLog(2, "Ocurrió un error: " + response.MESSAGE);
                                    //log.escribirLog(2, header);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.LogExeption("Ocurrió un error al generar el invoice para la poliza " + policyInfo.POLICY_NUMBER.Trim(), 2, ex);
                            }
                        }

                        if (policyTransaction.TRANS_TYPE == "C")
                        {
                            RMTransactionHeader header;
                            RMTransactionDist[] distrib;
                            RMDocumentTaxes[] taxes;
                            try
                            {
                                header = transactions.CreateRMHeader(policyInfo, policyTransaction, TransactionCoverage);

                                if (((policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year == policyTransaction.TRANS_DATE.GetValueOrDefault().Year) ||
                                    (policyInfo.INITAL_DATE.GetValueOrDefault().Month <= policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year > policyTransaction.TRANS_DATE.GetValueOrDefault().Year)))
                                {
                                    taxes = transactions.CreateRMTax(policyInfo, policyTransaction, header, TransactionCoverage, true);
                                    distrib = transactions.CreateRMUnitDistribution(policyInfo, policyTransaction, TransactionCoverage, taxes, header);
                                }
                                else
                                {
                                    taxes = transactions.CreateRMTax(policyInfo, policyTransaction, header, TransactionCoverage, false);
                                    distrib = transactions.CreateRMDistribution(policyInfo, policyTransaction, TransactionCoverage, taxes, header);
                                }


                                response = rminvoice.CreateRMInvoice(header, distrib, taxes, company);

                                if (response.SUCCESS)
                                {
                                    transactions.UpdateTransactionInvoce(header.RMDTYPAL, "RM", header.CUSTNMBR, policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());
                                    log.WriteLog(0, "Invoice Creado:" + response.MESSAGE);

                                    if (((policyInfo.INITAL_DATE.GetValueOrDefault().Month > policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year == policyTransaction.TRANS_DATE.GetValueOrDefault().Year) ||
                                       (policyInfo.INITAL_DATE.GetValueOrDefault().Month <= policyTransaction.TRANS_DATE.GetValueOrDefault().Month && policyInfo.INITAL_DATE.GetValueOrDefault().Year > policyTransaction.TRANS_DATE.GetValueOrDefault().Year)))
                                    {
                                        
                                        var taxesrenew = transactions.CreateRMTax(policyInfo, policyTransaction, header, TransactionCoverage, false);
                                        var distribrenew = transactions.CreateRMDistribution(policyInfo, policyTransaction, TransactionCoverage, taxesrenew, header);
                                        transactions.SaveRenewDistribution(policyInfo.INITAL_DATE.GetValueOrDefault(), policyTransaction, distribrenew);
                                    }
                                }
                                else
                                {
                                    log.WriteLog(2, "Ocurrió un error: " + response.MESSAGE);
                                    //log.escribirLog(2, header);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.LogExeption("Ocurrió un error: ", 2, ex);
                                // throw;
                            }

                        }
                    }
                }/////
                transactions.UpdateCommission();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }
    }
}
