using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySaloon.Model.DbModels;

namespace BeautySaloon.Desktop.ViewModels.WindowsViewModels
{
    public class ClientVisitsWindowViewModel : BaseViewModel
    {
        private ClientService _selectedClientService;

        public Client Client { get; }

        public ClientService SelectedClientService
        {
            get => _selectedClientService;
            set
            {
                _selectedClientService = value;
                OnPropertyChanged();
            }
        }

        public ClientVisitsWindowViewModel(Client client)
        {
            Client = client;
            SelectedClientService = client.ClientServices.FirstOrDefault();
        }

        public RelayCommand ShowVisitInfoCommand => new(o =>
        {
            var clientService = (ClientService) o;
            SelectedClientService = clientService;
        });

        public RelayCommand OpenFileCommand => new(o =>
        {
            var document = (DocumentByService) o;
            System.Diagnostics.Process.Start(document.FullDocumentPath);
        });
    }
}
