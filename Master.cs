using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NShapeCreator.Message;
using NShapeCreator.UI;

namespace NShapeCreator
{

    static class Master
    {

        #region Fields

        private static List<Thread> threads;
        private static ManualResetEvent[] events;
        private static List<Action<Object>> threadStartDelegates;
        private static List<MessageForm> baseForms;
        private static Object handleMessageLocker;

        #endregion Fields

        #region Methods

        private static void SendMessage(Object sender, MessageEventArgs e)
        {
            try
            {
                foreach (MessageForm f in baseForms)
                {
                    if (f.Name != ((MessageForm)sender).Name)
                    {
                        f.HandleMessage(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void LogError(string error)
        {
            Process p = Process.GetCurrentProcess();
            string file = p.ProcessName + "_" + p.StartTime.ToString("yyyyMMddHHmmss");
            using (FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(DateTime.Now);
                    sw.Write(error);
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
        }

        #endregion Methods

        #region Threads

        private static void StartThreads()
        {
            try
            {
                events = new ManualResetEvent[threads.Count];
                for (int i = 0; i < threads.Count; i++)
                {
                    events[i] = new ManualResetEvent(false);
                    threads[i].Start(events[i]);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void SetThreadStartDelegates()
        {
            try
            {
                threadStartDelegates = new List<Action<Object>>();
                threadStartDelegates.Add(StartNShapeCreatorDisplay);
                threadStartDelegates.Add(StartNShapeCreatorControl);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void SetThreads()
        {
            try
            {
                threads = new List<Thread>();
                foreach (Action<Object> a in threadStartDelegates)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(a));
                    t.IsBackground = true;
                    t.SetApartmentState(ApartmentState.STA);
                    threads.Add(t);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void StartNShapeCreatorDisplay(Object manualResetEvent)
        {
            try
            {
                MessageForm f = new NDisplay();
                StartBaseMessageForm(manualResetEvent, f);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void StartNShapeCreatorControl(Object manualResetEvent)
        {
            try
            {
                MessageForm f = new NControl();
                StartBaseMessageForm(manualResetEvent, f);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private static void StartBaseMessageForm(Object manualResetEvent, MessageForm messageForm)
        {
            bool failSafe = false;
            while (!failSafe)
            {
                try
                {
                    baseForms.Add(messageForm);
                    messageForm.Messages += new EventHandler<MessageEventArgs>(HandleMessage);
                    messageForm.FormClosed += new FormClosedEventHandler(OnFormClosed);
                    messageForm.ShowDialog();
                    failSafe = ((ManualResetEvent)manualResetEvent).Set();
                }
                catch (Exception ex)
                {
                    if (baseForms.Contains(messageForm))
                    {
                        baseForms.Remove(messageForm);
                    }
                    LogError(ex.ToString());
                }
            }
        } 

        #endregion Threads

        #region Event Handlers

        private static void OnFormClosed(Object sender, FormClosedEventArgs e)
        {
            MessageEventArgs eArgs = new MessageEventArgs();
            eArgs.Messages[MessageID.FormClosed] = null;
            SendMessage(sender, eArgs);
        }

        private static void HandleMessage(Object sender, MessageEventArgs e)
        {
            lock (handleMessageLocker)
            {
                if (e.Messages.ContainsKey(MessageID.LogError))
                {
                    LogError((string)e.Messages[MessageID.LogError]);
                }
                SendMessage(sender, e);
            }
        }

        #endregion Event Handlers

        private static void Main()
        {
            handleMessageLocker = new Object();
            baseForms = new List<MessageForm>();
            SetThreadStartDelegates();
            SetThreads();
            StartThreads();
            ManualResetEvent.WaitAll(events);
        }

    }

}
