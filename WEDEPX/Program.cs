using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEDEPX
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WedepxService service = new WedepxService();
            await service.CallWedepxServiceAsync();

            if (args.Length >= 1)
            {
                string webServiceName = args[0];

                switch (webServiceName)
                {
                    case "CallWedepxService":
                        await service.CallWedepxServiceAsync();
                        break;


                }
            }
        }
    }
}
