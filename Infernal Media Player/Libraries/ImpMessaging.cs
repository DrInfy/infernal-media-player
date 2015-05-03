using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Interop;
using System.Windows.Threading;
using TheCodeKing.Net.Messaging;


namespace Imp.Libraries
{
    static class ImpMessaging
    {
        private const string CHANNEL_NAME = "IMP5";
        public const string START_EVENT = "¤started¤";
        public const string CMD_LINES = "¤cmdls¤";

        private const string MAKE_ACTIVE = "¤Make this Active¤";

        public const string NAME_SEPARATOR = "?";
        public const string DoNotDoAnythingMsg = "donot";
        public static bool LastActive { get; set; }

        static MainWindow imp = null;
        public static int handle = 0;
        public static string lastMsg = "";
        public static List<string> List = new List<string>();

        /// <summary>
        /// The instance used to listen to broadcast messages.
        /// </summary>
        private static IXDListener listener;

        /// <summary>
        /// The instance used to broadcast messages on a particular channel.
        /// </summary>
        private static IXDBroadcast broadcast;

        /// <summary>
        /// Delegate used for invoke callback.
        /// </summary>
        /// <remarks></remarks>
        private delegate void UpdateFiles();


        private static string handleText;

        /// <summary>
        /// Initialize the broadcast and listener mode.
        /// </summary>
        /// <param name="mode">The new mode.</param>
        public static void InitializeMode(XDTransportMode mode)
        {
            if (listener != null)
            {
                // ensure we dispose any previous listeners, dispose should aways be
                // called on IDisposable objects when we are done with it to avoid leaks
                listener.Dispose();
            }
            listener = XDListener.CreateListener(mode);

            // attach the message handler
            listener.MessageReceived += OnMessageReceived;

            listener.RegisterChannel(CHANNEL_NAME);

            // create an instance of IXDBroadcast using the given mode, 
            // note IXDBroadcast does not implement IDisposable
            broadcast = XDBroadcast.CreateBroadcast(mode, false);
        }


        public static void SetImp(MainWindow imppi)
        {
            imp = imppi;
            
            //IMPController.InitializeMode(TheCodeKing.Net.Messaging.XDTransportMode.IOStream)
            var helper = new WindowInteropHelper(imp);
            ImpMessaging.handle = helper.Handle.ToInt32();

            handleText = handle.ToString(CultureInfo.InvariantCulture);
        }


        public static void DeclareActive()
        {
            LastActive = true;
            Debug.WriteLine("LastActive True");
            SendMessage(MAKE_ACTIVE);
        }


        /// <summary>
        /// The delegate which processes all cross AppDomain messages and writes them to screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMessageReceived(object sender, XDMessageEventArgs e)
        {
            
            if (e.DataGram.Message.IndexOf(START_EVENT, System.StringComparison.Ordinal) > 0 & LastActive)
            {
                SendMessage(DoNotDoAnythingMsg);
            }

            if (handle != 0) // this instance of imp has a working window
            {
                int handleIndex = e.DataGram.Message.IndexOf(handleText, System.StringComparison.Ordinal);

                if (handleIndex != 0) // if this is the handle, then the message came from this instance
                {
                    lastMsg = e.DataGram.Message;
                    if (e.DataGram.Message.IndexOf(START_EVENT, System.StringComparison.Ordinal) > 0 & LastActive)
                    {
                        Debug.WriteLine("donot sent");
                        SendMessage(DoNotDoAnythingMsg);
                    }
                    else if (e.DataGram.Message.IndexOf(CMD_LINES, System.StringComparison.Ordinal) > 0 & LastActive)
                    {
                        int i = 0;
                        int j = 0;
                        do
                        {
                            i = lastMsg.IndexOf(NAME_SEPARATOR);
                            j = lastMsg.IndexOf(NAME_SEPARATOR, i + 1, System.StringComparison.Ordinal);

                            if (i < 0)
                                break; // no more separators found

                            if (j < i)
                                j = lastMsg.Length;

                            List.Add(lastMsg.Substring(i + 1, j - i - 1));
                            if (j == lastMsg.Length)
                                break;  // this was the last command, exit

                            lastMsg = lastMsg.Substring(j);
                        } while (true);

                        imp.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateFiles(imp.OpenFileLinesFromMessaging));
                    }
                    else if (e.DataGram.Message.IndexOf(MAKE_ACTIVE, System.StringComparison.Ordinal) > 0)
                    {
                        Debug.WriteLine("LastActive False");
                        LastActive = false;
                    }
                }
            }
            else // this instance of imp doesn't yet have a working window
            {
                if (e.DataGram.Message.Substring(0, 2) != "0 ")
                {
                    lastMsg = DoNotDoAnythingMsg;
                }
            }
        }

        /// <summary>
        /// Helper method for sending message.
        /// </summary>
        public static void SendMessage(string text)
        {
            broadcast.SendToChannel(CHANNEL_NAME, handle.ToString() + " " + text);
        }
    }
}