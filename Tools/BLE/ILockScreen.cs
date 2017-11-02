using System;
using System.Threading.Tasks;

namespace Injectoclean.Tools.BLE
{
    public interface ILockScreen
    {
        void Show(String title);
        void Close();
        void setTitle(String title);
        Task set(String title, String content, int timeout);
        void SetwithButton(String title, String content, String CloseButtonName);
    }
}
