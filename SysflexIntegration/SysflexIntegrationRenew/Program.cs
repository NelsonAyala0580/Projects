using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolicyRenewSync;


namespace SysflexIntegrationRenew
{
    class Program
    {
        static void Main(string[] args)
        {
            var PoliyRenew = new PolicyRenewTask();

            PoliyRenew.Process();
        }
    }
}
