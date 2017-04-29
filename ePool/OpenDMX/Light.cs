using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePool
{
    public class Light
    {

        private Int32 DIMMER_CHAN = 1;
        private Int32 STROBE_CHAN = 3;
        private Int32 COLOR_CHAN = 4;
        private Int32 GOBO_CHAN = 5;
        private Int32 PAN_CHAN = 6;
        private Int32 PANF_CHAN = 7;
        private Int32 TILT_CHAN = 8;
        private Int32 TILTF_CHAN = 9;

        public Boolean LEDOn { get; set; }
        public LightCtrl.Color ColorVal { get; set; }
        public LightCtrl.Gobo GoboVal { get; set; }
        public LightCtrl.Strobe StrobeVal { get; set; }
        public Int32 Pan { get; set; }
        public Int32 PanFine { get; set; }
        public Int32 Tilt { get; set; }
        public Int32 TiltFine { get; set; }
        public Int32 Channel { get; set; }

        public Light(Int32 channel)
        {
            LEDOn = false;
            ColorVal = LightCtrl.Color.WHITE;
            GoboVal = LightCtrl.Gobo.OPEN;
            StrobeVal = LightCtrl.Strobe.OFF;
            Pan = 0;
            PanFine = 0;
            Tilt = 0;
            TiltFine = 0;
            Channel = channel;
        }

        public Tuple<Int32, byte> GetLEDValue()
        {
            byte val;
            if (LEDOn)
                val = 255;
            else
                val = 0;
            return Tuple.Create(Channel+DIMMER_CHAN, val);
        }

        public Tuple<Int32, byte> GetStrobeValue()
        {
            byte val;
            if (StrobeVal == LightCtrl.Strobe.OFF)
                val = 0;
            else if (StrobeVal == LightCtrl.Strobe.OPEN)
                val = 15;
            else
                val = 131;
            return Tuple.Create(Channel+STROBE_CHAN, val);
        }

        public Tuple<Int32, byte> GetGoboValue()
        {
            byte val = 0;
            switch (GoboVal)
            {
                case LightCtrl.Gobo.OPEN:
                    val = 0;
                    break;
                case LightCtrl.Gobo.SWIRL:
                    val = 15;
                    break;
                case LightCtrl.Gobo.CIRCLE:
                    val = 30;
                    break;
                case LightCtrl.Gobo.SQUARE:
                    val = 45;
                    break;
                case LightCtrl.Gobo.DOTS:
                    val = 60;
                    break;
                case LightCtrl.Gobo.LINE:
                    val = 75;
                    break;
                case LightCtrl.Gobo.OCT:
                    val = 90;
                    break;
                case LightCtrl.Gobo.SWIRLCROSS:
                    val = 105;
                    break;
                case LightCtrl.Gobo.STARS:
                    val = 120;
                    break;
                case LightCtrl.Gobo.ROTATE:
                    val = 200;
                    break;
            }
            return Tuple.Create(Channel+GOBO_CHAN, val);
        }

        public Tuple<Int32, byte> GetColorValue()
        {
            byte val = 0;
            switch (ColorVal)
            {
                case LightCtrl.Color.WHITE:
                    val = 0;
                    break;
                case LightCtrl.Color.RED:
                    val = 15;
                    break;
                case LightCtrl.Color.ORANGE:
                    val = 30;
                    break;
                case LightCtrl.Color.YELLOW:
                    val = 45;
                    break;
                case LightCtrl.Color.LGREEN:
                    val = 60;
                    break;
                case LightCtrl.Color.DBLUE:
                    val = 75;
                    break;
                case LightCtrl.Color.MAGENTA:
                    val = 90;
                    break;
                case LightCtrl.Color.LBLUE:
                    val = 105;
                    break;
                case LightCtrl.Color.PINK:
                    val = 120;
                    break;
                case LightCtrl.Color.ROTATE:
                    val = 193;
                    break;
            }
            return Tuple.Create(Channel+COLOR_CHAN, val);
        }

        public Tuple<Int32, byte> GetPanValue()
        {
            return Tuple.Create(Channel+PAN_CHAN, Convert.ToByte(Pan));
        }

        public Tuple<Int32, byte> GetPanfValue()
        {
            return Tuple.Create(Channel+PANF_CHAN, Convert.ToByte(PanFine));
        }

        public Tuple<Int32, byte> GetTiltValue()
        {
            return Tuple.Create(Channel+TILT_CHAN, Convert.ToByte(Tilt));
        }

        public Tuple<Int32, byte> GetTiltfValue()
        {
            return Tuple.Create(Channel+TILTF_CHAN, Convert.ToByte(TiltFine));
        }

    }
}
