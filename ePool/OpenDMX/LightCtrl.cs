using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePool
{
    public static class LightCtrl
    {
        public static void SetAll(Light l, Color c, Gobo g, Strobe s, Int32 pan, Int32 panf, Int32 tilt, Int32 tiltf)
        {
            LightOnOff(l, true);
            SetColor(l, c);
            SetGobo(l, g);
            SetStrobe(l, s);
            SetPan(l, pan);
            SetPanFine(l, panf);
            SetTilt(l, tilt);
            SetTiltFine(l, tiltf);
        }

        public enum Light
        {
            LIGHT1,
            LIGHT2
        }

        public enum Color
        {
            WHITE,
            RED,
            ORANGE,
            YELLOW,
            LGREEN,
            DBLUE,
            MAGENTA,
            LBLUE,
            PINK,
            ROTATE
        }

        public enum Gobo
        {
            OPEN,
            SWIRL,
            CIRCLE,
            SQUARE,
            DOTS,
            LINE,
            OCT,
            SWIRLCROSS,
            STARS,
            ROTATE
        }

        public enum Strobe
        {
            OFF,
            OPEN,
            ON
        }

        public static void SetStrobe(Light l, Strobe s)
        {
            if (l == Light.LIGHT1)
                LightThread.Light1.StrobeVal = s;
            if (l == Light.LIGHT2)
                LightThread.Light2.StrobeVal = s;
        }

        public static void SetGobo(Light l, Gobo g)
        {
            if (l == Light.LIGHT1)
                LightThread.Light1.GoboVal = g;
            if (l == Light.LIGHT2)
                LightThread.Light2.GoboVal = g;
        }

        public static void SetColor(Light l, Color c)
        {
            if (l == Light.LIGHT1)
                LightThread.Light1.ColorVal = c;
            if (l == Light.LIGHT2)
                LightThread.Light2.ColorVal = c;
        }

        public static void SetPan(Light l, Int32 deg)
        {
            if (deg > 255 || deg < 0)
                return;

            if (l == Light.LIGHT1)
                LightThread.Light1.Pan = deg;
            if (l == Light.LIGHT2)
                LightThread.Light2.Pan = deg;
        }

        public static void SetPanFine(Light l, Int32 deg)
        {
            if (deg > 255 || deg < 0)
                return;

            if (l == Light.LIGHT1)
                LightThread.Light1.PanFine = deg;
            if (l == Light.LIGHT2)
                LightThread.Light2.PanFine = deg;
        }

        public static void SetTilt(Light l, Int32 deg)
        {
            if (deg > 255 || deg < 0)
                return;

            if (l == Light.LIGHT1)
                LightThread.Light1.Tilt = deg;
            if (l == Light.LIGHT2)
                LightThread.Light2.Tilt = deg;
        }

        public static void SetTiltFine(Light l, Int32 deg)
        {
            if (deg > 255 || deg < 0)
                return;

            if (l == Light.LIGHT1)
                LightThread.Light1.TiltFine = deg;
            if (l == Light.LIGHT2)
                LightThread.Light2.TiltFine = deg;
        }

        public static void LightOnOff(Light l, Boolean on) 
        {
            if (l == Light.LIGHT1)
                LightThread.Light1.LEDOn = on;
            if (l == Light.LIGHT2)
                LightThread.Light2.LEDOn = on;
        }

    }
}
