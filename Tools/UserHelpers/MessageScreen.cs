using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Injectoclean.Tools.UserHelpers
{
    class MessageScreen
    {
        private ContentDialog dialog;
        private ProgressRing ring;
        public  MessageScreen(String waitmessage)
        { 
            dialog = new ContentDialog
            {
                Title = waitmessage,
            };
            ProgressRing ring = new ProgressRing();
            ring.IsActive = true;
            dialog.Content = ring;
        }
        private MessageScreen()
        {
            dialog = new ContentDialog();
        }
        public async void Show()
        {
            await dialog.ShowAsync();
        }
        public void Close()
        {
            dialog.Hide();
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
        public void SetwithButton(String title, String content, String CloseButton)
        {
            dialog.Title = title;
            dialog.Content = content;
            dialog.CloseButtonText = CloseButton;
        }
        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }
    }
}
