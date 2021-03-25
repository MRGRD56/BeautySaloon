using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BeautySaloon.Desktop.ViewModels.WindowsViewModels;
using BeautySaloon.Model.DbModels;

namespace BeautySaloon.Desktop.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClientVisitsWindow.xaml
    /// </summary>
    public partial class ClientVisitsWindow : Window
    {
        /// <summary>
        /// Клиент, посещения которого отображаются.
        /// </summary>
        public Client Client { get; }

        public ClientVisitsWindow(Client client)
        {
            InitializeComponent();
            Client = client;
            Title = $"{client.FullName} - посещения";
            DataContext = new ClientVisitsWindowViewModel(client);
        }
    }
}
