using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberMaster.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var cyb = new CyberMaster();

            Console.ReadKey();

            cyb.Forward();

            Console.ReadKey();

            cyb.Stop();
        }
    }
}
