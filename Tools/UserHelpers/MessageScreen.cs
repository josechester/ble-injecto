using Injectoclean.Tools.BLE;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Injectoclean.Tools.UserHelpers
{
    public class MessageScreen: ILockScreen
    {
        private ContentDialog dialog;
        private ProgressRing ring;
        private bool IsOpen = false;
        public MessageScreen()
        {
            dialog = new ContentDialog();
            ring = new ProgressRing();
            ring.IsActive = true;
            dialog.Content = ring;
        }

        public async void Show(String title)
        {
            if (IsOpen)
                this.Close();
            IsOpen = true;
            dialog.Hide();
            dialog.Title = title;
            await dialog.ShowAsync();
        }
        public void Close()
        {
            dialog.Hide();
            IsOpen = false;
        }
        public void setTitle(String title)
        {
            dialog.Title = title;
            
        }
        public async void set(String title,String content,int timeout)
        {
            dialog.Title = title;
            dialog.Content =content;
            await PutTaskDelay(timeout);
            this.Close();
        }
        public void SetwithButton(String title, String content, String CloseButtonName)
        {
            dialog.Title = title;
            dialog.Content = content;
            dialog.CloseButtonText = CloseButtonName;
        }
        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }
    }
}
