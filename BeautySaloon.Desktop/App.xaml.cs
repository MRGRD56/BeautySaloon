using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BeautySaloon.Desktop.Views.Windows;

namespace BeautySaloon.Desktop
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static MainWindow MainWindow => (MainWindow)Current.MainWindow;
    }
}
