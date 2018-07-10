using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;
using InvoiceSync.DAL;
using SysflexIntegrationUTIL;
using PolicySync;
using PolicySync.DAL;
using SOPClass;
using RMClass;
using Log4NetMR;
using System.Collections;

namespace InvoiceSync.BL
{
    public class InvoiceBL
    {
        classClsRegistrarLog log = new classClsRegistrarLog();

        /// <summary>
        /// Obtiene la lista de invoices a ser cargados, ordenados por fecha
        /// </summary>
        /// <returns></returns>
        public List<TRANSACTION_INVOICE> GetTransactions()
        {
            var invoicedal = new InvoiceDAL();
            return invoicedal.GetTransactions();
        }

        /// <summary>
        /// Obtiene la distribucion por cobertura de la poliza
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="transaction_secuence"></param>
        /// <returns></returns>
        public List<TRANSACTION_BY_COVERAGE> GetCoverageDistribution(string policy, int transaction_secuence)
        {
            var invoicedal = new InvoiceDAL();
            return invoicedal.GetCoverageDistribution(policy, transaction_secuence);
        }

        /// <summary>
        /// Obtiene la informacion de la poliza
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public SYSFLEX_POLICY GetPolicyInfo(string policy)
        {
            var invoicedal = new InvoiceDAL();
            return invoicedal.GetPolicyInfo(policy);
        }

        /// <summary>
        /// Descarga los invoices de Sysflex que no existan en GP
        /// </summary>
        public void InsertTransactionFromSysflex()
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.InsertTransactionFromSysflex();
        }

