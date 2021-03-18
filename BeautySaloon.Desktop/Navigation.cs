using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeautySaloon.Desktop
{
    public static class Navigation
    {
        public static Frame Frame => App.MainWindow.Frame;

        public static void Navigate(Page page) => Frame.Navigate(page);

        public static void GoBack()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
