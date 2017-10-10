using Injectoclean.Tools.BLE;
using System;
using Windows.Foundation.Diagnostics;
using static Injectoclean.MainPage;

namespace Injectoclean.Tools.Developers
{
    public class Log: ILog
    {
        
        static LoggingChannel lc = new LoggingChannel("InjectoClean", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));
        public void LogMessageError(String message)
        {
            lc.LogMessage("Error: " + message);
            Current.NotifyUser(message, NotifyType.ErrorMessage);
        }
        public void LogMessageNotification(String message)
        {
            lc.LogMessage("Notification: " + message);
            Current.NotifyUser(message, NotifyType.StatusMessage);
        }
        public void LogMessage(String message)
        {
            lc.LogMessage("Log: "+message);
        }
    }
}
