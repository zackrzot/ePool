using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ePool
{
    public static class Logger
    {
        private static RichTextBox rtb = null;

        public static void Init(RichTextBox r)
        {
            rtb = r;
        }

        public static void Log(String text)
        {
            if (rtb != null)
            {
                rtb.AppendText(DateTime.Now.ToString("h:mm:ss tt : ") + text + "\n");
                rtb.ScrollToEnd();
            }
        }

    }
}
