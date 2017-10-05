using System;
namespace Injectoclean.Tools.BLE
{
    public interface ILog
    {
        void LogMessageError(String message);
        void LogMessageNotification(String message);
        void LogMessage(String message);
    }
}
