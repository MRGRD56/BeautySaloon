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
        /// <summary>
        /// Выводит <see cref="MessageBox"/> с сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="title">Заголовок окна.</param>
        public static void ShowError(string message, string title = "Ошибка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Выводит <see cref="MessageBox"/> с кнопками <see cref="MessageBoxButton.OKCancel"/>.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="title">Заголовок окна.</param>
        /// <returns>Результат <see cref="MessageBox"/>.</returns>
        public static MessageBoxResult ShowOkCancel(string message, string title = "")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }
    }
}
