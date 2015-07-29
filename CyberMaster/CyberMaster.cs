using System;
using System.CodeDom;
using System.Runtime.InteropServices.WindowsRuntime;
using SPIRITLib;

namespace CyberMaster
{
    public sealed class CyberMaster : SpiritClass
    {
        public enum MotorDirection
        {
            Forward,
            Backward
        }

        public CyberMaster()
        {
            
        }

        public bool Connect()
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

            if (!PBAliveOrNot())
                return false;

            return true;
        }

        public void MotorOn(MotorDirection direction, int motorNumber)
        {
            if (direction == MotorDirection.Forward)
                SetFwd(motorNumber.ToString());
            else
                SetRwd(motorNumber.ToString());

            On(motorNumber.ToString());
        }

        public void MotorOff(int motorNumber)
        {
            Off(motorNumber.ToString());
        }
    }
}
