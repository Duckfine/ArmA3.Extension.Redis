using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis_CS.util
{

    public static class ConsoleLog
    {

        #region constants, vars, etc (header)

        #region timeserver

        const string ntpServer = "time.windows.com";


        #endregion


        #region consoleedit
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #endregion


        #endregion




        internal static bool InitConsole()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }


        public static void Log(string text, int loglevel)
        {
            DateTime dt = DateTime.Now;

            String ret = dt.ToString("T", DateTimeFormatInfo.InvariantInfo);

            Console.WriteLine("[" + ret + "]" + " " + text);

            //todo output to textfile






        }



    }
}
