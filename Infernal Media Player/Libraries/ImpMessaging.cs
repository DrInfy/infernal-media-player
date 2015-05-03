#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Interop;
using System.Windows.Threading;
using TheCodeKing.Net.Messaging;

#endregion

namespace Imp.Libraries
{
    internal static class ImpMessaging
    {
        #region Helpers

        /// <summary>
        /// Delegate used for invoke callback.
        /// </summary>
        /// <remarks></remarks>
        private delegate void UpdateFiles();

        #endregion

        #region Static Fields and Constants

        private const string CHANNEL_NAME = "IMP5";
        public const string START_EVENT = "¤started¤";
        public const string CMD_LINES = "¤cmdls¤";
        private const string MAKE_ACTIVE = "¤Make this Active¤";
        public const string NAME_SEPARATOR = "?";
        public const string DoNotDoAnythingMsg = "donot";
        private static MainWindow imp = null;
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

        private static string handleText;

        #endregion

        #region Properties

        public static bool LastActive { get; set; }

        #endregion

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
            handle = helper.Handle.ToInt32();

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
            if (e.DataGram.Message.IndexOf(START_EVENT, StringComparison.Ordinal) > 0 & LastActive)
            {
                SendMessage(DoNotDoAnythingMsg);
            }

            if (handle != 0) // this instance of imp has a working window
            {
                var handleIndex = e.DataGram.Message.IndexOf(handleText, StringComparison.Ordinal);

                if (handleIndex != 0) // if this is the handle, then the message came from this instance
                {
                    lastMsg = e.DataGram.Message;
                    if (e.DataGram.Message.IndexOf(START_EVENT, StringComparison.Ordinal) > 0 & LastActive)
                    {
                        Debug.WriteLine("donot sent");
                        SendMessage(DoNotDoAnythingMsg);
                    }
                    else if (e.DataGram.Message.IndexOf(CMD_LINES, StringComparison.Ordinal) > 0 & LastActive)
                    {
                        var i = 0;
                        var j = 0;
                        do
                        {
                            i = lastMsg.IndexOf(NAME_SEPARATOR);
                            j = lastMsg.IndexOf(NAME_SEPARATOR, i + 1, StringComparison.Ordinal);

                            if (i < 0)
                                break; // no more separators found

                            if (j < i)
                                j = lastMsg.Length;

                            List.Add(lastMsg.Substring(i + 1, j - i - 1));
                            if (j == lastMsg.Length)
                                break; // this was the last command, exit

                            lastMsg = lastMsg.Substring(j);
                        } while (true);

                        imp.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateFiles(imp.OpenFileLinesFromMessaging));
                    }
                    else if (e.DataGram.Message.IndexOf(MAKE_ACTIVE, StringComparison.Ordinal) > 0)
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