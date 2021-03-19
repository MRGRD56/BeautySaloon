using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BeautySaloon.Desktop.Extensions
{
    public static class MBox
    {
        public static void ShowError(string message, string title = "Ошибка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowOkCancel(string message, string title = "")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
    }
}