        /// <summary>
        /// Crea encabezado de Invoice
        /// </summary>
        /// <param name="policyInfo">Datos de Poliza</param>
        /// <param name="policyTransaction">Datos de la transaccion de la poliza</param>
        /// <param name="transactionbyCoverage">Distribucion por cobertura de la poliza</param>
        /// <returns></returns>
        public SOPClass.SOPHeader CreateHeader(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage)
        {
            var sopHeader = new SOPClass.SOPHeader();
            string integrationid = ConfigKey.ReadSetting("INTEGRATIONID");
            string defaultCountry = ConfigKey.ReadSetting("DEFAULTCOUNTRY");
            string defaultCity = ConfigKey.ReadSetting("DEFAULTCITY");
            var InvoiceDate = policyTransaction.TRANS_DATE.GetValueOrDefault();
            var policy = new PolicyDAL();


            log.WriteLog(0, "Contruyendo encabezado");

            //var strBACHNUMB = "SFFA" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.Hour.ToString();
            var strBACHNUMB = "SFFA" + policyTransaction.TRANS_DATE.GetValueOrDefault().ToShortDateString().Replace("/", "");

            log.WriteLog(0, "Creacion de numero de batch " + strBACHNUMB);
            try
            {
                var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
                log.WriteLog(0, "Se obtiene el cliente de GP " + policyInfo.POLICY_NUMBER.Trim());
                //sopHeader.SOPTYPE = policyTransaction.
                sopHeader.SOPNUMBE = policyTransaction.GP_DOCNUMBR;
                sopHeader.CUSTNMBR = policyTransaction.POLICY_NUMBER;

                if (policyTransaction.TRANS_TYPE.Trim() == "D")
                {
                    sopHeader.DOCID = "INV";
                    sopHeader.SOPTYPE = 3;
                }


                //var InvoiceDate = policyTransaction.TRANS_DATE.GetValueOrDefault();

                sopHeader.DOCDATE = InvoiceDate.Date; //new DateTime(policyTransaction.TRANS_DATE.GetValueOrDefault().Year, policyTransaction.TRANS_DATE.GetValueOrDefault().Month, policyTransaction.TRANS_DATE.GetValueOrDefault().Day);
                sopHeader.DUEDATE = InvoiceDate.Date;//new DateTime(policyTransaction.TRANS_DATE.GetValueOrDefault().Year, policyTransaction.TRANS_DATE.GetValueOrDefault().Month, policyTransaction.TRANS_DATE.GetValueOrDefault().Day);
                //sopHeader.SUBTOTAL = Math.Round(transactionbyCoverage.FirstOrDefault().TRANS_AMOUNT.GetValueOrDefault(), 2);
                //sopHeader.TAXAMNT = Math.Round(transactionbyCoverage.FirstOrDefault().Trans_Tax_Amount.GetValueOrDefault(), 2);
                sopHeader.TAXSCHID = ConfigKey.ReadSetting("TAXID");
                //sopHeader.DOCAMNT = sopHeader.SUBTOTAL + sopHeader.TAXAMNT;
                sopHeader.CREATECOMM = 0;
                sopHeader.CREATETAXES = 0;
                sopHeader.CREATEDIST = 0;
                sopHeader.INTEGRATIONID = integrationid;
                sopHeader.LOCNCODE = "PRIMARY";
                sopHeader.SLPRSNID = policyInfo.AGENT_ID.GetValueOrDefault().ToString();
                //sopHeader.COUNTRY = gpCustomer.COUNTRY;
                if (string.IsNullOrEmpty(gpCustomer.COUNTRY) || string.IsNullOrWhiteSpace(gpCustomer.COUNTRY) || gpCustomer.COUNTRY.Trim() == "Dominicana")
                {
                    sopHeader.COUNTRY = defaultCountry.Trim();
                }
                else
                {
                    sopHeader.COUNTRY = gpCustomer.COUNTRY;
                }
                sopHeader.CITY = gpCustomer.CITY;
                if (string.IsNullOrEmpty(gpCustomer.CITY) || string.IsNullOrWhiteSpace(gpCustomer.CITY))
                {
                    sopHeader.CITY = defaultCity.Trim();
                }
                else
                {
                    sopHeader.CITY = gpCustomer.CITY;
                }

                sopHeader.BACHNUMB = strBACHNUMB;
                sopHeader.FRTTXAMT = 0;
                sopHeader.MSCTXAMT = 0;
                sopHeader.MISCAMNT = 0;
                sopHeader.TRDISAMT = 0;
                sopHeader.FREIGHT = 0;
                sopHeader.MISCAMNT = 0;

                return sopHeader;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionbyCoverage"></param>
        /// <returns></returns>
        public RMTransactionHeader CreateRMHeader(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage)
        {
            var rmheader = new RMTransactionHeader();
            string integrationid = ConfigKey.ReadSetting("INTEGRATIONID");
            string defaultCountry = ConfigKey.ReadSetting("DEFAULTCOUNTRY");
            string defaultCity = ConfigKey.ReadSetting("DEFAULTCITY");
            var InvoiceDate = policyTransaction.TRANS_DATE.GetValueOrDefault();

            var policy = new PolicyDAL();
            log.WriteLog(0, "Contruyendo encabezado");

            //var strBACHNUMB = "SFNC" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.Hour.ToString();
            var strBACHNUMB = "SFNC" + policyTransaction.TRANS_DATE.GetValueOrDefault().ToShortDateString().Replace("/", "");

            //var InvoiceDate = policyTransaction.TRANS_DATE.GetValueOrDefault();
            log.WriteLog(0, "Creacion de numero de batch " + strBACHNUMB);
            try
            {
                var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
                log.WriteLog(0, "Se obtiene el cliente de GP " + policyInfo.POLICY_NUMBER.Trim());
                rmheader.DOCNUMBR = policyTransaction.GP_DOCNUMBR;
                rmheader.CUSTNMBR = policyTransaction.POLICY_NUMBER;
                rmheader.RMDTYPAL = 7;
                rmheader.DOCDESCR = policyTransaction.TRANS_DESC;///"Nota de Crédito"; 
                rmheader.DOCDATE = InvoiceDate.Date; //new DateTime(policyTransaction.TRANS_DATE.GetValueOrDefault().Year, policyTransaction.TRANS_DATE.GetValueOrDefault().Month, policyTransaction.TRANS_DATE.GetValueOrDefault().Day);
                rmheader.DUEDATE = InvoiceDate.Date;  //new DateTime(policyTransaction.TRANS_DATE.GetValueOrDefault().Year, policyTransaction.TRANS_DATE.GetValueOrDefault().Month, policyTransaction.TRANS_DATE.GetValueOrDefault().Day);
                if (transactionbyCoverage.Count > 0)
                {
                    rmheader.TAXAMNT = Math.Abs(transactionbyCoverage.FirstOrDefault().Trans_Tax_Amount.GetValueOrDefault());
                    rmheader.SLSAMNT = Math.Abs(transactionbyCoverage.Sum(x => x.Coverage_Amount.GetValueOrDefault()));
                }
                else
                {
                    rmheader.TAXAMNT = 0;
                    rmheader.SLSAMNT = Math.Abs(Math.Round(policyTransaction.TRANS_AMOUNT.GetValueOrDefault(), 2));
                }

                rmheader.TAXSCHID = ConfigKey.ReadSetting("TAXID");
                rmheader.CreateTaxes = 1;
                rmheader.CREATEDIST = 0;
                rmheader.SLPRSNID = policyInfo.AGENT_ID.GetValueOrDefault().ToString();
                rmheader.BACHNUMB = strBACHNUMB;
                rmheader.FRTAMNT = 0;
                rmheader.MISCAMNT = 0;
                rmheader.TRDISAMT = 0;
                rmheader.FRTAMNT = 0;
                rmheader.DOCAMNT = rmheader.SLSAMNT + rmheader.TAXAMNT.GetValueOrDefault();

                return rmheader;
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// Genera los items del invoice
        /// </summary>
        /// <param name="policyInfo">Informacion de la poliza</param>
        /// <param name="policyTransaction">Informacion de la transaccion</param>
        /// <param name="transactionbyCoverage">Invormacion de la distribucion de las coberturas</param>
        /// <param name="header">Encabezado del invoice</param>
        /// <returns>Lista de items confirgurados</returns>
        public SOPClass.SOPDetail[] CreateDetail(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage, SOPClass.SOPHeader header)
        {
            var invoicedal = new InvoiceDAL();
            var sopdetail = new List<SOPClass.SOPDetail>();
            var accountB = new AccountBuilder();
            try
            {
                foreach (TRANSACTION_BY_COVERAGE item in transactionbyCoverage)
                {
                    if (item.Coverage_Group != "Impuestos")
                    {
                        IV00101 itemnmbr;
                        if (item.Coverage_Group.Contains("TERCERO") || item.Coverage_Group.Contains("Tercero"))
                        {
                            itemnmbr = invoicedal.GetItemLEY(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemLEY(item.VEHICLE_TYPE.Trim().Substring(0, 4));

                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);

                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "LEY");
                                    }
                                }
                            }
                        }
                        else if (item.Coverage_Group.Contains("PROPIO") || item.Coverage_Group.Contains("Propio"))
                        {
                            itemnmbr = invoicedal.GetItemPROPIOS(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemPROPIOS(item.VEHICLE_TYPE.Trim().Substring(0, 4));
                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);
                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "PROPIO");
                                    }
                                }
                            }
                        }
                        else if (item.Coverage_Group.Contains("SERVICIO") || item.Coverage_Group.Contains("Servicio"))
                        {
                            itemnmbr = invoicedal.GetItemSERVICIOS(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemSERVICIOS(item.VEHICLE_TYPE.Trim().Substring(0, 4));
                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);
                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "SERVICIO");
                                    }
                                }
                            }
                        }
                        else
                        {
                            itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);

                            ////colocar aqui el codigo para log
                        }

                        if (itemnmbr == null)
                        {
                            itemnmbr = new IV00101();
                            log.WriteLog(1, "No se ha encontrado el ITEMNMBR adecuado, " + item.VEHICLE_TYPE.Trim() + " - " + item.Product_type.Trim());
                            //break;
                        }
                        else
                        {
                            var detail = new SOPClass.SOPDetail();

                            detail.SOPNUMBE = header.SOPNUMBE;
                            detail.SOPTYPE = header.SOPTYPE;
                            detail.DOCID = header.DOCID;
                            detail.DOCDATE = header.DOCDATE;
                            detail.CUSTNMBR = header.CUSTNMBR;
                            detail.ITEMNMBR = itemnmbr.ITEMNMBR;
                            detail.ITEMDESC = item.Coverage_Desc.Trim();
                            detail.UNITPRCE = Math.Round(item.Coverage_Amount.GetValueOrDefault(), 2);
                            detail.QUANTITY = 1;
                            detail.QtyShrtOpt = 1;
                            detail.XTNDPRCE = Math.Round((detail.QUANTITY.GetValueOrDefault() * detail.UNITPRCE.GetValueOrDefault()), 2);
                            string SalesAccount = string.Empty;

                            SalesAccount = accountB.BuildSalesAccount(2, policyInfo.LINE_OF_BUSINESS.Trim(),
                                                                     item.Product_type.Trim(),
                                                                     item.VEHICLE_TYPE.Trim(),
                                                                     header.COUNTRY.Trim(),
                                                                     policyInfo.CHANNEL.Trim(),
                                                                     policyInfo.SUPERVISOR.Trim(),
                                                                     policyInfo.SUPERVISOR_CODE,
                                                                     item.Coverage_Desc.Trim(), policyInfo.GpProductID);

                            detail.INVINDX = SalesAccount;
                            detail.SLSINDX = SalesAccount;
                            detail.CSLSINDX = SalesAccount;
                            detail.USERDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            sopdetail.Add(detail);
                        }
                    }
                }
                return sopdetail.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionbyCoverage"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public SOPClass.SOPDetail[] CreateUnitDetail(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage, SOPClass.SOPHeader header)
        {
            var invoicedal = new InvoiceDAL();
            var sopdetail = new List<SOPClass.SOPDetail>();
            var accountB = new AccountBuilder();
            try
            {
                foreach (TRANSACTION_BY_COVERAGE item in transactionbyCoverage)
                {
                    if (item.Coverage_Group != "Impuestos")
                    {

                        IV00101 itemnmbr;
                        if (item.Coverage_Group.Contains("TERCERO") || item.Coverage_Group.Contains("Tercero"))
                        {
                            itemnmbr = invoicedal.GetItemLEY(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemLEY(item.VEHICLE_TYPE.Trim().Substring(0, 4));

                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);
                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "LEY");
                                    }
                                }
                            }
                        }
                        else if (item.Coverage_Group.Contains("PROPIO") || item.Coverage_Group.Contains("Propio"))
                        {
                            itemnmbr = invoicedal.GetItemPROPIOS(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemPROPIOS(item.VEHICLE_TYPE.Trim().Substring(0, 4));
                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);
                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "PROPIO");
                                    }
                                }
                            }
                        }
                        else if (item.Coverage_Group.Contains("SERVICIO") || item.Coverage_Group.Contains("Servicio"))
                        {
                            itemnmbr = invoicedal.GetItemSERVICIOS(item.VEHICLE_TYPE.Trim());
                            if (itemnmbr == null)
                            {
                                itemnmbr = invoicedal.GetItemSERVICIOS(item.VEHICLE_TYPE.Trim().Substring(0, 4));
                                if (itemnmbr == null)
                                {
                                    itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);
                                    if (itemnmbr == null)
                                    {
                                        itemnmbr = invoicedal.GetItemNumberByProductCode(accountB.GetProductMapping(item.Product_type.Trim(), item.VEHICLE_TYPE.Trim()), "SERVICIO");
                                    }
                                }
                            }
                        }
                        else
                        {
                            itemnmbr = invoicedal.GetItemNumberByProductCode(policyInfo.GpProductID);

                            ////colocar aqui el codigo para log
                        }
                        if (itemnmbr == null)
                        {
                            itemnmbr = new IV00101();
                            log.WriteLog(1, "No se ha encontrado el ITEMNMBR adecuado");
                            //break;
                        }
                        else
                        {
                            var detail = new SOPClass.SOPDetail();

                            detail.SOPNUMBE = header.SOPNUMBE;
                            detail.SOPTYPE = header.SOPTYPE;
                            detail.DOCID = header.DOCID;
                            detail.DOCDATE = header.DOCDATE;
                            detail.CUSTNMBR = header.CUSTNMBR;
                            detail.ITEMNMBR = itemnmbr.ITEMNMBR;
                            detail.ITEMDESC = item.Coverage_Desc.Trim();
                            detail.UNITPRCE = Math.Round(item.Coverage_Amount.GetValueOrDefault(), 2);
                            detail.QUANTITY = 1;
                            detail.QtyShrtOpt = 1;
                            detail.XTNDPRCE = Math.Round((detail.QUANTITY.GetValueOrDefault() * detail.UNITPRCE.GetValueOrDefault()), 2);

                            string SalesAccount = string.Empty;

                            SalesAccount = accountB.BuildUnitAccount(2, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                            detail.INVINDX = SalesAccount;
                            detail.SLSINDX = SalesAccount;
                            detail.CSLSINDX = SalesAccount;

                            detail.USERDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            sopdetail.Add(detail);
                        }
                    }
                }
                return sopdetail.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// Agrega la distribucion de impuestos aplicables
        /// </summary>
        /// <param name="policyInfo">Informacion de la poliza</param>
        /// <param name="policyTransaction">Informacion de la transaccion de Sysflex</param>
        /// <param name="header">Encabezado del invoice</param>
        /// <returns>Lista de impuestos configurados</returns>
        public SOPClass.SOPTax[] CreateTax(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, SOPClass.SOPHeader header, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage)
        {
            string taxid = ConfigKey.ReadSetting("TAXID");
            string taxAccount = ConfigKey.ReadSetting("TAXACCOUNT");

            var accountB = new AccountBuilder();
            var sopTax = new List<SOPClass.SOPTax>();
            var tax = new SOPClass.SOPTax();
            TRANSACTION_BY_COVERAGE coverageTAX;
            if (transactionbyCoverage.Count > 0)
            {
                coverageTAX = transactionbyCoverage.FirstOrDefault();//transactionbyCoverage.(x => x.Coverage_Group == "Impuestos");
            }
            else
            {
                coverageTAX = new TRANSACTION_BY_COVERAGE();
                coverageTAX.Trans_Tax_Amount = 0;
            }
            try
            {
                if (coverageTAX != null && coverageTAX.Trans_Tax_Amount > 0)
                {
                    tax.LNITMSEQ = 0;
                    tax.SOPTYPE = header.SOPTYPE;
                    tax.SOPNUMBE = header.SOPNUMBE;
                    tax.SALESAMT = header.SUBTOTAL;
                    tax.CUSTNMBR = header.CUSTNMBR;
                    tax.TAXDTLID = taxid;
                    tax.STAXAMNT = Math.Round(coverageTAX.Trans_Tax_Amount.GetValueOrDefault(), 2);
                    tax.ACTNUMST = taxAccount;
                    tax.FREIGHT = 0;
                    tax.FRTTXAMT = 0;
                    tax.MISCAMNT = 0;
                    tax.MSCTXAMT = 0;
                    sopTax.Add(tax);
                }

                return sopTax.ToArray();

            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="header"></param>
        /// <param name="transactionbyCoverage"></param>
        /// <returns></returns>
        public SOPClass.SOPTax[] CreateUnitTax(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, SOPClass.SOPHeader header, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage)
        {
            var AccountB = new AccountBuilder();
            string taxid = ConfigKey.ReadSetting("TAXID");
            string taxAccount = AccountB.BuildUnitTaxAccount(7); //ConfigKey.ReadSetting("TAXACCOUNT");
            var sopTax = new List<SOPClass.SOPTax>();
            var tax = new SOPClass.SOPTax();
            TRANSACTION_BY_COVERAGE coverageTAX;
            if (transactionbyCoverage.Count > 0)
            {
                coverageTAX = transactionbyCoverage.FirstOrDefault();//transactionbyCoverage.(x => x.Coverage_Group == "Impuestos");
            }
            else
            {
                coverageTAX = new TRANSACTION_BY_COVERAGE();
                coverageTAX.Trans_Tax_Amount = 0;
            }
            try
            {
                if (coverageTAX != null && coverageTAX.Trans_Tax_Amount > 0)
                {
                    tax.LNITMSEQ = 0;
                    tax.SOPTYPE = header.SOPTYPE;
                    tax.SOPNUMBE = header.SOPNUMBE;
                    tax.SALESAMT = header.SUBTOTAL;
                    tax.CUSTNMBR = header.CUSTNMBR;
                    tax.TAXDTLID = taxid;
                    tax.STAXAMNT = Math.Round(coverageTAX.Trans_Tax_Amount.GetValueOrDefault(), 2);
                    tax.ACTNUMST = taxAccount;
                    tax.FREIGHT = 0;
                    tax.FRTTXAMT = 0;
                    tax.MISCAMNT = 0;
                    tax.MSCTXAMT = 0;
                    sopTax.Add(tax);
                }

                return sopTax.ToArray();

            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="header"></param>
        /// <param name="transactionbyCoverage"></param>
        /// <returns></returns>
        public RMDocumentTaxes[] CreateRMTax(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, RMTransactionHeader header, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage, bool isRenew)
        {
            string taxid;
            string taxAccount;
            var AccountB = new AccountBuilder();
            if (isRenew)
            {

                taxid = ConfigKey.ReadSetting("TAXID");
                taxAccount = AccountB.BuildUnitTaxAccount(7); //ConfigKey.ReadSetting("TAXACCOUNT");
            }
            else
            {
                taxid = ConfigKey.ReadSetting("TAXID");
                taxAccount = ConfigKey.ReadSetting("TAXACCOUNT");
            }


            var rmTax = new List<RMDocumentTaxes>();
            var tax = new RMDocumentTaxes();
            //TRANSACTION_BY_COVERAGE coverageTAX = transactionbyCoverage.Find(x => x.Coverage_Group == "Impuestos");

            TRANSACTION_BY_COVERAGE coverageTAX;
            if (transactionbyCoverage.Count > 0)
            {
                coverageTAX = transactionbyCoverage.FirstOrDefault();//transactionbyCoverage.(x => x.Coverage_Group == "Impuestos");
            }
            else
            {
                coverageTAX = new TRANSACTION_BY_COVERAGE();
                coverageTAX.Trans_Tax_Amount = 0;
            }
            try
            {
                if (coverageTAX != null && Math.Abs(coverageTAX.Trans_Tax_Amount.GetValueOrDefault()) > 0)
                {
                    //tax.SEQNUMBR = 0;
                    tax.RMDTYPAL = header.RMDTYPAL;
                    tax.DOCNUMBR = header.DOCNUMBR;
                    tax.BACHNUMB = header.BACHNUMB;
                    tax.CUSTNMBR = header.CUSTNMBR;
                    tax.TAXDTLID = taxid;
                    tax.TAXAMNT = Math.Abs(Math.Round(coverageTAX.Trans_Tax_Amount.GetValueOrDefault(), 2));
                    tax.STAXAMNT = 0;
                    tax.ACTNUMST = taxAccount;
                    tax.FRTTXAMT = 0;
                    tax.MSCTXAMT = 0;
                    tax.TAXDTSLS = Math.Abs(header.SLSAMNT);
                    tax.ACTNUMST = taxAccount;

                    rmTax.Add(tax);
                }

                return rmTax.ToArray();

            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <returns></returns>
        public SOPClass.SOPCommissions[] CreateCommissions(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, SOPClass.SOPHeader header, SOPClass.SOPDetail[] detail, List<TRANSACTION_BY_COVERAGE> transactionbyCoverage)
        {
            var sopCommisionList = new List<SOPClass.SOPCommissions>();
            var sopCommision = new SOPClass.SOPCommissions();
            var invoicedal = new InvoiceDAL();
            var policydal = new PolicyDAL();
            RM00101 customerGP = policydal.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
            RM00301 salesperson = policydal.GetGPSalesPerson(policyInfo.AGENT_CODE.ToString());
            decimal sopCommisionTotal = 0;

            try
            {

                //foreach (SOPDetail item in detail)
                //{
                //    sopCommisionTotal = sopCommisionTotal + invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), item.ITEMNMBR, policyInfo.CHANNEL, item.XTNDPRCE.GetValueOrDefault());
                //}
                if (policyInfo.VEHICLE_TYPE.Contains("FLOTI") || policyInfo.POLICY_TYPE.Contains("FLOTI"))
                {
                    foreach (TRANSACTION_BY_COVERAGE item in transactionbyCoverage)
                    {
                        if (item.Coverage_Group != "Impuestos" && item.Coverage_Amount.GetValueOrDefault() > 0)
                        {
                            sopCommisionTotal += invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), item.Product_type.Trim(), policyInfo.CHANNEL.Trim(), item.Coverage_Amount.GetValueOrDefault());
                        }

                    }
                }
                else
                {
                    sopCommisionTotal = invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), policyInfo.POLICY_TYPE.Trim(), policyInfo.CHANNEL.Trim(), detail.Sum(x => x.XTNDPRCE.GetValueOrDefault()));
                }




                sopCommision.CUSTNMBR = header.CUSTNMBR;
                sopCommision.SOPTYPE = header.SOPTYPE;
                sopCommision.SOPNUMBE = header.SOPNUMBE;
                sopCommision.LNITMSEQ = 16384;
                sopCommision.SLPRSNID = policyInfo.AGENT_CODE.ToString();
                sopCommision.SALSTERR = salesperson.SALSTERR;
                sopCommision.COMMAMNT = Math.Round(sopCommisionTotal, 2);

                sopCommisionList.Add(sopCommision);

                return sopCommisionList.ToArray();

            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }
        }

        /// <summary>
        /// Construye la distribucion contable del Invoice
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionCoverage"></param>
        /// <param name="detail"></param>
        /// <param name="commission"></param>
        /// <param name="taxes"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public SOPClass.SOPDistribution[] CreateDistribution(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionCoverage,
                                                             SOPClass.SOPDetail[] detail, SOPClass.SOPCommissions[] commission, SOPClass.SOPTax[] taxes, SOPHeader header)
        {
            var accountB = new AccountBuilder();
            var policy = new PolicyDAL();
            var invoicedal = new InvoiceDAL();
            var commissionlist = commission.ToList();
            var itemlist = detail.ToList();
            var taxlist = taxes.ToList();
            var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
            var coverageList = invoicedal.GetReinsuranceByCoverage(policyTransaction.POLICY_NUMBER.Trim(), policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());
            var productlist = transactionCoverage.Select(x => new { x.Product_type, x.VEHICLE_TYPE }).Distinct().ToList();

            var sopDistribution = new List<SOPClass.SOPDistribution>();

            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas por cobrar
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var sopDistributionCXC = new SOPDistribution();
                sopDistributionCXC.ACTNUMST = accountB.BuildARAccount(1, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                sopDistributionCXC.SOPNUMBE = header.SOPNUMBE;
                sopDistributionCXC.SOPTYPE = header.SOPTYPE;
                sopDistributionCXC.CUSTNMBR = header.CUSTNMBR;
                sopDistributionCXC.DistRef = "Primas por Cobrar";
                sopDistributionCXC.CRDTAMNT = 0;
                sopDistributionCXC.DEBITAMT = itemlist.Sum(x => x.XTNDPRCE) + taxlist.Sum(x => x.STAXAMNT);
                sopDistributionCXC.DISTTYPE = 2;
                sopDistribution.Add(sopDistributionCXC);

                log.WriteLog(0, "Cuenta por cobrar: " + sopDistributionCXC.ACTNUMST);


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Extraer cuentas de los items
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                foreach (SOPClass.SOPDetail item in itemlist)
                {
                    var distribAccount = new SOPClass.SOPDistribution();

                    distribAccount.ACTNUMST = item.SLSINDX;
                    distribAccount.SOPNUMBE = item.SOPNUMBE;
                    distribAccount.SOPTYPE = item.SOPTYPE;
                    distribAccount.CUSTNMBR = item.CUSTNMBR;
                    distribAccount.DistRef = item.ITEMDESC;
                    distribAccount.CRDTAMNT = item.XTNDPRCE;
                    distribAccount.DEBITAMT = 0;
                    distribAccount.DISTTYPE = 1;
                    sopDistribution.Add(distribAccount);

                    log.WriteLog(0, "Cuenta de producto: " + distribAccount.ACTNUMST);

                }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Extraer cuentas de impuestos
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                foreach (SOPClass.SOPTax item in taxlist)
                {
                    var distribAccount = new SOPClass.SOPDistribution();


                    distribAccount.ACTNUMST = item.ACTNUMST;
                    distribAccount.SOPNUMBE = item.SOPNUMBE;
                    distribAccount.SOPTYPE = item.SOPTYPE;
                    distribAccount.CUSTNMBR = item.CUSTNMBR;
                    distribAccount.DistRef = "ISC sobre prima pendiente de cobro";
                    distribAccount.CRDTAMNT = item.STAXAMNT;
                    distribAccount.DEBITAMT = 0;
                    distribAccount.DISTTYPE = 9;
                    sopDistribution.Add(distribAccount);

                    log.WriteLog(0, "Cuenta de impuestos: " + distribAccount.ACTNUMST);
                }


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Extraer cuentas de comisiones
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                foreach (SOPClass.SOPCommissions item in commissionlist)
                {
                    //Cuenta de gasto

                    var htComisiones = new Hashtable();
                    foreach (TRANSACTION_BY_COVERAGE itemcoverage in transactionCoverage)
                    {
                        if (itemcoverage.Coverage_Group != "Impuestos" && itemcoverage.Coverage_Amount.GetValueOrDefault() > 0)
                        {
                            var distribAccountEXP = new SOPClass.SOPDistribution();
                            distribAccountEXP.ACTNUMST = accountB.BuildAccountCommission(4, policyInfo.LINE_OF_BUSINESS, itemcoverage.Product_type.Trim(), itemcoverage.VEHICLE_TYPE, header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, itemcoverage.Coverage_Desc.Trim(), policyInfo.GpProductID);
                            distribAccountEXP.SOPNUMBE = header.SOPNUMBE;
                            distribAccountEXP.SOPTYPE = header.SOPTYPE;
                            distribAccountEXP.CUSTNMBR = header.CUSTNMBR;
                            distribAccountEXP.DistRef = "Comisiones intermediarios";
                            distribAccountEXP.CRDTAMNT = 0;
                            distribAccountEXP.DEBITAMT = invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault()); //item.COMMAMNT;

                            distribAccountEXP.DISTTYPE = 11;

                            log.WriteLog(0, "Cuenta de comisiones GASTO " + distribAccountEXP.ACTNUMST);
                            sopDistribution.Add(distribAccountEXP);
                        }

                        if (htComisiones.Contains(itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()))
                        {
                            htComisiones[itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()] = (decimal)htComisiones[itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()] + invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault());
                        }
                        else
                        {
                            htComisiones.Add(itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim(), invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault()));
                        }

                        // sopCommisionTotal += invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), item.Product_type.Trim(), policyInfo.CHANNEL.Trim(), item.Coverage_Amount.GetValueOrDefault());
                    }


                    //////////////////////
                    //Cuenta de Pasivo

                    foreach (var PL in productlist)
                    {
                        decimal sumcom = transactionCoverage.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => z.Coverage_Amount.GetValueOrDefault());

                        var distribAccountPAS = new SOPClass.SOPDistribution();
                        distribAccountPAS.ACTNUMST = accountB.BuildAccountCommission(5, policyInfo.LINE_OF_BUSINESS, PL.Product_type.Trim(), PL.VEHICLE_TYPE.Trim(), header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        distribAccountPAS.SOPNUMBE = header.SOPNUMBE;
                        distribAccountPAS.SOPTYPE = header.SOPTYPE;
                        distribAccountPAS.CUSTNMBR = header.CUSTNMBR;
                        distribAccountPAS.DistRef = "Comisiones a intermediarios pendientes de cobro";
                        //distribAccountPAS.CRDTAMNT = invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), PL.Product_type.Trim(), policyInfo.CHANNEL.Trim(), sumcom);//item.COMMAMNT;
                        distribAccountPAS.CRDTAMNT = (decimal)htComisiones[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];
                        distribAccountPAS.DEBITAMT = 0;
                        distribAccountPAS.DISTTYPE = 12;
                        sopDistribution.Add(distribAccountPAS);
                        log.WriteLog(0, "Cuenta de comisiones PAGO " + distribAccountPAS.ACTNUMST);
                    }

                    htComisiones.Clear();
                }

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Reserva de riesgos en curso
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                decimal percent;
                if (policyInfo.VEHICLE_TYPE.Contains("TRANSP") || policyInfo.POLICY_TYPE.ToUpper().Contains("TRANSP"))
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPERTRANS"));
                }
                else
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPER"));
                }

                log.WriteLog(0, "Porcentaje Reserva Riesgos en curso: " + percent.ToString());
                var htRRC = new Hashtable();
                //Expense
                foreach (TRANSACTION_BY_COVERAGE item in transactionCoverage)
                {
                    if (item.Coverage_Group != "Impuestos")
                    {
                        var sopDistributionRRCEXP = new SOPDistribution();
                        sopDistributionRRCEXP.ACTNUMST = accountB.BuildAccountRRC(8, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                  header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID, item.Coverage_Desc.Trim());
                        sopDistributionRRCEXP.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionRRCEXP.SOPTYPE = header.SOPTYPE;
                        sopDistributionRRCEXP.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionRRCEXP.DistRef = "Reservas para RRC presente ejercicio";
                        sopDistributionRRCEXP.CRDTAMNT = 0;
                        sopDistributionRRCEXP.DEBITAMT = Math.Round((item.Coverage_Amount.GetValueOrDefault() * (percent / 100)), 2);
                        sopDistributionRRCEXP.DISTTYPE = 13;
                        log.WriteLog(0, "Cuenta para RRC GASTO: " + sopDistributionRRCEXP.ACTNUMST);
                        sopDistribution.Add(sopDistributionRRCEXP);
                    }

                    if (htRRC.Contains(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()))
                    {
                        htRRC[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] = (decimal)htRRC[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] + Math.Round((item.Coverage_Amount.GetValueOrDefault() * (percent / 100)), 2);
                    }
                    else
                    {
                        htRRC.Add(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim(), Math.Round((item.Coverage_Amount.GetValueOrDefault() * (percent / 100)), 2));
                    }
                }

                //Pasivo
                foreach (var PL in productlist)
                {
                    decimal sumcom = transactionCoverage.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => z.Coverage_Amount.GetValueOrDefault());
                    var sopDistributionRRCPAS = new SOPDistribution();
                    sopDistributionRRCPAS.ACTNUMST = accountB.BuildARAccount(9, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                              header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    sopDistributionRRCPAS.SOPNUMBE = header.SOPNUMBE;
                    sopDistributionRRCPAS.SOPTYPE = header.SOPTYPE;
                    sopDistributionRRCPAS.CUSTNMBR = header.CUSTNMBR;
                    sopDistributionRRCPAS.DistRef = "Reservas para RRC ";
                    //sopDistributionRRCPAS.CRDTAMNT = Math.Round(sumcom * (percent / 100), 2);

                    sopDistributionRRCPAS.CRDTAMNT = (decimal)htRRC[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];
                    sopDistributionRRCPAS.DEBITAMT = 0;
                    sopDistributionRRCPAS.DISTTYPE = 13;

                    log.WriteLog(0, "Cuenta para RRC PASIVO: " + sopDistributionRRCPAS.ACTNUMST);
                    sopDistribution.Add(sopDistributionRRCPAS);
                }

                htRRC.Clear();

                if (coverageList.Count > 0)
                {

                    var productlistCoverage = coverageList.Select(x => new { x.Product_type, x.VEHICLE_TYPE }).Distinct().ToList();


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///Reaseguro y RRC para reaseguro
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////reaseguro por cobertura
                    foreach (SYSFLEX_REINSURANCE_BY_COVERAGE item in coverageList)
                    {
                        var sopDistributionREEXP = new SOPDistribution();
                        var sopDistributionRECOMM = new SOPDistribution();
                        bool IsLocal;
                        bool IsFacult;

                        if (item.Company_is_Local == 1)
                        {
                            IsLocal = true;
                        }
                        else
                        {
                            IsLocal = false;
                        }


                        if (item.Type_Contrat == "Contractual u Obligatorio")
                        {
                            IsFacult = false;
                        }
                        else
                        {
                            IsFacult = true;
                        }

                        sopDistributionREEXP.ACTNUMST = accountB.BuildReinsuranceAccount(10, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                         header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, item.Coverage_Desc,
                                                                                         policyInfo.GpProductID, IsLocal, IsFacult);
                        sopDistributionREEXP.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREEXP.SOPTYPE = header.SOPTYPE;
                        sopDistributionREEXP.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREEXP.DistRef = "Primas de reaseguro contractuales";
                        sopDistributionREEXP.CRDTAMNT = 0;
                        sopDistributionREEXP.DEBITAMT = item.Coverage_Amount.GetValueOrDefault();
                        sopDistributionREEXP.DISTTYPE = 13;
                        log.WriteLog(0, "Reaseguro Contractual: " + sopDistributionREEXP.ACTNUMST);
                        sopDistribution.Add(sopDistributionREEXP);

                        ////comision reaseguro
                        sopDistributionRECOMM.ACTNUMST = accountB.BuildReinsuranceCommAccount(10, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                         header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, item.Coverage_Desc,
                                                                                         policyInfo.GpProductID, IsFacult);

                        sopDistributionRECOMM.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionRECOMM.SOPTYPE = header.SOPTYPE;
                        sopDistributionRECOMM.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionRECOMM.DistRef = "Comision sobre prima cedida";
                        sopDistributionRECOMM.CRDTAMNT = item.Reinsurance_Amount_Commission.GetValueOrDefault();
                        sopDistributionRECOMM.DEBITAMT = 0;
                        sopDistributionRECOMM.DISTTYPE = 13;
                        sopDistribution.Add(sopDistributionRECOMM);
                        log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + sopDistributionRECOMM.ACTNUMST);

                    }

                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => z.Coverage_Amount.GetValueOrDefault());

                        ///Pasivo Reaseguro ----cuenta corriente
                        var sopDistributionREPAS = new SOPDistribution();
                        sopDistributionREPAS.ACTNUMST = accountB.BuildARAccount(11, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                   header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        sopDistributionREPAS.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREPAS.SOPTYPE = header.SOPTYPE;
                        sopDistributionREPAS.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREPAS.DistRef = "Reaseguradores del exterior – cuenta corriente";
                        sopDistributionREPAS.CRDTAMNT = sumcom; //coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2));
                        sopDistributionREPAS.DEBITAMT = 0;
                        sopDistributionREPAS.DISTTYPE = 13;
                        log.WriteLog(0, "Reaseguradores - cuenta corriente: " + sopDistributionREPAS.ACTNUMST);
                        sopDistribution.Add(sopDistributionREPAS);
                    }




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///Pasivo Comision Reaseguro
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => z.Reinsurance_Amount_Commission.GetValueOrDefault());

                        var sopDistributionREPASCOMM = new SOPDistribution();
                        sopDistributionREPASCOMM.ACTNUMST = accountB.BuildARAccount(11, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                   header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        sopDistributionREPASCOMM.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREPASCOMM.SOPTYPE = header.SOPTYPE;
                        sopDistributionREPASCOMM.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREPASCOMM.DistRef = "Comision sobre prima cedida";
                        sopDistributionREPASCOMM.CRDTAMNT = 0;
                        sopDistributionREPASCOMM.DEBITAMT = sumcom;//coverageList.Sum(x => Math.Round(x.Reinsurance_Amount_Commission.GetValueOrDefault(), 2));
                        sopDistributionREPASCOMM.DISTTYPE = 13;
                        log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + sopDistributionREPASCOMM.ACTNUMST);
                        sopDistribution.Add(sopDistributionREPASCOMM);
                    }

                    var htrrcReaseguro = new Hashtable();
                    //Income
                    foreach (SYSFLEX_REINSURANCE_BY_COVERAGE item in coverageList)
                    {
                        var sopDistributionREARRCEXP = new SOPDistribution();
                        sopDistributionREARRCEXP.ACTNUMST = accountB.BuildAccountRRC(12, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                  header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID, item.Coverage_Desc);
                        sopDistributionREARRCEXP.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREARRCEXP.SOPTYPE = header.SOPTYPE;
                        sopDistributionREARRCEXP.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREARRCEXP.DistRef = "Reservas para RRC a cargo reaseguro presente ejercicio";
                        sopDistributionREARRCEXP.CRDTAMNT = Math.Round(item.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2);
                        sopDistributionREARRCEXP.DEBITAMT = 0;
                        sopDistributionREARRCEXP.DISTTYPE = 13;
                        log.WriteLog(0, "RRC para reaseguro INGRESO: " + sopDistributionREARRCEXP.ACTNUMST);
                        sopDistribution.Add(sopDistributionREARRCEXP);

                        if (htrrcReaseguro.Contains(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()))
                        {
                            htrrcReaseguro[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] = (decimal)htrrcReaseguro[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] + Math.Round(item.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2);
                        }
                        else
                        {
                            htrrcReaseguro.Add(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim(), Math.Round(item.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2));
                        }
                    }



                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => z.Coverage_Amount.GetValueOrDefault());
                        //Pasivo
                        var sopDistributionREARRCPAS = new SOPDistribution();
                        sopDistributionREARRCPAS.ACTNUMST = accountB.BuildARAccount(9, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                                  header.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        sopDistributionREARRCPAS.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREARRCPAS.SOPTYPE = header.SOPTYPE;
                        sopDistributionREARRCPAS.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREARRCPAS.DistRef = "Reservas para RRC a cargo de reaseguro ";
                        sopDistributionREARRCPAS.CRDTAMNT = 0;
                        //sopDistributionREARRCPAS.DEBITAMT = Math.Round(sumcom * (percent / 100), 2, MidpointRounding.ToEven);
                        sopDistributionREARRCPAS.DEBITAMT = (decimal)htrrcReaseguro[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];
                        sopDistributionREARRCPAS.DISTTYPE = 13;
                        log.WriteLog(0, "RRC para reaseguro PASIVO: " + sopDistributionREARRCPAS.ACTNUMST);
                        sopDistribution.Add(sopDistributionREARRCPAS);
                    }
                    htrrcReaseguro.Clear();
                }

                return sopDistribution.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionCoverage"></param>
        /// <param name="detail"></param>
        /// <param name="commission"></param>
        /// <param name="taxes"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public SOPClass.SOPDistribution[] CreateUnitDistribution(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionCoverage,
                                                     SOPClass.SOPDetail[] detail, SOPClass.SOPCommissions[] commission, SOPClass.SOPTax[] taxes, SOPHeader header)
        {
            var accountB = new AccountBuilder();
            var policy = new PolicyDAL();
            var invoicedal = new InvoiceDAL();
            var commissionlist = commission.ToList();
            var itemlist = detail.ToList();
            var taxlist = taxes.ToList();
            var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
            var coverageList = invoicedal.GetReinsuranceByCoverage(policyTransaction.POLICY_NUMBER.Trim(), policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());

            var sopDistribution = new List<SOPClass.SOPDistribution>();

            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas por cobrar
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var sopDistributionCXC = new SOPDistribution();
                sopDistributionCXC.ACTNUMST = accountB.BuildUnitAccount(1, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);//accountB.BuildARAccount(1, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                sopDistributionCXC.SOPNUMBE = header.SOPNUMBE;
                sopDistributionCXC.SOPTYPE = header.SOPTYPE;
                sopDistributionCXC.CUSTNMBR = header.CUSTNMBR;
                sopDistributionCXC.DistRef = "Primas por Cobrar";

                if (policyTransaction.TRANS_TYPE == "D")
                {
                    sopDistributionCXC.CRDTAMNT = 0;
                    sopDistributionCXC.DEBITAMT = itemlist.Sum(x => x.XTNDPRCE) + taxlist.Sum(x => x.STAXAMNT);
                }
                else
                {
                    sopDistributionCXC.CRDTAMNT = itemlist.Sum(x => x.XTNDPRCE) + taxlist.Sum(x => x.STAXAMNT);
                    sopDistributionCXC.DEBITAMT = 0;
                }

                sopDistributionCXC.DISTTYPE = 2;

                sopDistribution.Add(sopDistributionCXC);

                log.WriteLog(0, "Cuenta por cobrar: " + sopDistributionCXC.ACTNUMST);



                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Extraer cuentas de los items
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //foreach (SOPClass.SOPDetail item in itemlist)
                //{
                var distribAccount = new SOPClass.SOPDistribution();

                distribAccount.ACTNUMST = accountB.BuildUnitAccount(2, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                distribAccount.SOPNUMBE = header.SOPNUMBE;
                distribAccount.SOPTYPE = header.SOPTYPE;
                distribAccount.CUSTNMBR = header.CUSTNMBR;
                distribAccount.DistRef = "PRIMAS SUSCRITAS";

                distribAccount.CRDTAMNT = itemlist.Sum(x => x.XTNDPRCE);
                distribAccount.DEBITAMT = 0;
                distribAccount.DISTTYPE = 1;
                sopDistribution.Add(distribAccount);

                log.WriteLog(0, "Cuenta de producto: " + distribAccount.ACTNUMST);

                //}


                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas de impuestos
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                foreach (SOPClass.SOPTax item in taxlist)
                {
                    var TaxdistribAccount = new SOPClass.SOPDistribution();


                    TaxdistribAccount.ACTNUMST = item.ACTNUMST;
                    TaxdistribAccount.SOPNUMBE = item.SOPNUMBE;
                    TaxdistribAccount.SOPTYPE = item.SOPTYPE;
                    TaxdistribAccount.CUSTNMBR = item.CUSTNMBR;
                    TaxdistribAccount.DistRef = "ISC sobre prima pendiente de cobro";
                    if (policyTransaction.TRANS_TYPE == "D")
                    {
                        TaxdistribAccount.CRDTAMNT = item.STAXAMNT;
                        TaxdistribAccount.DEBITAMT = 0;
                    }
                    else
                    {
                        TaxdistribAccount.CRDTAMNT = 0;
                        TaxdistribAccount.DEBITAMT = item.STAXAMNT;
                    }

                    TaxdistribAccount.DISTTYPE = 9;
                    sopDistribution.Add(TaxdistribAccount);

                    log.WriteLog(0, "Cuenta de impuestos: " + TaxdistribAccount.ACTNUMST);
                }


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Extraer cuentas de comisiones
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                foreach (SOPClass.SOPCommissions item in commissionlist)
                {
                    var distribAccountEXP = new SOPClass.SOPDistribution();
                    var distribAccountPAS = new SOPClass.SOPDistribution();


                    //Cuenta de gasto
                    //distribAccountEXP.ACTNUMST = accountB.BuildAccountCommission(4, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    distribAccountEXP.ACTNUMST = accountB.BuildUnitAccount(4, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);// accountB.BuildAccountCommission(4, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    distribAccountEXP.SOPNUMBE = header.SOPNUMBE;
                    distribAccountEXP.SOPTYPE = header.SOPTYPE;
                    distribAccountEXP.CUSTNMBR = header.CUSTNMBR;
                    distribAccountEXP.DistRef = "Comisiones intermediarios";
                    distribAccountEXP.CRDTAMNT = 0;
                    distribAccountEXP.DEBITAMT = item.COMMAMNT;
                    distribAccountEXP.DISTTYPE = 11;
                    sopDistribution.Add(distribAccountEXP);

                    //////////////////////
                    //Cuenta de Pasivo
                    //var distribAccountPAS = new SOPClass.SOPDistribution();
                    distribAccountPAS.ACTNUMST = accountB.BuildUnitAccount(5, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID); //accountB.BuildAccountCommission(5, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    distribAccountPAS.SOPNUMBE = header.SOPNUMBE;
                    distribAccountPAS.SOPTYPE = header.SOPTYPE;
                    distribAccountPAS.CUSTNMBR = header.CUSTNMBR;
                    distribAccountPAS.DistRef = "Comisiones a intermediarios pendientes de cobro";
                    distribAccountPAS.CRDTAMNT = item.COMMAMNT;
                    distribAccountPAS.DEBITAMT = 0;
                    distribAccountPAS.DISTTYPE = 12;
                    sopDistribution.Add(distribAccountPAS);

                    log.WriteLog(0, "Cuenta de comisiones GASTO " + distribAccountEXP.ACTNUMST);
                    log.WriteLog(0, "Cuenta de comisiones PAGO " + distribAccountPAS.ACTNUMST);
                }


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Reserva de riesgos en curso
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                decimal percent;
                if (policyInfo.VEHICLE_TYPE.Contains("TRANSP") || policyInfo.POLICY_TYPE.ToUpper().Contains("TRANSP"))
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPERTRANS"));
                }
                else
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPER"));
                }

                log.WriteLog(0, "Porcentaje Reserva Riesgos en curso: " + percent.ToString());


                //Expense
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var sopDistributionRRCEXP = new SOPDistribution();
                sopDistributionRRCEXP.ACTNUMST = accountB.BuildUnitAccount(8, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID); //accountB.BuildAccountRRC(8, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID, item.ITEMDESC);

                sopDistributionRRCEXP.SOPNUMBE = header.SOPNUMBE;
                sopDistributionRRCEXP.SOPTYPE = header.SOPTYPE;
                sopDistributionRRCEXP.CUSTNMBR = header.CUSTNMBR;
                sopDistributionRRCEXP.DistRef = "Reservas para RRC presente ejercicio";
                sopDistributionRRCEXP.CRDTAMNT = 0;
                sopDistributionRRCEXP.DEBITAMT = Math.Round((itemlist.Sum(x => x.XTNDPRCE.GetValueOrDefault()) * (percent / 100)), 2);
                sopDistributionRRCEXP.DISTTYPE = 13;
                log.WriteLog(0, "Cuenta para RRC GASTO: " + sopDistributionRRCEXP.ACTNUMST);

                sopDistribution.Add(sopDistributionRRCEXP);

                //Pasivo
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var sopDistributionRRCPAS = new SOPDistribution();

                sopDistributionRRCPAS.ACTNUMST = accountB.BuildUnitAccount(9, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                sopDistributionRRCPAS.SOPNUMBE = header.SOPNUMBE;
                sopDistributionRRCPAS.SOPTYPE = header.SOPTYPE;
                sopDistributionRRCPAS.CUSTNMBR = header.CUSTNMBR;
                sopDistributionRRCPAS.DistRef = "Reservas para RRC ";
                sopDistributionRRCPAS.CRDTAMNT = Math.Round((itemlist.Sum(x => x.XTNDPRCE.GetValueOrDefault()) * (percent / 100)), 2);
                sopDistributionRRCPAS.DEBITAMT = 0;

                sopDistributionRRCPAS.DISTTYPE = 13;

                log.WriteLog(0, "Cuenta para RRC PASIVO: " + sopDistributionRRCPAS.ACTNUMST);
                sopDistribution.Add(sopDistributionRRCPAS);




                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Reaseguro y RRC para reaseguro
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                /////reaseguro por cobertura

                var sopDistributionREEXP = new SOPDistribution();
                var sopDistributionRECOMM = new SOPDistribution();
                if (coverageList.Count > 0)
                {
                    foreach (SYSFLEX_REINSURANCE_BY_COVERAGE item in coverageList)
                    {
                        bool IsLocal;
                        bool IsFacult;
                        var typeAccount = 0;
                        if (item.Company_is_Local == 1)
                        {
                            IsLocal = true;
                        }
                        else
                        {
                            IsLocal = false;
                        }


                        if (item.Type_Contrat == "Contractual u Obligatorio")
                        {
                            IsFacult = false;
                        }
                        else
                        {
                            IsFacult = true;
                        }

                        if (IsLocal && !IsFacult)
                        {
                            typeAccount = 10;
                        }

                        else if (IsLocal && IsFacult)
                        {
                            typeAccount = 14;
                        }

                        else if (!IsLocal && IsFacult)
                        {
                            typeAccount = 15;
                        }
                        else //(!IsLocal && !IsFacult)
                        {
                            typeAccount = 13;
                        }

                        sopDistributionREEXP.ACTNUMST = accountB.BuildUnitAccount(typeAccount, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                        sopDistributionREEXP.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionREEXP.SOPTYPE = header.SOPTYPE;
                        sopDistributionREEXP.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionREEXP.DistRef = "Primas de reaseguro contractuales";
                        sopDistributionREEXP.CRDTAMNT = 0;
                        sopDistributionREEXP.DEBITAMT += Math.Round(item.Coverage_Amount.GetValueOrDefault(), 2);

                        sopDistributionREEXP.DISTTYPE = 13;



                        ///////////////////////////////////////////////////////////////
                        ////comision reaseguro
                        if (IsFacult)
                        {
                            typeAccount = 17;
                        }
                        else
                        {
                            typeAccount = 16;
                        }

                        sopDistributionRECOMM.ACTNUMST = accountB.BuildUnitAccount(typeAccount, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);

                        sopDistributionRECOMM.SOPNUMBE = header.SOPNUMBE;
                        sopDistributionRECOMM.SOPTYPE = header.SOPTYPE;
                        sopDistributionRECOMM.CUSTNMBR = header.CUSTNMBR;
                        sopDistributionRECOMM.DistRef = "Comision sobre prima cedida";

                        sopDistributionRECOMM.CRDTAMNT += Math.Round(item.Reinsurance_Amount_Commission.GetValueOrDefault(), 2);
                        sopDistributionRECOMM.DEBITAMT = 0;

                        sopDistributionRECOMM.DISTTYPE = 13;


                    }
                    sopDistributionREEXP.DEBITAMT = coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2));
                    sopDistributionREEXP.CRDTAMNT = 0;

                    sopDistributionRECOMM.CRDTAMNT = coverageList.Sum(x => Math.Round(x.Reinsurance_Amount_Commission.GetValueOrDefault(), 2));
                    sopDistributionRECOMM.DEBITAMT = 0;

                    sopDistribution.Add(sopDistributionRECOMM);
                    log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + sopDistributionRECOMM.ACTNUMST);

                    log.WriteLog(0, "Reaseguro Contractual: " + sopDistributionREEXP.ACTNUMST);
                    sopDistribution.Add(sopDistributionREEXP);


                    ///Pasivo Reaseguro ----cuenta corriente
                    var sopDistributionREPAS = new SOPDistribution();
                    sopDistributionREPAS.ACTNUMST = accountB.BuildUnitAccount(11, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    sopDistributionREPAS.SOPNUMBE = header.SOPNUMBE;
                    sopDistributionREPAS.SOPTYPE = header.SOPTYPE;
                    sopDistributionREPAS.CUSTNMBR = header.CUSTNMBR;
                    sopDistributionREPAS.DistRef = "Reaseguradores del exterior – cuenta corriente";
                    sopDistributionREPAS.CRDTAMNT = coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2));
                    sopDistributionREPAS.DEBITAMT = 0;
                    sopDistributionREPAS.DISTTYPE = 13;
                    sopDistribution.Add(sopDistributionREPAS);
                    log.WriteLog(0, "Reaseguradores del exterior – cuenta corriente: " + sopDistributionRECOMM.ACTNUMST);


                    ////////////////////////////////////////////////////
                    ///Pasivo Comision Reaseguro
                    var sopDistributionREPASCOMM = new SOPDistribution();
                    sopDistributionREPASCOMM.ACTNUMST = accountB.BuildUnitAccount(11, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);

                    sopDistributionREPASCOMM.SOPNUMBE = header.SOPNUMBE;
                    sopDistributionREPASCOMM.SOPTYPE = header.SOPTYPE;
                    sopDistributionREPASCOMM.CUSTNMBR = header.CUSTNMBR;
                    sopDistributionREPASCOMM.DistRef = "Comision sobre prima cedida";
                    sopDistributionREPASCOMM.CRDTAMNT = 0;
                    sopDistributionREPASCOMM.DEBITAMT = coverageList.Sum(x => Math.Round(x.Reinsurance_Amount_Commission.GetValueOrDefault(), 2));
                    sopDistributionREPASCOMM.DISTTYPE = 13;
                    log.WriteLog(0, "Reaseguradores Comision por Reaseguro Pasivo: " + sopDistributionREPASCOMM.ACTNUMST);
                    sopDistribution.Add(sopDistributionREPASCOMM);


                    //Income
                    var sopDistributionREARRCEXP = new SOPDistribution();



                    sopDistributionREARRCEXP.ACTNUMST = accountB.BuildUnitAccount(12, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);

                    sopDistributionREARRCEXP.SOPNUMBE = header.SOPNUMBE;
                    sopDistributionREARRCEXP.SOPTYPE = header.SOPTYPE;
                    sopDistributionREARRCEXP.CUSTNMBR = header.CUSTNMBR;
                    sopDistributionREARRCEXP.DistRef = "Reservas para RRC a cargo reaseguro presente ejercicio";
                    sopDistributionREARRCEXP.CRDTAMNT = coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2));
                    sopDistributionREARRCEXP.DEBITAMT = 0;

                    sopDistributionREARRCEXP.DISTTYPE = 13;

                    log.WriteLog(0, "RRC para reaseguro INGRESO: " + sopDistributionREARRCEXP.ACTNUMST);




                    sopDistribution.Add(sopDistributionREARRCEXP);

                    //Pasivo
                    var sopDistributionREARRCPAS = new SOPDistribution();
                    sopDistributionREARRCPAS.ACTNUMST = accountB.BuildUnitAccount(9, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    sopDistributionREARRCPAS.SOPNUMBE = header.SOPNUMBE;
                    sopDistributionREARRCPAS.SOPTYPE = header.SOPTYPE;
                    sopDistributionREARRCPAS.CUSTNMBR = header.CUSTNMBR;
                    sopDistributionREARRCPAS.DistRef = "Reservas para RRC a cargo de reaseguro ";
                    sopDistributionREARRCPAS.CRDTAMNT = 0;
                    sopDistributionREARRCPAS.DEBITAMT = coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2));
                    sopDistributionREARRCPAS.DISTTYPE = 13;
                    log.WriteLog(0, "RRC para reaseguro PASIVO: " + sopDistributionREARRCPAS.ACTNUMST);
                    sopDistribution.Add(sopDistributionREARRCPAS);
                }

                return sopDistribution.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionCoverage"></param>
        /// <param name="detail"></param>
        /// <param name="commission"></param>
        /// <param name="taxes"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public RMTransactionDist[] CreateRMUnitDistribution(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionCoverage, RMDocumentTaxes[] taxes, RMTransactionHeader header)
        {
            var accountB = new AccountBuilder();
            var policy = new PolicyDAL();
            var invoicedal = new InvoiceDAL();
            var taxlist = taxes.ToList();
            var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
            var reinsuranceCoverageList = invoicedal.GetReinsuranceByCoverage(policyTransaction.POLICY_NUMBER.Trim(), policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());

            var rmDistribution = new List<RMTransactionDist>();

            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas por cobrar
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //var sopDistributionCXC = new SOPDistribution();

                var rmdistributionCxC = new RMTransactionDist();
                rmdistributionCxC.ACTNUMST = accountB.BuildUnitAccount(1, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);//accountB.BuildARAccount(1, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                rmdistributionCxC.DOCNUMBR = header.DOCNUMBR;
                rmdistributionCxC.RMDTYPAL = header.RMDTYPAL;
                rmdistributionCxC.CUSTNMBR = header.CUSTNMBR;
                rmdistributionCxC.DistRef = "Primas por Cobrar";
                rmdistributionCxC.CRDTAMNT = Math.Abs(header.DOCAMNT);
                rmdistributionCxC.DEBITAMT = 0;
                rmdistributionCxC.DISTTYPE = 3;
                rmDistribution.Add(rmdistributionCxC);

                log.WriteLog(0, "Cuenta por cobrar: " + rmdistributionCxC.ACTNUMST);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Extraer cuentas de los items
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                var rmItemsAccount = new RMTransactionDist();

                rmItemsAccount.ACTNUMST = accountB.BuildUnitAccount(2, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                rmItemsAccount.DOCNUMBR = header.DOCNUMBR;
                rmItemsAccount.RMDTYPAL = header.RMDTYPAL;
                rmItemsAccount.CUSTNMBR = header.CUSTNMBR;
                rmItemsAccount.DistRef = "PRIMAS SUSCRITAS";
                rmItemsAccount.CRDTAMNT = 0;
                rmItemsAccount.DEBITAMT = Math.Abs(transactionCoverage.Sum(x => x.Coverage_Amount.GetValueOrDefault()));
                rmItemsAccount.DISTTYPE = 19;
                rmDistribution.Add(rmItemsAccount);

                log.WriteLog(0, "Cuenta de producto: " + rmItemsAccount.ACTNUMST);


                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas de impuestos
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                foreach (RMDocumentTaxes item in taxlist)
                {
                    var TaxdistribAccount = new RMTransactionDist();


                    TaxdistribAccount.ACTNUMST = item.ACTNUMST;
                    TaxdistribAccount.DOCNUMBR = item.DOCNUMBR;
                    TaxdistribAccount.RMDTYPAL = item.RMDTYPAL;
                    TaxdistribAccount.CUSTNMBR = item.CUSTNMBR;
                    TaxdistribAccount.DistRef = "ISC sobre prima pendiente de cobro";

                    TaxdistribAccount.CRDTAMNT = 0;
                    TaxdistribAccount.DEBITAMT = Math.Abs(item.TAXAMNT);

                    TaxdistribAccount.DISTTYPE = 13;
                    rmDistribution.Add(TaxdistribAccount);

                    log.WriteLog(0, "Cuenta de impuestos: " + TaxdistribAccount.ACTNUMST);
                }


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Extraer cuentas de comisiones
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                //var distribAccountEXP = new SOPClass.SOPDistribution();
                //var distribAccountPAS = new SOPClass.SOPDistribution();

                if (!ChannelNoComm(policyInfo.CHANNEL.Trim()))
                {

                    var rmCommissionEXP = new RMTransactionDist();
                    var rmCommissionPAS = new RMTransactionDist();

                    var htComisiones = new Hashtable();
                    foreach (TRANSACTION_BY_COVERAGE itemcoverage in transactionCoverage)
                    {

                        if (htComisiones.Contains("UnitCommission"))
                        {
                            htComisiones["UnitCommission"] = (decimal)htComisiones["UnitCommission"] + invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault());
                        }
                        else
                        {
                            htComisiones.Add("UnitCommission", invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault()));
                        }
                    }

                    ///////////////////////////////////////////////////////////////////////////////////
                    //Cuenta de gasto
                    ///////////////////////////////////////////////////////////////////////////////////
                    rmCommissionEXP.ACTNUMST = accountB.BuildUnitAccount(4, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);// accountB.BuildAccountCommission(4, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    rmCommissionEXP.DOCNUMBR = header.DOCNUMBR;
                    rmCommissionEXP.RMDTYPAL = header.RMDTYPAL;
                    rmCommissionEXP.CUSTNMBR = header.CUSTNMBR;
                    rmCommissionEXP.DistRef = "Comisiones intermediarios";
                    rmCommissionEXP.CRDTAMNT = Math.Abs((decimal)htComisiones["UnitCommission"]);
                    rmCommissionEXP.DEBITAMT = 0;
                    rmCommissionEXP.DISTTYPE = 8;
                    rmDistribution.Add(rmCommissionEXP);



                    ///////////////////////////////////////////////////////////////////////////////////////
                    //Cuenta de Pasivo
                    ///////////////////////////////////////////////////////////////////////////////////////
                    rmCommissionPAS.ACTNUMST = accountB.BuildUnitAccount(5, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID); //accountB.BuildAccountCommission(5, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, policyInfo.ZONE, gpCustomer.COUNTRY, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                    rmCommissionPAS.DOCNUMBR = header.DOCNUMBR;
                    rmCommissionPAS.RMDTYPAL = header.RMDTYPAL;
                    rmCommissionPAS.CUSTNMBR = header.CUSTNMBR;
                    rmCommissionPAS.DistRef = "Comisiones a intermediarios pendientes de cobro";
                    rmCommissionPAS.CRDTAMNT = 0;
                    rmCommissionPAS.DEBITAMT = Math.Abs((decimal)htComisiones["UnitCommission"]);
                    rmCommissionPAS.DISTTYPE = 8;
                    rmDistribution.Add(rmCommissionPAS);

                    log.WriteLog(0, "Cuenta de comisiones GASTO " + rmCommissionEXP.ACTNUMST);
                    log.WriteLog(0, "Cuenta de comisiones PAGO " + rmCommissionPAS.ACTNUMST);

                    htComisiones.Clear();
                }


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Reserva de riesgos en curso
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                decimal percent;
                if (policyInfo.VEHICLE_TYPE.Contains("TRANSP") || policyInfo.POLICY_TYPE.ToUpper().Contains("TRANSP"))
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPERTRANS"));
                }
                else
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPER"));
                }
                if (transactionCoverage.Count > 0)
                {

                    var rmRRCEXP = new RMTransactionDist();
                    var rmRRCPAS = new RMTransactionDist();
                    var htRRC = new Hashtable();



                    log.WriteLog(0, "Porcentaje Reserva Riesgos en curso: " + percent.ToString());

                    foreach (TRANSACTION_BY_COVERAGE item in transactionCoverage)
                    {
                        if (htRRC.Contains("UnitRRC"))
                        {
                            htRRC["UnitRRC"] = (decimal)htRRC["UnitRRC"] + Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2);
                        }
                        else
                        {
                            htRRC.Add("UnitRRC", Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2));
                        }
                    }


                    //Expense
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    rmRRCEXP.ACTNUMST = accountB.BuildUnitAccount(8, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmRRCEXP.DOCNUMBR = header.DOCNUMBR;
                    rmRRCEXP.RMDTYPAL = header.RMDTYPAL;
                    rmRRCEXP.CUSTNMBR = header.CUSTNMBR;
                    rmRRCEXP.DistRef = "Reservas para RRC presente ejercicio";
                    rmRRCEXP.CRDTAMNT = Math.Abs((decimal)htRRC["UnitRRC"]);
                    rmRRCEXP.DEBITAMT = 0;
                    rmRRCEXP.DISTTYPE = 8;
                    log.WriteLog(0, "Cuenta para RRC GASTO: " + rmRRCEXP.ACTNUMST);

                    rmDistribution.Add(rmRRCEXP);


                    //Pasivo
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    rmRRCPAS.ACTNUMST = accountB.BuildUnitAccount(9, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmRRCPAS.DOCNUMBR = header.DOCNUMBR;
                    rmRRCPAS.RMDTYPAL = header.RMDTYPAL;
                    rmRRCPAS.CUSTNMBR = header.CUSTNMBR;
                    rmRRCPAS.DistRef = "Reservas para RRC ";
                    rmRRCPAS.CRDTAMNT = 0;
                    rmRRCPAS.DEBITAMT = Math.Abs((decimal)htRRC["UnitRRC"]);
                    rmRRCPAS.DISTTYPE = 8;
                    rmDistribution.Add(rmRRCPAS);


                    log.WriteLog(0, "Cuenta para RRC PASIVO: " + rmRRCPAS.ACTNUMST);

                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Reaseguro y RRC para reaseguro
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





                if (reinsuranceCoverageList.Count > 0)
                {
                    var rmReinsuranceEXP = new RMTransactionDist();
                    var rmReinsurancePAS = new RMTransactionDist();
                    var rmReinsuranceCOMM = new RMTransactionDist();
                    var rmReinsurancePASCOMM = new RMTransactionDist();
                    var rmReinsuranceING = new RMTransactionDist();
                    var rmReinsurancePASING = new RMTransactionDist();
                    var reinsuranceItem = reinsuranceCoverageList.FirstOrDefault();

                    bool IsLocal;
                    bool IsFacult;
                    var typeAccount = 0;
                    if (reinsuranceItem.Company_is_Local == 1)
                    {
                        IsLocal = true;
                    }
                    else
                    {
                        IsLocal = false;
                    }


                    if (reinsuranceItem.Type_Contrat == "Contractual u Obligatorio")
                    {
                        IsFacult = false;
                    }
                    else
                    {
                        IsFacult = true;
                    }

                    if (IsLocal && !IsFacult)
                    {
                        typeAccount = 10;
                    }

                    else if (IsLocal && IsFacult)
                    {
                        typeAccount = 14;
                    }

                    else if (!IsLocal && IsFacult)
                    {
                        typeAccount = 15;
                    }
                    else //(!IsLocal && !IsFacult)
                    {
                        typeAccount = 13;
                    }

                    ////////////////////////////////////////////////////////////
                    /////reaseguro por cobertura
                    ////////////////////////////////
                    rmReinsuranceEXP.ACTNUMST = accountB.BuildUnitAccount(typeAccount, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsuranceEXP.DOCNUMBR = header.DOCNUMBR;
                    rmReinsuranceEXP.RMDTYPAL = header.RMDTYPAL;
                    rmReinsuranceEXP.CUSTNMBR = header.CUSTNMBR;
                    rmReinsuranceEXP.DistRef = "Primas de reaseguro contractuales";
                    rmReinsuranceEXP.CRDTAMNT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2)));
                    rmReinsuranceEXP.DEBITAMT = 0;
                    rmReinsuranceEXP.DISTTYPE = 8;

                    log.WriteLog(0, "Reaseguro Contractual: " + rmReinsuranceEXP.ACTNUMST);
                    rmDistribution.Add(rmReinsuranceEXP);


                    ///////////////////////////////////////////////////////////////
                    ////comision reaseguro
                    if (IsFacult)
                    {
                        typeAccount = 17;
                    }
                    else
                    {
                        typeAccount = 16;
                    }

                    rmReinsuranceCOMM.ACTNUMST = accountB.BuildUnitAccount(typeAccount, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsuranceCOMM.DOCNUMBR = header.DOCNUMBR;
                    rmReinsuranceCOMM.RMDTYPAL = header.RMDTYPAL;
                    rmReinsuranceCOMM.CUSTNMBR = header.CUSTNMBR;
                    rmReinsuranceCOMM.DistRef = "Comision sobre prima cedida";
                    rmReinsuranceCOMM.DISTTYPE = 8;
                    rmReinsuranceCOMM.CRDTAMNT = 0;
                    rmReinsuranceCOMM.DEBITAMT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Reinsurance_Amount_Commission.GetValueOrDefault(), 2)));

                    rmDistribution.Add(rmReinsuranceCOMM);
                    log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + rmReinsuranceCOMM.ACTNUMST);



                    ///Pasivo Reaseguro ----cuenta corriente

                    rmReinsurancePAS.ACTNUMST = accountB.BuildUnitAccount(11, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsurancePAS.DOCNUMBR = header.DOCNUMBR;
                    rmReinsurancePAS.RMDTYPAL = header.RMDTYPAL;
                    rmReinsurancePAS.CUSTNMBR = header.CUSTNMBR;
                    rmReinsurancePAS.DistRef = "Reaseguradores del exterior – cuenta corriente";
                    rmReinsurancePAS.CRDTAMNT = 0;
                    rmReinsurancePAS.DEBITAMT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2)));;
                    rmReinsurancePAS.DISTTYPE = 8;
                    rmDistribution.Add(rmReinsurancePAS);
                    log.WriteLog(0, "Reaseguradores del exterior – cuenta corriente: " + rmReinsurancePAS.ACTNUMST);


                    ////////////////////////////////////////////////////
                    ///Pasivo Comision Reaseguro

                    rmReinsurancePASCOMM.ACTNUMST = accountB.BuildUnitAccount(11, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsurancePASCOMM.DOCNUMBR = header.DOCNUMBR;
                    rmReinsurancePASCOMM.RMDTYPAL = header.RMDTYPAL;
                    rmReinsurancePASCOMM.CUSTNMBR = header.CUSTNMBR;
                    rmReinsurancePASCOMM.DistRef = "Comision sobre prima cedida";
                    rmReinsurancePASCOMM.CRDTAMNT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Reinsurance_Amount_Commission.GetValueOrDefault(), 2)));
                    rmReinsurancePASCOMM.DEBITAMT = 0;
                    rmReinsurancePASCOMM.DISTTYPE = 8;
                    rmDistribution.Add(rmReinsurancePASCOMM);
                    log.WriteLog(0, "Reaseguradores Comision por Reaseguro Pasivo: " + rmReinsurancePASCOMM.ACTNUMST);

                    //////////////////////////////////////////////////
                    //Income
                    rmReinsuranceING.ACTNUMST = accountB.BuildUnitAccount(12, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsuranceING.DOCNUMBR = header.DOCNUMBR;
                    rmReinsuranceING.RMDTYPAL = header.RMDTYPAL;
                    rmReinsuranceING.CUSTNMBR = header.CUSTNMBR;
                    rmReinsuranceING.DistRef = "Reservas para RRC a cargo reaseguro presente ejercicio";
                    rmReinsuranceING.CRDTAMNT = 0;
                    rmReinsuranceING.DEBITAMT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2)));
                    rmReinsuranceING.DISTTYPE = 8;
                    rmDistribution.Add(rmReinsuranceING);
                    log.WriteLog(0, "RRC para reaseguro INGRESO: " + rmReinsuranceING.ACTNUMST);

                    /////////////////////////////////////////////////
                    //Pasivo
                    //var sopDistributionREARRCPAS = new SOPDistribution();
                    rmReinsurancePASING.ACTNUMST = accountB.BuildUnitAccount(9, policyInfo.LINE_OF_BUSINESS.Trim(), policyInfo.POLICY_TYPE.Trim(), policyInfo.VEHICLE_TYPE.Trim(), policyInfo.GpProductID);
                    rmReinsurancePASING.DOCNUMBR = header.DOCNUMBR;
                    rmReinsurancePASING.RMDTYPAL = header.RMDTYPAL;
                    rmReinsurancePASING.CUSTNMBR = header.CUSTNMBR;
                    rmReinsurancePASING.DistRef = "Reservas para RRC a cargo de reaseguro ";
                    rmReinsurancePASING.CRDTAMNT = Math.Abs(reinsuranceCoverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault() * (percent / 100), 2)));
                    rmReinsurancePASING.DEBITAMT = 0;
                    rmReinsurancePASING.DISTTYPE = 8;
                    rmDistribution.Add(rmReinsurancePASING);
                    log.WriteLog(0, "RRC para reaseguro PASIVO: " + rmReinsurancePASING.ACTNUMST);

                }

                return rmDistribution.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyInfo"></param>
        /// <param name="policyTransaction"></param>
        /// <param name="transactionCoverage"></param>
        /// <param name="taxes"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public RMTransactionDist[] CreateRMDistribution(SYSFLEX_POLICY policyInfo, TRANSACTION_INVOICE policyTransaction, List<TRANSACTION_BY_COVERAGE> transactionCoverage, RMDocumentTaxes[] taxes, RMTransactionHeader header)
        {
            var accountB = new AccountBuilder();
            var policy = new PolicyDAL();
            var invoicedal = new InvoiceDAL();
            var taxlist = taxes.ToList();
            var gpCustomer = policy.GetGPCustomer(policyInfo.POLICY_NUMBER.Trim());
            //var channel_no_comm = ConfigKey.ReadSetting("CHANNEL_NO_COMM");
            var coverageList = invoicedal.GetReinsuranceByCoverage(policyTransaction.POLICY_NUMBER.Trim(), policyTransaction.TRANSACTION_SEQUENCE.GetValueOrDefault());
            var productlist = transactionCoverage.Select(x => new { x.Product_type, x.VEHICLE_TYPE }).Distinct().ToList();

            /////Pais Predeterminado
            string defaultCountry = ConfigKey.ReadSetting("DEFAULTCOUNTRY");

            var rmCountry = string.Empty;
            if (string.IsNullOrEmpty(gpCustomer.COUNTRY) || string.IsNullOrWhiteSpace(gpCustomer.COUNTRY) || gpCustomer.COUNTRY.Trim() == "Dominicana")
            {
                rmCountry = defaultCountry.Trim();
            }
            else
            {
                rmCountry = gpCustomer.COUNTRY;
            }

            var rmDistribution = new List<RMTransactionDist>();

            try
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Extraer cuentas por cobrar
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var rmDistributionCXC = new RMTransactionDist();
                rmDistributionCXC.ACTNUMST = accountB.BuildARAccount(1, policyInfo.LINE_OF_BUSINESS, policyInfo.POLICY_TYPE, policyInfo.VEHICLE_TYPE, rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                rmDistributionCXC.DOCNUMBR = header.DOCNUMBR;
                rmDistributionCXC.RMDTYPAL = header.RMDTYPAL;
                rmDistributionCXC.CUSTNMBR = header.CUSTNMBR;
                rmDistributionCXC.DistRef = "Primas por Cobrar";

                rmDistributionCXC.CRDTAMNT = Math.Abs(header.DOCAMNT);
                rmDistributionCXC.DEBITAMT = 0;

                rmDistributionCXC.DISTTYPE = 3;

                rmDistribution.Add(rmDistributionCXC);

                log.WriteLog(0, "Cuenta por cobrar: " + rmDistributionCXC.ACTNUMST);



                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Extraer cuentas de impuestos
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                foreach (RMDocumentTaxes item in taxlist)
                {
                    var distribAccount = new RMTransactionDist();


                    distribAccount.ACTNUMST = item.ACTNUMST;
                    distribAccount.DOCNUMBR = item.DOCNUMBR;
                    distribAccount.RMDTYPAL = item.RMDTYPAL;
                    distribAccount.CUSTNMBR = item.CUSTNMBR;
                    distribAccount.DistRef = "ISC sobre prima pendiente de cobro";

                    distribAccount.CRDTAMNT = 0;
                    distribAccount.DEBITAMT = Math.Abs(item.TAXAMNT);
                    distribAccount.DISTTYPE = 13;
                    rmDistribution.Add(distribAccount);

                    log.WriteLog(0, "Cuenta de impuestos: " + distribAccount.ACTNUMST);
                }


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////Extraer Cuenta de productos
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                foreach (TRANSACTION_BY_COVERAGE item in transactionCoverage)
                {
                    if (item.Coverage_Group != "Impuestos")
                    {
                        var distribAccount = new RMTransactionDist();


                        distribAccount.ACTNUMST = accountB.BuildSalesAccount(2, policyInfo.LINE_OF_BUSINESS.Trim(),
                                                                             item.Product_type.Trim(),
                                                                             item.VEHICLE_TYPE.Trim(),
                                                                             rmCountry.Trim(),
                                                                             policyInfo.CHANNEL.Trim(),
                                                                             policyInfo.SUPERVISOR.Trim(),
                                                                             policyInfo.SUPERVISOR_CODE,
                                                                             item.Coverage_Desc.Trim(), policyInfo.GpProductID);
                        distribAccount.DOCNUMBR = header.DOCNUMBR;
                        distribAccount.RMDTYPAL = header.RMDTYPAL;
                        distribAccount.CUSTNMBR = header.CUSTNMBR;
                        distribAccount.DistRef = item.Coverage_Desc.Trim();

                        distribAccount.CRDTAMNT = 0;
                        distribAccount.DEBITAMT = Math.Round(item.Coverage_Amount.GetValueOrDefault(), 2);
                        distribAccount.DISTTYPE = 19;
                        rmDistribution.Add(distribAccount);

                        log.WriteLog(0, "Cuenta de Producto: " + distribAccount.ACTNUMST);
                    }
                }



                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Extraer cuentas de comisiones
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // if (!channel_no_comm.Contains(policyInfo.CHANNEL.Trim()))
                if (!ChannelNoComm(policyInfo.CHANNEL.Trim()))
                {
                    /////////////////////////////
                    ///Cuenta de gasto
                    ////////////////////////////
                    if (transactionCoverage.Count > 0)
                    {
                        var htComisiones = new Hashtable();
                        foreach (TRANSACTION_BY_COVERAGE itemcoverage in transactionCoverage)
                        {
                            if (itemcoverage.Coverage_Group != "Impuestos" && itemcoverage.Coverage_Amount.GetValueOrDefault() > 0)
                            {
                                var distribAccountEXP = new RMTransactionDist();
                                distribAccountEXP.ACTNUMST = accountB.BuildAccountCommission(4, policyInfo.LINE_OF_BUSINESS, itemcoverage.Product_type.Trim(), itemcoverage.VEHICLE_TYPE, rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, itemcoverage.Coverage_Desc.Trim(), policyInfo.GpProductID);
                                distribAccountEXP.DOCNUMBR = header.DOCNUMBR;
                                distribAccountEXP.RMDTYPAL = header.RMDTYPAL;
                                distribAccountEXP.CUSTNMBR = header.CUSTNMBR;
                                distribAccountEXP.DistRef = "Comisiones intermediarios";
                                distribAccountEXP.CRDTAMNT = invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), Math.Round(itemcoverage.Coverage_Amount.GetValueOrDefault(), 2)); //item.COMMAMNT;
                                distribAccountEXP.DEBITAMT = 0;
                                distribAccountEXP.DISTTYPE = 8;

                                if (distribAccountEXP.CRDTAMNT.GetValueOrDefault() > 0)
                                {
                                    rmDistribution.Add(distribAccountEXP);
                                    log.WriteLog(0, "Cuenta de comisiones GASTO " + distribAccountEXP.ACTNUMST);
                                }


                            }
                            if (htComisiones.Contains(itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()))
                            {
                                htComisiones[itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()] = (decimal)htComisiones[itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim()] + invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault());
                            }
                            else
                            {
                                htComisiones.Add(itemcoverage.Product_type.Trim() + itemcoverage.VEHICLE_TYPE.Trim(), invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), itemcoverage.Product_type.Trim(), policyInfo.CHANNEL.Trim(), itemcoverage.Coverage_Amount.GetValueOrDefault()));
                            }

                        }


                        //////////////////////////////
                        /////Cuenta de Pasivo
                        /////////////////////////////
                        foreach (var PL in productlist)
                        {
                            decimal sumcom = transactionCoverage.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => Math.Round(z.Coverage_Amount.GetValueOrDefault(), 2));

                            var distribAccountPAS = new RMTransactionDist();
                            distribAccountPAS.ACTNUMST = accountB.BuildAccountCommission(5, policyInfo.LINE_OF_BUSINESS, PL.Product_type.Trim(), PL.VEHICLE_TYPE.Trim(), rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                            distribAccountPAS.DOCNUMBR = header.DOCNUMBR;
                            distribAccountPAS.RMDTYPAL = header.RMDTYPAL;
                            distribAccountPAS.CUSTNMBR = header.CUSTNMBR;
                            distribAccountPAS.DistRef = "Comisiones a intermediarios pendientes de cobro";
                            distribAccountPAS.CRDTAMNT = 0;
                            distribAccountPAS.DEBITAMT = (decimal)htComisiones[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];// invoicedal.GetCommission(policyInfo.AGENT_CODE.ToString(), PL.Product_type.Trim(), policyInfo.CHANNEL.Trim(), sumcom);//item.COMMAMNT;
                            distribAccountPAS.DISTTYPE = 8;



                            if (distribAccountPAS.DEBITAMT.GetValueOrDefault() > 0)
                            {
                                rmDistribution.Add(distribAccountPAS);
                                log.WriteLog(0, "Cuenta de comisiones PAGO " + distribAccountPAS.ACTNUMST);
                            }


                            //rmDistribution.Add(distribAccountPAS);
                            //log.escribirLog(0, "Cuenta de comisiones PAGO " + distribAccountPAS.ACTNUMST);
                        }
                        htComisiones.Clear();
                    }
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Reserva de riesgos en curso
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                decimal percent;
                //var rmDistributionRRCPAS = new RMTransactionDist();
                //rmDistributionRRCPAS.DEBITAMT = 0;
                if (policyInfo.VEHICLE_TYPE.Contains("TRANSP") || policyInfo.POLICY_TYPE.ToUpper().Contains("TRANSP"))
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPERTRANS"));
                }
                else
                {
                    percent = Convert.ToDecimal(ConfigKey.ReadSetting("RRCPER"));
                }

                log.WriteLog(0, "Porcentaje Reserva Riesgos en curso: " + percent.ToString());


                if (transactionCoverage.Count > 0)
                {
                    var htRRC = new Hashtable();
                    //Expense
                    foreach (TRANSACTION_BY_COVERAGE item in transactionCoverage)
                    {
                        if (item.Coverage_Group != "Impuestos")
                        {
                            var rmDistributionRRCEXP = new RMTransactionDist();
                            rmDistributionRRCEXP.ACTNUMST = accountB.BuildAccountRRC(8, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                      rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID, item.Coverage_Desc);
                            rmDistributionRRCEXP.DOCNUMBR = header.DOCNUMBR;
                            rmDistributionRRCEXP.RMDTYPAL = header.RMDTYPAL;
                            rmDistributionRRCEXP.CUSTNMBR = header.CUSTNMBR;
                            rmDistributionRRCEXP.DistRef = "Reservas para RRC presente ejercicio";

                            rmDistributionRRCEXP.CRDTAMNT = Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2);
                            rmDistributionRRCEXP.DEBITAMT = 0;
                            rmDistributionRRCEXP.DISTTYPE = 8;

                            if (rmDistributionRRCEXP.CRDTAMNT.GetValueOrDefault() > 0)
                            {
                                log.WriteLog(0, "Cuenta para RRC GASTO: " + rmDistributionRRCEXP.ACTNUMST);
                                rmDistribution.Add(rmDistributionRRCEXP);
                            }
                        }
                        if (htRRC.Contains(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()))
                        {
                            htRRC[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] = (decimal)htRRC[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] + Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2);
                        }
                        else
                        {
                            htRRC.Add(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim(), Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2));
                        }
                    }


                    foreach (var PL in productlist)
                    {
                        decimal sumcom = transactionCoverage.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => Math.Round(z.Coverage_Amount.GetValueOrDefault(), 2));
                        //Pasivo

                        var rmDistributionRRCPAS = new RMTransactionDist();
                        rmDistributionRRCPAS.ACTNUMST = accountB.BuildARAccount(9, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                              rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        rmDistributionRRCPAS.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionRRCPAS.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionRRCPAS.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionRRCPAS.DistRef = "Pasivo RRC ";
                        rmDistributionRRCPAS.CRDTAMNT = 0;
                        //rmDistributionRRCPAS.DEBITAMT = Math.Round(sumcom * (percent / 100), 2);
                        rmDistributionRRCPAS.DEBITAMT = (decimal)htRRC[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];
                        rmDistributionRRCPAS.DISTTYPE = 8;

                        log.WriteLog(0, "Cuenta para RRC PASIVO: " + rmDistributionRRCPAS.ACTNUMST);
                        rmDistribution.Add(rmDistributionRRCPAS);
                    }
                    htRRC.Clear();
                }

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///Reaseguro y RRC para reaseguro
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (coverageList.Count > 0)
                {
                    var productlistCoverage = coverageList.Select(x => new { x.Product_type, x.VEHICLE_TYPE }).Distinct().ToList();

                    /////reaseguro por cobertura
                    foreach (SYSFLEX_REINSURANCE_BY_COVERAGE item in coverageList)
                    {
                        var rmDistributionREEXP = new RMTransactionDist();
                        var rmDistributionRECOMM = new RMTransactionDist();
                        bool IsLocal;
                        bool IsFacult;

                        if (item.Company_is_Local == 1)
                        {
                            IsLocal = true;
                        }
                        else
                        {
                            IsLocal = false;
                        }


                        if (item.Type_Contrat == "Contractual u Obligatorio")
                        {
                            IsFacult = false;
                        }
                        else
                        {
                            IsFacult = true;
                        }

                        rmDistributionREEXP.ACTNUMST = accountB.BuildReinsuranceAccount(10, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                         rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, item.Coverage_Desc,
                                                                                         policyInfo.GpProductID, IsLocal, IsFacult);
                        rmDistributionREEXP.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionREEXP.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionREEXP.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionREEXP.DistRef = "Primas de reaseguro contractuales";

                        rmDistributionREEXP.CRDTAMNT = Math.Abs(Math.Round(item.Coverage_Amount.GetValueOrDefault(), 2));
                        rmDistributionREEXP.DEBITAMT = 0;

                        rmDistributionREEXP.DISTTYPE = 8;

                        if (rmDistributionREEXP.CRDTAMNT.GetValueOrDefault() > 0)
                        {
                            log.WriteLog(0, "Reaseguro Contractual: " + rmDistributionREEXP.ACTNUMST);
                            rmDistribution.Add(rmDistributionREEXP);
                        }


                        ////comision reaseguro
                        rmDistributionRECOMM.ACTNUMST = accountB.BuildReinsuranceCommAccount(10, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                         rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, item.Coverage_Desc,
                                                                                         policyInfo.GpProductID, IsFacult);

                        rmDistributionRECOMM.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionRECOMM.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionRECOMM.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionRECOMM.DistRef = "Comision sobre prima cedida";
                        rmDistributionRECOMM.CRDTAMNT = 0;
                        rmDistributionRECOMM.DEBITAMT = Math.Abs(Math.Round(item.Reinsurance_Amount_Commission.GetValueOrDefault(), 2));
                        rmDistributionRECOMM.DISTTYPE = 8;
                        if (rmDistributionRECOMM.DEBITAMT.GetValueOrDefault() > 0)
                        {
                            rmDistribution.Add(rmDistributionRECOMM);
                            log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + rmDistributionRECOMM.ACTNUMST);
                        }



                    }

                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => Math.Round(z.Coverage_Amount.GetValueOrDefault(), 2));

                        ///Pasivo Reaseguro ----cuenta corriente
                        var RMDistributionREPAS = new RMTransactionDist();
                        RMDistributionREPAS.ACTNUMST = accountB.BuildARAccount(11, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                   rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        RMDistributionREPAS.DOCNUMBR = header.DOCNUMBR;
                        RMDistributionREPAS.RMDTYPAL = header.RMDTYPAL;
                        RMDistributionREPAS.CUSTNMBR = header.CUSTNMBR;
                        RMDistributionREPAS.DistRef = "Reaseguradores del exterior – cuenta corriente";
                        RMDistributionREPAS.CRDTAMNT = 0;//coverageList.Sum(x => Math.Round(x.Coverage_Amount.GetValueOrDefault(), 2));
                        RMDistributionREPAS.DEBITAMT = sumcom;
                        RMDistributionREPAS.DISTTYPE = 8;



                        if (RMDistributionREPAS.DEBITAMT.GetValueOrDefault() > 0)
                        {
                            rmDistribution.Add(RMDistributionREPAS);
                            log.WriteLog(0, "Reaseguradores - cuenta corriente: " + RMDistributionREPAS.ACTNUMST);
                        }


                        //log.escribirLog(0, "Reaseguradores - cuenta corriente: " + RMDistributionREPAS.ACTNUMST);
                        //rmDistribution.Add(RMDistributionREPAS);
                    }

                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => Math.Round(Math.Abs(z.Reinsurance_Amount_Commission.GetValueOrDefault()), 2));
                        ///Pasivo Comision Reaseguro
                        var rmDistributionREPASCOMM = new RMTransactionDist();
                        rmDistributionREPASCOMM.ACTNUMST = accountB.BuildARAccount(11, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                   rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        rmDistributionREPASCOMM.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionREPASCOMM.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionREPASCOMM.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionREPASCOMM.DistRef = "Comision sobre prima cedida";
                        rmDistributionREPASCOMM.CRDTAMNT = sumcom;//Math.Abs(Math.Round(coverageList.Sum(x => x.Reinsurance_Amount_Commission.GetValueOrDefault()), 2));
                        rmDistributionREPASCOMM.DEBITAMT = 0;
                        rmDistributionREPASCOMM.DISTTYPE = 8;


                        if (rmDistributionREPASCOMM.CRDTAMNT.GetValueOrDefault() > 0)
                        {
                            rmDistribution.Add(rmDistributionREPASCOMM);
                            log.WriteLog(0, "Reaseguradores Comision por Reaseguro: " + rmDistributionREPASCOMM.ACTNUMST);
                        }


                        //log.escribirLog(0, "Reaseguradores Comision por Reaseguro: " + rmDistributionREPASCOMM.ACTNUMST);
                        //rmDistribution.Add(rmDistributionREPASCOMM);

                    }

                    var htrrcReaseguro = new Hashtable();
                    //Income
                    foreach (SYSFLEX_REINSURANCE_BY_COVERAGE item in coverageList)
                    {
                        var rmDistributionREARRCEXP = new RMTransactionDist();
                        rmDistributionREARRCEXP.ACTNUMST = accountB.BuildAccountRRC(12, policyInfo.LINE_OF_BUSINESS, item.Product_type, item.VEHICLE_TYPE,
                                                                                  rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID, item.Coverage_Desc);
                        rmDistributionREARRCEXP.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionREARRCEXP.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionREARRCEXP.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionREARRCEXP.DistRef = "Reservas para RRC a cargo reaseguro presente ejercicio";
                        rmDistributionREARRCEXP.CRDTAMNT = 0;
                        //sopDistributionREARRCEXP.DEBITAMT = coverageList.Sum(x => x.Coverage_Amount) * (percent / 100);
                        rmDistributionREARRCEXP.DEBITAMT = Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2);
                        rmDistributionREARRCEXP.DISTTYPE = 8;

                        if (rmDistributionREARRCEXP.DEBITAMT.GetValueOrDefault() > 0)
                        {
                            log.WriteLog(0, "RRC para reaseguro INGRESO: " + rmDistributionREARRCEXP.ACTNUMST);
                            rmDistribution.Add(rmDistributionREARRCEXP);
                        }

                        if (htrrcReaseguro.Contains(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()))
                        {
                            htrrcReaseguro[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] = (decimal)htrrcReaseguro[item.Product_type.Trim() + item.VEHICLE_TYPE.Trim()] + Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2); ;
                        }
                        else
                        {
                            htrrcReaseguro.Add(item.Product_type.Trim() + item.VEHICLE_TYPE.Trim(), Math.Round(Math.Abs(item.Coverage_Amount.GetValueOrDefault()) * (percent / 100), 2));
                        }
                    }


                    foreach (var PL in productlistCoverage)
                    {
                        decimal sumcom = coverageList.Where(y => y.Product_type.Trim() == PL.Product_type.Trim() && y.VEHICLE_TYPE.Trim() == PL.VEHICLE_TYPE.Trim()).Sum(z => Math.Round(Math.Abs(z.Coverage_Amount.GetValueOrDefault()), 2));
                        //Pasivo
                        var rmDistributionREARRCPAS = new RMTransactionDist();
                        rmDistributionREARRCPAS.ACTNUMST = accountB.BuildARAccount(9, policyInfo.LINE_OF_BUSINESS, PL.Product_type, PL.VEHICLE_TYPE,
                                                                                  rmCountry, policyInfo.CHANNEL, policyInfo.SUPERVISOR, policyInfo.SUPERVISOR_CODE, policyInfo.GpProductID);
                        rmDistributionREARRCPAS.DOCNUMBR = header.DOCNUMBR;
                        rmDistributionREARRCPAS.RMDTYPAL = header.RMDTYPAL;
                        rmDistributionREARRCPAS.CUSTNMBR = header.CUSTNMBR;
                        rmDistributionREARRCPAS.DistRef = "Reservas para RRC a cargo de reaseguro ";

                        //rmDistributionREARRCPAS.CRDTAMNT = Math.Round(Math.Abs(sumcom * (percent / 100)), 2);
                        rmDistributionREARRCPAS.CRDTAMNT = (decimal)htrrcReaseguro[PL.Product_type.Trim() + PL.VEHICLE_TYPE.Trim()];
                        rmDistributionREARRCPAS.DEBITAMT = 0;

                        rmDistributionREARRCPAS.DISTTYPE = 8;

                        if (rmDistributionREARRCPAS.CRDTAMNT.GetValueOrDefault() > 0)
                        {

                            log.WriteLog(0, "RRC para reaseguro PASIVO: " + rmDistributionREARRCPAS.ACTNUMST);
                            rmDistribution.Add(rmDistributionREARRCPAS);
                        }

                        //log.escribirLog(0, "RRC para reaseguro PASIVO: " + rmDistributionREARRCPAS.ACTNUMST);
                        //rmDistribution.Add(rmDistributionREARRCPAS);
                    }
                    htrrcReaseguro.Clear();
                }

                return rmDistribution.ToArray();
            }
            catch (Exception ex)
            {
                log.LogExeption("Ocurrió un error: ", 2, ex);
                throw;
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
            var invoicedal = new InvoiceDAL();
            return invoicedal.UpdateTransactionInvoce(doctype, module, policy_number, transaction_Sequence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="INITIAL_DATE"></param>
        /// <param name="DistributionList"></param>
        public void SaveRenewDistribution(DateTime INITIAL_DATE, TRANSACTION_INVOICE transaction, SOPClass.SOPDistribution[] DistributionList)
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.DeleteExistingRenewDistribution(transaction.GP_DOCNUMBR.Trim());
            invoicedal.SaveRenewDistribution(INITIAL_DATE, transaction, DistributionList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="INITIAL_DATE"></param>
        /// <param name="DistributionList"></param>
        public void SaveRenewDistribution(DateTime INITIAL_DATE, TRANSACTION_INVOICE transaction, RMClass.RMTransactionDist[] DistributionList)
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.DeleteExistingRenewDistributionNC(transaction.GP_DOCNUMBR.Trim());
            invoicedal.SaveRenewDistribution(INITIAL_DATE, transaction, DistributionList);
        }
        public void ChangeCustomerState(string customer, byte state)
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.ChangeCustomerState(customer, state);
        }

        public void DeleteReserve(string sopnumbe)
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.DeleteReserve(sopnumbe);
        }

        public bool ChannelNoComm(string channel)
        {
            var invoicedal = new InvoiceDAL();
            return invoicedal.ChannelNoComm(channel);
        }


        public void UpdateCommission()
        {
            var invoicedal = new InvoiceDAL();
            invoicedal.UpdateCommission();
        }


    }
}
