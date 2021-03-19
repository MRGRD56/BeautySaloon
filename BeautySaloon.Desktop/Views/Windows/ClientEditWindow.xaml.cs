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
    /// Логика взаимодействия для ClientEditWindow.xaml
    /// </summary>
    public partial class ClientEditWindow : Window
    {
        /// <summary>
        /// Создаёт окно для добавления клиента.
        /// </summary>
        public ClientEditWindow()
        {
            InitializeComponent();
            DataContext = new ClientEditWindowViewModel();
            Title = "Добавление клиента";
        }

        /// <summary>
        /// Создаёт окно для редактирования клиента.
        /// </summary>
        /// <param name="client">Редактируемый клиент.</param>
        public ClientEditWindow(Client client)
        {
            InitializeComponent();
            DataContext = new ClientEditWindowViewModel(client);
            Title = "Редактирование клиента";
        }
    }
}
