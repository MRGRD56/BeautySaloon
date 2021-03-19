using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BeautySaloon.Context;
using BeautySaloon.Desktop.Extensions;
using BeautySaloon.Library;
using BeautySaloon.Model.DbModels;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.ViewModels.PagesViewModels
{
    public class ClientsPageViewModel : BaseViewModel
    {
        public RelayCommand ChangeSortCommand => new RelayCommand(o =>
        {
            var button = (Button)o;
            var tag = button.Tag;
            ClientsView.SortDescriptions.Clear();
            ClientsView.SortDescriptions.Add(tag switch
            {
                "LastName" => new SortDescription("LastName", ListSortDirection.Ascending),
                "LastVisitDate" => new SortDescription("LastVisitDate", ListSortDirection.Descending),
                "VisitsCount" => new SortDescription("VisitsCount", ListSortDirection.Descending),
                _ => throw new Exception($"ChangeSortCommand: {tag}")
            });

            LastNameButtonFontWeight = FontWeights.Normal;
            LastVisitButtonFontWeight = FontWeights.Normal;
            VisitsCountButtonFontWeight = FontWeights.Normal;

            switch (tag)
            {
                case "LastName":
                    LastNameButtonFontWeight = FontWeights.Bold;
                    break;
                case "LastVisitDate":
                    LastVisitButtonFontWeight = FontWeights.Bold;
                    break;
                case "VisitsCount":
                    VisitsCountButtonFontWeight = FontWeights.Bold;
                    break;
            }

            UpdateData();
        });

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
            PagesCount = Convert.ToInt32(Math.Ceiling((double)TotalItemsCount / OnePageItemsCount));
            UpdateData();
        });

        public List<string> BirthdayFilters { get; set; } = new List<string>
        {
            "Все",
            "В текущем месяце"
        };

        public int SelectedBirthdayFilterIndex
        {
            get => _selectedBirthdayFilterIndex; 
            set
            {
                _selectedBirthdayFilterIndex = value;
                OnPropertyChanged();
                UpdateData();
            }
        }
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
        private FontWeight _lastNameButtonFontWeight = FontWeights.Bold;
        private FontWeight _lastVisitButtonFontWeight = FontWeights.Normal;
        private FontWeight _visitsCountButtonFontWeight = FontWeights.Normal;
        private int _selectedBirthdayFilterIndex = 0;

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

        public FontWeight LastNameButtonFontWeight
        {
            get => _lastNameButtonFontWeight;
            set
            {
                _lastNameButtonFontWeight = value;
                OnPropertyChanged();
            }
        }
        public FontWeight LastVisitButtonFontWeight
        {
            get => _lastVisitButtonFontWeight;
            set
            {
                _lastVisitButtonFontWeight = value;
                OnPropertyChanged();
            }
        }
        public FontWeight VisitsCountButtonFontWeight
        {
            get => _visitsCountButtonFontWeight;
            set
            {
                _visitsCountButtonFontWeight = value;
                OnPropertyChanged();
            }
        }

        private bool GetClientSearchMatch(Client client)
        {
            var fullNameMatch = client.FullName.IsMatch(FullNameSearchQuery);
            var emailMatch = client.Email.IsMatch(EmailSearchQuery);
            var phoneMatch = client.Phone.IsMatch(PhoneSearchQuery);
            var genderMatch = GenderSearchQuery == null ||
                GenderSearchQuery.Code == _allGendersGender.Code ||
                client.Gender.Code == GenderSearchQuery.Code;
            var birthdayMatch = SelectedBirthdayFilterIndex == 0 ||
                (client.Birthday.HasValue && client.Birthday.Value.Month == DateTime.Today.Month);

            return fullNameMatch && emailMatch && phoneMatch && genderMatch && birthdayMatch;
        }

        public Client SelectedClient { get; set; }

        public RelayCommand DeleteClientCommand => new RelayCommand(async o =>
        {
            if (SelectedClient.VisitsCount > 0)
            {
                MBox.ShowError("Невозможно удалить клиента, т.к. есть информация о посещениях.");
                return;
            }

            var mbox = MBox.ShowOkCancel($"Вы действительно хотите удалить клиента '{SelectedClient.FullName}'?", "Удаление клиента");
            if (mbox != MessageBoxResult.OK) return;

            using (var db = new AppContext())
            {
                var dbClient = await db.Clients.FindAsync(SelectedClient.ID);
                db.Clients.Remove(dbClient);
                await db.SaveChangesAsync();
            }

            _clients.Remove(SelectedClient);
            UpdateData();
        }, o => SelectedClient != null);

        public ClientsPageViewModel()
        {
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
            ClientsView.Filter += o =>
            {
                var client = (Client)o;

                var searchMatch = GetClientSearchMatch(client);

                //TODO переписать
                var shownClients = _clients.Where(x => GetClientSearchMatch(x)).ToList();

                List<Client> shownClientsOrdered;
                var sortDescription = ClientsView.SortDescriptions.FirstOrDefault();
                var propertyName = sortDescription.PropertyName;
                if (propertyName != null)
                {
                    shownClientsOrdered = propertyName switch
                    {
                        "LastName" => shownClients.OrderBy(x => x.LastName).ToList(),
                        "LastVisitDate" => shownClients.OrderByDescending(x => x.LastVisitDate).ToList(),
                        "VisitsCount" => shownClients.OrderByDescending(x => x.VisitsCount).ToList(),
                        _ => throw new Exception()
                    };
                }
                else
                {
                    shownClientsOrdered = shownClients.ToList();
                }

                var listClient = shownClients.FirstOrDefault(x => x.ID == client.ID);
                bool indexMatch = false;
                if (listClient != null)
                {
                    var index = shownClientsOrdered.IndexOf(listClient);
                    indexMatch = index >= CurrentPageFirstItemIndex && index < CurrentPageFirstItemIndex + OnePageItemsCount;
                }

                return searchMatch && indexMatch;
            };
            ClientsView.SortDescriptions.Add(new SortDescription("LastName", ListSortDirection.Ascending));

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
                await db.Tags.LoadAsync();
                await db.Services.LoadAsync();
                await db.ClientServices.LoadAsync();

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

                await db.Clients.Include(x => x.Tags).ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => _clients.Add(x), null);
                });
            }
        }
    }
}
