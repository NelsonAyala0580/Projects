using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;


[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Log4NetMR
{
    public class classClsRegistrarLog
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void WriteLog(int tipo, Object mensaje)
        {
            log4net.GlobalContext.Properties["mensaje"] = "Exepcion Mensaje";
            try
            {
                if (tipo == 0) { log.Debug(mensaje); }
                if (tipo == 1) { log.Warn(mensaje); }
                if (tipo == 2) { log.Error(mensaje);
                }

                if (tipo == 3) { log.Fatal(mensaje); }
            }
            catch (Exception ex) {
                throw ex;
            }
       
        }

        public void LogExeption(string mensaje, int tipo, Exception ex) {
            log4net.GlobalContext.Properties["mensaje"] = "Exepcion Mensaje";
            try
            {
                if (tipo == 0) { log.Debug(mensaje, ex ); }
            if (tipo == 1) { log.Warn(mensaje, ex); }
            if (tipo == 2) { log.Error(mensaje, ex); }
            if (tipo == 3) { log.Fatal(mensaje, ex); }
            }
            catch (Exception ex1)
            {
                throw ex1;
            }
        }
    }
}
