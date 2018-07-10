using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLClass;
using eConnectIntegration;
using eConnectIntegration.CLASS;
using Microsoft.Dynamics.GP.eConnect;

namespace GPServicesREF.GL
{
    public class Journals
    {
        public Response CreateJournal(GLTrasactionHeader header, GLTransactionDetail[] detail, string company)
        {
            Response response;
            var gpServices = new GLTransactionClient();
            response = gpServices.CreateGLTransaction(header, detail, company);
            gpServices.Close();
            return response;
        }
    }
}
