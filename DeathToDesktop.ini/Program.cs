using System;
using System.Threading;

namespace DeathToDesktop.ini
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var watcher = new DeathToDesktopIniWatcher();
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
