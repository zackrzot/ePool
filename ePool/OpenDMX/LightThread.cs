using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ePool
{
    public static class LightThread
    {
        private static Light light1 = new Light(0);
        private static Light light2 = new Light(10);
        private static Boolean stop = false;

        private static object lockLight1 = new object();
        private static object lockLight2 = new object();
        private static object lockStop = new object();

        public static Light Light1
        {
            get
            {
                lock (lockLight1)
                {
                    return light1;
                }
            }
            set
            {
                lock (lockLight1)
                {
                    light1 = value;
                }
            }
        }
        public static Light Light2
        {
            get
            {
                lock (lockLight2)
                {
                    return light2;
                }
            }
            set
            {
                lock (lockLight2)
                {
                    light2 = value;
                }
            }
        }
        private static Boolean Stop
        {
            get
            {
                lock (lockStop)
                {
                    return stop;
                }
            }
            set
            {
                lock (lockLight2)
                {
                    stop = value;
                }
            }
        }

        public static void StartLights()
        {
            Thread t = new Thread(new ThreadStart(_lightThread));
            t.Start();
        }

        public static void StopLights()
        {
            Stop = true;
        }
   
        private static void _lightThread()
        {
            Console.WriteLine("_lightThread started.");
            Tuple<Int32, byte> lval;
            while (!Stop)
            {
                // light 1 led
                lval = Light1.GetLEDValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                OpenDMX.setDmxValue(lval.Item1 + 1, lval.Item2);
                // light 1 strobe
                lval = Light1.GetStrobeValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 gobo
                lval = Light1.GetGoboValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 color
                lval = Light1.GetColorValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 pan
                lval = Light1.GetPanValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 pan fine
                lval = Light1.GetPanfValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 tilt
                lval = Light1.GetTiltValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 1 tile fine
                lval = Light1.GetTiltfValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);

                // light 2 led
                lval = Light2.GetLEDValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                OpenDMX.setDmxValue(lval.Item1 + 1, lval.Item2);
                // light 2 strobe
                lval = Light2.GetStrobeValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 gobo
                lval = Light2.GetGoboValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 color
                lval = Light2.GetColorValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 pan
                lval = Light2.GetPanValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 pan fine
                lval = Light2.GetPanfValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 tilt
                lval = Light2.GetTiltValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);
                // light 2 tile fine
                lval = Light2.GetTiltfValue();
                OpenDMX.setDmxValue(lval.Item1, lval.Item2);

                Thread.Sleep(100);
                OpenDMX.writeData();
            }
            Console.WriteLine("_lightThread stopped.");
        }

    }
}
