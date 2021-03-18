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
using System.Windows.Media;
using BeautySaloon.Context;
using BeautySaloon.Library;
using BeautySaloon.Model.DbModels;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.ViewModels.PagesViewModels
{
    public class ClientsPageViewModel : BaseViewModel
    {
        private readonly Gender _allGendersGender = new Gender
        {
            Code = "All",
            Name = "Все"
        };

        public List<Gender> Genders { get; set; } = new List<Gender>();

        private readonly ObservableCollection<Client> _clients = new ObservableCollection<Client>();

        public ICollectionView ClientsView { get; set; }

        private string _fullNameSearchQuery;
        private string _emailSearchQuery;
        private string _phoneSearchQuery;
        private Gender _genderSearchQuery;

        public string FullNameSearchQuery
        {
            get => _fullNameSearchQuery;
            set
            {
                _fullNameSearchQuery = value;
                OnPropertyChanged();
                UpdateData();
            }
        }

        public string EmailSearchQuery
        {
            get => _emailSearchQuery;
            set
            {
                _emailSearchQuery = value;
                OnPropertyChanged();
                UpdateData();
            }
        }

        public string PhoneSearchQuery
        {
            get => _phoneSearchQuery;
            set
            {
                _phoneSearchQuery = value;
                OnPropertyChanged();
                UpdateData();
            }
        }

        public Gender GenderSearchQuery
        {
            get => _genderSearchQuery;
            set
            {
                _genderSearchQuery = value;
                OnPropertyChanged();
                UpdateData();
            }
        }

        public RelayCommand ResetFilterCommand => new RelayCommand(o =>
        {
            FullNameSearchQuery = "";
            EmailSearchQuery = "";
            PhoneSearchQuery = "";
            GenderSearchQuery = _allGendersGender;
        });

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
                var genderMatch = GenderSearchQuery == null || 
                    GenderSearchQuery.Code == _allGendersGender.Code ||
                    client.Gender.Code == GenderSearchQuery.Code;

                return fullNameMatch && emailMatch && phoneMatch && genderMatch;
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
                //await db.Genders.LoadAsync();

                synchronizationContext.Send(o => 
                {
                    Genders.Add(_allGendersGender);
                }, null);

                await db.Genders.ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => Genders.Add(x), null);
                });

                await db.Clients.ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => _clients.Add(x), null);
                });

                GenderSearchQuery = _allGendersGender;
            }
        }
    }
}
