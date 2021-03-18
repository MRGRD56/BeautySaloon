using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using BeautySaloon.Context;
using BeautySaloon.Library;
using BeautySaloon.Model.DbModels;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.ViewModels.PagesViewModels
{
    public class ClientsPageViewModel : BaseViewModel
    {
        private readonly ObservableCollection<Client> _clients = new ObservableCollection<Client>();

        public ICollectionView ClientsView { get; set; }

        private string _fullNameSearchQuery;
        private string _emailSearchQuery;
        private string _phoneSearchQuery;

        public string FullNameSearchQuery
        {
            get => _fullNameSearchQuery;
            set
            {
                _fullNameSearchQuery = value;
                UpdateData();
            }
        }

        public string EmailSearchQuery
        {
            get => _emailSearchQuery;
            set
            {
                _emailSearchQuery = value;
                UpdateData();
            }
        }

        public string PhoneSearchQuery
        {
            get => _phoneSearchQuery;
            set
            {
                _phoneSearchQuery = value;
                UpdateData();
            }
        }

        private void UpdateData()
        {
            ClientsView.Refresh();
        }

        public ClientsPageViewModel()
        {
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
            ClientsView.Filter += o =>
            {
                var client = (Client) o;
                var fullNameMatch = client.FullName.IsMatch(FullNameSearchQuery);
                var emailMatch = client.Email.IsMatch(EmailSearchQuery);
                var phoneMatch = client.Phone.IsMatch(PhoneSearchQuery);

                return fullNameMatch && emailMatch && phoneMatch;
            };

            LoadData();
        }

        private async void LoadData()
        {
            var synchronizationContext = SynchronizationContext.Current;

            if (_clients.Any())
            {
                _clients.Clear();
            }

            using (var db = new AppContext())
            {
                await db.Genders.LoadAsync();

                await db.Clients.ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => _clients.Add(x), null);
                });
            }
        }
    }
}
