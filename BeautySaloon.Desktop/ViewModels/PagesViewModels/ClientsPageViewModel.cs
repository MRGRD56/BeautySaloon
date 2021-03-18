using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public RelayCommand PreviousPageCommand => new RelayCommand(o =>
        {
            CurrentPage--;
            UpdateData();
        }, o => CurrentPage > 1);

        public RelayCommand NextPageCommand => new RelayCommand(o =>
        {
            CurrentPage++;
            UpdateData();
        }, o => CurrentPage < PagesCount);

        public RelayCommand ChangeOnePageItemsCount => new RelayCommand(o =>
        {
            var button = (Button)o;
            var buttonContent = button.Content.ToString();
            if (int.TryParse(buttonContent, out var itemsCount))
            {
                ShowAllItems = false;
                OnePageItemsCount = itemsCount;
            }
            else if (buttonContent == "ВСЕ")
            {
                ShowAllItems = true;
                OnePageItemsCount = TotalItemsCount;
            }
            else
            {
                throw new Exception($"ChangeOnePageItemsCount: button.Content is '{buttonContent}'");
            }

            CurrentPage = 1;
            PagesCount = Convert.ToInt32(Math.Ceiling((double) TotalItemsCount / OnePageItemsCount));
            UpdateData();
        });

        private int CurrentPageFirstItemIndex => (CurrentPage - 1) * OnePageItemsCount;

        public int CurrentPage 
        { 
            get => _currentPage; 
            set 
            {
                _currentPage = value;
                OnPropertyChanged();
            } 
        }

        public int PagesCount
        {
            get => _pagesCount;
            set
            {
                _pagesCount = value;
                OnPropertyChanged();
            }
        }

        public int OnePageItemsCount
        {
            get => _onePageItemsCount;
            set
            {
                _onePageItemsCount = value;
                OnPropertyChanged();
            }
        }

        public bool ShowAllItems
        {
            get => _showAllItems;
            set
            {
                _showAllItems = value;
                OnPropertyChanged();
            }
        }

        public int ShownItemsCount
        {
            get => _shownItemsCount;
            set
            {
                _shownItemsCount = value;
                OnPropertyChanged();
            }
        }

        public int TotalItemsCount
        {
            get => _totalItemsCount;
            set
            {
                _totalItemsCount = value;
                OnPropertyChanged();
            }
        }


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
        private int _totalItemsCount;
        private int _shownItemsCount;
        private bool _showAllItems = true;
        private int _onePageItemsCount;
        private int _pagesCount;
        private int _currentPage = 1;

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
            ShownItemsCount = ClientsView.Cast<Client>().Count();
        }

        public ClientsPageViewModel()
        {
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
            ClientsView.Filter += o =>
            {
                var client = (Client)o;

                var fullNameMatch = client.FullName.IsMatch(FullNameSearchQuery);
                var emailMatch = client.Email.IsMatch(EmailSearchQuery);
                var phoneMatch = client.Phone.IsMatch(PhoneSearchQuery);
                var genderMatch = GenderSearchQuery == null ||
                    GenderSearchQuery.Code == _allGendersGender.Code ||
                    client.Gender.Code == GenderSearchQuery.Code;

                var shownClients = _clients
                    .Where(x =>
                    {
                        return x.FullName.IsMatch(FullNameSearchQuery) &&
                            x.Email.IsMatch(EmailSearchQuery) &&
                            x.Phone.IsMatch(PhoneSearchQuery) &&
                            (GenderSearchQuery == null ||
                            GenderSearchQuery.Code == _allGendersGender.Code ||
                            x.Gender.Code == GenderSearchQuery.Code);
                    })
                    .ToList();
                var listClient = shownClients.FirstOrDefault(x => x.ID == client.ID);
                bool indexMatch = false;
                if (listClient != null)
                {
                    var index = shownClients.IndexOf(listClient);
                    indexMatch = index >= CurrentPageFirstItemIndex && index < CurrentPageFirstItemIndex + OnePageItemsCount;
                }

                return fullNameMatch && emailMatch && phoneMatch && genderMatch && indexMatch;
            };

            //ClientsView.Filter += o =>
            //{
            //    if (ShowAllItems)
            //    {
            //        return true;
            //    }

            //    var client = (Client)o;

            //    var shownClientsList = ClientsView.Cast<Client>().ToList();
            //    var index = shownClientsList.IndexOf(client);
            //    var pageMatch = index >= CurrentPageFirstItemIndex && index < CurrentPageFirstItemIndex + OnePageItemsCount;
            //    return pageMatch;
            //};

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

                GenderSearchQuery = _allGendersGender;
                TotalItemsCount = await db.Clients.CountAsync();
                ShownItemsCount = TotalItemsCount;
                OnePageItemsCount = TotalItemsCount;
                PagesCount = 1;

                await db.Clients.ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => _clients.Add(x), null);
                });
            }
        }
    }
}
