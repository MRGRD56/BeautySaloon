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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BeautySaloon.Desktop.Views.Pages;

namespace BeautySaloon.Desktop.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Frame.Navigated += Frame_Navigated;
            Navigation.Navigate(new MainMenuPage());

            new ClientEditWindow().Show();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                Title = $"Салон красоты - {page.Title}";
                GoBackButton.Visibility = (Frame.CanGoBack && !(page is MainMenuPage))
                    ? Visibility.Visible 
                    : Visibility.Collapsed;

                MainMenuButton.Visibility = page is MainMenuPage ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate(new MainMenuPage());
        }
    }
}
