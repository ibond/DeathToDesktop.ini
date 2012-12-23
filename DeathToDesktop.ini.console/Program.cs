using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeathToDesktop.ini.console
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var watcher = new LibDeathToDesktop.ini.DeathToDesktopIniWatcher();
                Thread.Sleep(Timeout.Infinite);

                return 0;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed to create directory watcher: {0}", exp.Message);
                return 1;
            }
        }
    }
}
