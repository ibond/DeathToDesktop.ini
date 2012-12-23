using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeathToDesktop.ini
{
    /// <summary>
    /// This class takes care of watching for the desktop.ini file.
    /// </summary>
    public class DeathToDesktopIniWatcher
    {
        /// <summary>
        /// How long do we wait after failing to delete the file before retrying.
        /// </summary>
        const int DeleteRetryDelayMs = 2000;

        /// <summary>
        /// How many times do we try to delete the file before giving up.
        /// </summary>
        const int DeleteRetryCount = 10;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeathToDesktopIniWatcher()
        {
            // Get the desktop directories.
            m_desktopDirectories = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
            };
            
            // Create the file system watchers.
            m_watchers = m_desktopDirectories
                .Select(directory =>
                {
                    Console.WriteLine("Watching for creation of '{0}'.", Path.Combine(directory, "desktop.ini"));

                    var watcher = new FileSystemWatcher(directory, "desktop.ini");
                    watcher.Created += new FileSystemEventHandler(OnCreated);
                    watcher.EnableRaisingEvents = true;

                    return watcher;
                })
                .ToArray();
        }

        /// <summary>
        /// This is called when a desktop.ini file is created.
        /// </summary>
        private void OnCreated(object in_source, FileSystemEventArgs in_args)
        {
            try
            {
                // Do nothing unless the file was created.
                if (in_args.ChangeType != WatcherChangeTypes.Created)
                {
                    Console.WriteLine("Unexpected ChangeType '{0}', ignoring.", in_args.ChangeType.ToString());
                    return;
                }

                // Verify that we have a desktop.ini file.
                string filename = Path.GetFileName(in_args.FullPath);
                if (filename != "desktop.ini")
                {
                    Console.WriteLine("Unexpected file '{0}', ignoring.", in_args.FullPath);
                    return;
                }

                // Verify that the directory is a Desktop directory.
                string directoryName = Path.GetDirectoryName(in_args.FullPath);
                if (!m_desktopDirectories.Contains(directoryName))
                {
                    Console.WriteLine("Unexpected directory '{0}', ignoring.", directoryName);
                    return;
                }

                // Delete the file.
                Console.WriteLine("Deleting '{0}'.", in_args.FullPath);
                try
                {
                    // Sleep for a little bit to help prevent access issues with the writer.
                    Thread.Sleep(10);

                    DeleteFile(in_args.FullPath);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Failed to delete '{0}': {1}", in_args.FullPath, exp.Message);
                    Console.WriteLine("Retrying {0} more times in {1} seconds.", DeleteRetryCount, (double)DeleteRetryDelayMs / 1000);

                    // Run an asynchronous task to delete the file.
                    Task task = Task.Factory.StartNew(() =>
                    {
                        for (int retryCount = 0; retryCount < DeleteRetryCount; ++retryCount)
                        {
                            Thread.Sleep(DeleteRetryDelayMs);
                            try
                            {
                                DeleteFile(in_args.FullPath);

                                // If there wasn't an exception then the file was deleted.
                                return;
                            }
                            catch (Exception deleteExp)
                            {
                                Console.WriteLine("Failed to delete '{0}': {1}", in_args.FullPath, deleteExp.Message);
                                Console.WriteLine("Retrying {0} more times in {1} seconds.", DeleteRetryCount - retryCount, (double)DeleteRetryDelayMs / 1000);
                            }
                        }
                    });
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Unexpected exception: {0}", exp.Message);
            }
        }

        /// <summary>
        /// Deletes the named file.
        /// </summary>
        /// <param name="in_filename">The file to be deleted.</param>
        private void DeleteFile(string in_filename)
        {
            File.Delete(in_filename);
        }

        /// <summary>
        /// The path to the desktop directories.
        /// </summary>
        private string[] m_desktopDirectories;
        
        /// <summary>
        /// Our list of file system watchers.
        /// </summary>
        private FileSystemWatcher[] m_watchers;
    }
}
