using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySaloon.Desktop.Views.Windows;
using BeautySaloon.Model.DbModels;
using Microsoft.Win32;

namespace BeautySaloon.Desktop.ViewModels.WindowsViewModels
{
    public class ClientEditWindowViewModel : BaseViewModel
    {
        public bool IsEditMode { get; set; }

        public Client EditingClient { get; set; }

        public bool Result { get; set; }

        public RelayCommand LoadPhotoCommand => new(o =>
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp";
            openFileDialog.ShowDialog();
        });

        public RelayCommand CancelCommand => new(o =>
        {
            var window = (ClientEditWindow)o;

            window.Close();
        });


        public ClientEditWindowViewModel()
        {
            EditingClient = new Client();
            IsEditMode = false;
        }

        public ClientEditWindowViewModel(Client client)
        {
            EditingClient = (Client) client.Clone();
            IsEditMode = true;
        }
    }
}
