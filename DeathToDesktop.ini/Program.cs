using Microsoft.Win32;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace DeathToDesktop.ini
{
    class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            // Check if we should install this in the startup menu.
            if (args.Length > 0)
            {
                if (args[0] == "--install")
                {
                    return Install(args);
                }
            }

            try
            {
                // Start the watcher.
                var watcher = new DeathToDesktopIniWatcher();

                // The watcher does its thing from here, just sleep to prevent exit.
                Thread.Sleep(Timeout.Infinite);

                return 0;
            }
            catch (Exception exp)
            {
                MessageBox.Show(string.Format("Failed to create directory watcher: {0}", exp.Message), "DeathToDesktop.ini Failed", MessageBoxButtons.OK);
                return 1;
            }
        }

        /// <summary>
        /// This will install this program in the current user's startup program group.
        /// </summary>
        static int Install(string[] args)
        {
            try
            {
                // Get the full path to this executable.
                string executablePath = Assembly.GetEntryAssembly().Location;

                // Add the registry key.
                RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                startupKey.SetValue("DeathToDesktop.ini", "\"" + executablePath + "\"");

                MessageBox.Show(string.Format("'{0}' installed successfully.", executablePath), "Success", MessageBoxButtons.OK);

                return 0;
            }
            catch (Exception exp)
            {
                MessageBox.Show(string.Format("Failed to install: {0}", exp.Message), "Install Failed", MessageBoxButtons.OK);
                return 1;
            }
        }
    }
}
