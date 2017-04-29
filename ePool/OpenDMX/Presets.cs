using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePool
{
    public class Presets
    {

        public static void SetFR()
        {
            // light 1
            LightThread.Light1.Pan = 170;
            LightThread.Light1.PanFine = 0;
            LightThread.Light1.Tilt = 80;
            LightThread.Light1.TiltFine = 127;
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light1.LEDOn = true;
            // light 2
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light2.LEDOn = false;
        }

        public static void SetFL()
        {
            // light 1
            LightThread.Light1.Pan = 170;
            LightThread.Light1.PanFine = 0;
            LightThread.Light1.Tilt = 59;
            LightThread.Light1.TiltFine = 124;
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light1.LEDOn = true;
            // light 2
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OFF;
        }

        public static void SetCR()
        {
            // light 1
            LightThread.Light1.Pan = 152;
            LightThread.Light1.PanFine = 0;
            LightThread.Light1.Tilt = 75;
            LightThread.Light1.TiltFine = 124;
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light1.LEDOn = true;
            // light 2
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light2.LEDOn = false;
        }

        public static void SetCL()
        {
            // light 1
            LightThread.Light1.Pan = 159;
            LightThread.Light1.PanFine = 0;
            LightThread.Light1.Tilt = 56;
            LightThread.Light1.TiltFine = 119;
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light1.LEDOn = true;
            // light 2
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light2.LEDOn = false;
        }

        public static void SetBR()
        {
            // light 1
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light1.LEDOn = false;
            // light 2
            LightThread.Light2.Pan = 172;
            LightThread.Light2.PanFine = 0;
            LightThread.Light2.Tilt = 82;
            LightThread.Light2.TiltFine = 178;
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light2.LEDOn = true;
        }

        public static void SetBL()
        {
            // light 1
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light1.LEDOn = false;
            // light 2
            LightThread.Light2.Pan = 172;
            LightThread.Light2.PanFine = 0;
            LightThread.Light2.Tilt = 61;
            LightThread.Light2.TiltFine = 178;
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light2.LEDOn = true;
        }

        public static void SetBreak()
        {
            // light 1
            LightThread.Light1.Pan = 162;
            LightThread.Light1.PanFine = 173;
            LightThread.Light1.Tilt = 69;
            LightThread.Light1.TiltFine = 176;
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light1.LEDOn = true;
            // light 2
            LightThread.Light2.Pan = 178;
            LightThread.Light2.PanFine = 0;
            LightThread.Light2.Tilt = 70;
            LightThread.Light2.TiltFine = 177;
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OPEN;
            LightThread.Light2.LEDOn = true;
        }

        public static void SetOff()
        {
            // light 1
            LightThread.Light1.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light1.LEDOn = false;
            // light 2
            LightThread.Light2.StrobeVal = LightCtrl.Strobe.OFF;
            LightThread.Light2.LEDOn = false;
        }

    }
}
