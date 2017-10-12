using Injectoclean.Tools.BLE;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Injectoclean.Tools.UserHelpers
{
    public class MessageScreen : ILockScreen
    {
        private ContentDialog dialog;
        private ProgressRing ring;
        private bool IsOpen = false;
        public void Close()
        {
            dialog.Hide();
            IsOpen = false;
        }

        public void setTitle(string title)
        {
            dialog.Title = title;
            dialog.Content = ring;

        }

        public void SetwithButton(string title, string content, string CloseButtonName)
        {
            dialog.Title = title;
            dialog.Content = content;
            dialog.CloseButtonText = CloseButtonName;

        }

        public /*async*/ void Show(string title)
        {
            if (IsOpen)
                this.Close();
            IsOpen = true;
            dialog.Hide();
            dialog.Title = title;
            /*await*/ dialog.ShowAsync();
        }
       
        public MessageScreen()
        {
            dialog = new ContentDialog();
            ring = new ProgressRing();
            ring.IsActive = true;
            dialog.Content = ring;
        }

        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }

        public async void set(string title, string content, int timeout)
        {
            dialog.Title = title;
            dialog.Content = content;
            await PutTaskDelay(timeout);
            this.Close();
        }
    }
}
