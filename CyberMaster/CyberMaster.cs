using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPIRITLib;

namespace CyberMaster
{
    public sealed class CyberMaster : SpiritClass
    {
        public CyberMaster()
        {
            Console.WriteLine("Perform Setup");
            ComPortNo = COMPORTOPTIONS.COM3;
            LinkType = LINKTYPEOPTIONS.Radio;
            PBrick = PBRICKOPTIONS.Spirit;
            Console.WriteLine("Initialize Communication");
            InitComm();
            Console.WriteLine("Unlock Firmware");
            UnlockFirmware("Do you byte, when I knock?");
            Console.WriteLine("Unlock Brick");
            UnlockPBrick();

            if(!PBAliveOrNot())
                Console.WriteLine("ERROR");
            else
                Console.WriteLine("SUCCESS");
        }

        public void Forward()
        {
            SetFwd("01");
            On("01");
        }

        public void Stop()
        {
            Off("01");
        }
    }
}
