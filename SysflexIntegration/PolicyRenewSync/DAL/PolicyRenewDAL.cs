using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MODEL;

namespace PolicyRenewSync.DAL
{
    public class PolicyRenewDAL
    {
        public List<DateTime?> GetListDates(int Year, int Month)
        {
            var contexto = new ATLANEntities();

            try
            {
                return contexto.usp_ST_SYSFLEX_GET_RENEW_DATES(Year, Month).ToList();
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

        public List<string> GetListInvoices(DateTime Initial_Date)
        {
            var contexto = new ATLANEntities();
            try
            {
                return contexto.usp_ST_SYSFLEX_GET_RENEW_INVOICES(Initial_Date).ToList();
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

        public List<UNIT_DISTRIBUTION> GetUnitDistribution(string SOPNUMBE)
        {
            var contexto = new ATLANEntities();
            try
            {
                return contexto.usp_SYSFLEX_GET_UNIT_DISTRIBUTION(SOPNUMBE).ToList();
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

        public List<POSTING_DITRIBUTION> GetPostingDistribution(string SOPNUMBE)
        {
            var contexto = new ATLANEntities();
            try
            {
                return contexto.usp_SYSFLEX_GET_POSTING_DITRIBUTION(SOPNUMBE).ToList();
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

        public void UpdateState(DateTime INITIAL_DATE)
        {
            var contexto = new ATLANEntities();
            try
            {
                contexto.usp_SYSFLEX_RENEW_UPDATE_STATE(INITIAL_DATE);
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
    }
}
