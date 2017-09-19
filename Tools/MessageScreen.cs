using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SDKTemplate.Tools
{
    class MessageScreen
    {
        ContentDialog dialog;
        public  MessageScreen(String title)
        { 
            dialog = new ContentDialog
            {
                Title = title,
            };
            ProgressRing ring = new ProgressRing();
            ring.IsActive = true;
            dialog.Content = ring;
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
    }
}
