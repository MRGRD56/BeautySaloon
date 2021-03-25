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

        /// <summary>
        /// Клиент, посещения которого отображаются.
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// Выбранное посещение клиента.
        /// </summary>
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

        /// <summary>
        /// Команда показа информации о выбранном посещении.
        /// </summary>
        public RelayCommand ShowVisitInfoCommand => new(o =>
        {
            var clientService = (ClientService) o;
            SelectedClientService = clientService;
        });

        /// <summary>
        /// Команда открытия файла <see cref="DocumentByService"/>.
        /// </summary>
        public RelayCommand OpenFileCommand => new(o =>
        {
            var document = (DocumentByService) o;
            System.Diagnostics.Process.Start(document.FullDocumentPath);
        });
    }
}
