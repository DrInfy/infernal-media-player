using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Imp.Libraries;

namespace Infernal_Media_Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            // send startup message to other instances
            ImpMessaging.InitializeMode(TheCodeKing.Net.Messaging.XDTransportMode.WindowsMessaging);
            ImpMessaging.SendMessage(ImpMessaging.START_EVENT);

            if (e.Args.Length > 0)
            {
                // handle arguments
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(150);
                System.Windows.Forms.Application.DoEvents();

                if (!string.IsNullOrEmpty(ImpMessaging.lastMsg) &&
                    ImpMessaging.lastMsg.Length >= 5 &&
                    ImpMessaging.lastMsg.Substring(ImpMessaging.lastMsg.Length - 5, 5) ==
                    ImpMessaging.DoNotDoAnythingMsg)
                {
                    SendPathsToActiveInstance(e);
                    System.Environment.Exit(0); // exit silently
                }
                else
                {
                    // Handle the command lines in this instance and start normally
                    ImpMessaging.List = new List<string>();
                    foreach (string commandLine in e.Args)
                    {
                        var c = commandLine;
                        ImpMessaging.List.Add(c);
                    }
                }
            }
        }

        /// <summary>
        /// Send the paths to current active instance
        /// </summary>
        private static void SendPathsToActiveInstance(StartupEventArgs e)
        {
            string files = ImpMessaging.CMD_LINES;
            foreach (string commandLine in e.Args)
            {
                var c = commandLine;
                files += ImpMessaging.NAME_SEPARATOR + c;
            }
            ImpMessaging.SendMessage(files);
        }


        public App()
        {
            Startup += Application_Startup;
        }
    }
}
