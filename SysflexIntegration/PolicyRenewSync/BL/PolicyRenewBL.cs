using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolicyRenewSync.DAL;
using MODEL;
using GLClass;
using eConnectIntegration;
using eConnectIntegration.CLASS;
using Microsoft.Dynamics.GP.eConnect;

namespace PolicyRenewSync.BL
{
    public class PolicyRenewBL
    {
        public List<DateTime?> GetListDates(int Year, int Month)
        {
            var policyRenew = new PolicyRenewDAL();
            return policyRenew.GetListDates(Year, Month);
        }

        public List<string> GetListInvoices(DateTime Initial_Date)
        {
            var policyRenew = new PolicyRenewDAL();
            return policyRenew.GetListInvoices(Initial_Date);
        }

        public List<UNIT_DISTRIBUTION> GetUnitDistribution(string SOPNUMBE)
        {
            var policyRenew = new PolicyRenewDAL();
            return policyRenew.GetUnitDistribution(SOPNUMBE);
        }

        public List<POSTING_DITRIBUTION> GetPostingDistribution(string SOPNUMBE)
        {
            var policyRenew = new PolicyRenewDAL();
            return policyRenew.GetPostingDistribution(SOPNUMBE);
        }

        public void UpdateState(DateTime INITIAL_DATE)
        {
            var policyRenew = new PolicyRenewDAL();
            policyRenew.UpdateState(INITIAL_DATE);
        }

        public GLTrasactionHeader CreateHeader(DateTime Initial_Date, string bachType, string Description)
        {
            var JRNHeader = new GLTrasactionHeader();
            //var policyNumber = postingList.FirstOrDefault().CUSTNMBR.Trim();

            try
            {
                JRNHeader.Adjustment_Transaction = 0;
                JRNHeader.BACHNUMB = bachType.Trim() + Initial_Date.ToString("yyyyMMdd");
                JRNHeader.DATELMTS = 0;
                JRNHeader.Ledger_ID = 1;
                JRNHeader.NOTETEXT = "";
                JRNHeader.PRVDSLMT = 0;
                JRNHeader.RATEEXPR = 0;
                JRNHeader.RATEVARC = 0;
                JRNHeader.REFRENCE = Description.Trim() + " " + Initial_Date.ToString("yyyy-MM-dd");
                JRNHeader.RequesterTrx = 0;
                JRNHeader.SERIES = 2;
                JRNHeader.SQNCLINE = 0;
                JRNHeader.TRXDATE = Initial_Date;
                
                //JRNHeader.USERID = "Systemsflex";

                return JRNHeader;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public GLTransactionDetail CreateDetail (DateTime Initial_Date, UNIT_DISTRIBUTION Dist, string bachType)
        {
            var gltran = new GLTransactionDetail();

            try
            {
                gltran.BACHNUMB = bachType.Trim() + Initial_Date.ToString("yyyyMMdd");
                gltran.ACTNUMST = Dist.ACTNUMST;
                gltran.CRDTAMNT = Math.Round(Dist.CRDTAMNT,2);
                gltran.DEBITAMT = Math.Round(Dist.DEBITAMT,2);
                gltran.DATELMTS = 0;
                gltran.DOCDATE = Initial_Date;
                gltran.DSCRIPTN = "Reversa Cuenta Unitaria " + Initial_Date.ToString("yyyy-MM-dd");
                gltran.ORTRXTYP = 0;
                gltran.OrigSeqNum = 0;
                gltran.RATEEXPR = 0;
                gltran.PRVDSLMT = 0;
                gltran.RATEVARC = 0;
                gltran.RequesterTrx = 0;
                gltran.SQNCLINE = 0;
                gltran.ORMSTRID = Dist.CUSTNMBR;
                gltran.ORDOCNUM = Dist.SOPNUMBE;
                //gltran.ORMSTRNM = Dist.CUSTNAME;


                return gltran;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public GLTransactionDetail CreateDetail(DateTime Initial_Date, POSTING_DITRIBUTION Dist, string bachType)
        {
            var gltran = new GLTransactionDetail();

            try
            {
                gltran.BACHNUMB = bachType + Initial_Date.ToString("yyyyMMdd");
                gltran.ACTNUMST = Dist.ACTNUMST;
                gltran.CRDTAMNT = Dist.CRDTAMNT;
                gltran.DEBITAMT = Dist.DEBITAMT;
                gltran.DATELMTS = 0;
                gltran.DOCDATE = Initial_Date;
                gltran.DSCRIPTN = "Renovaciones " + Initial_Date.ToString("yyyy-MM-dd");
                gltran.ORTRXTYP = 0;
                gltran.OrigSeqNum = 0;
                gltran.RATEEXPR = 0;
                gltran.PRVDSLMT = 0;
                gltran.RATEVARC = 0;
                gltran.RequesterTrx = 0;
                gltran.SQNCLINE = 0;
                gltran.ORMSTRID = Dist.CUSTNMBR;
                gltran.ORDOCNUM = Dist.SOPNUMBE;
                //gltran.ORMSTRNM = Dist.CUSTNAME;


                return gltran;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
