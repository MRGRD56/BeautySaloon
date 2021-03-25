using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
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
using BeautySaloon.Desktop.Extensions.ModelsExtensions;
using BeautySaloon.Desktop.Views.Windows;
using BeautySaloon.Library;
using BeautySaloon.Model.DbModels;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.ViewModels.PagesViewModels
{
    public class ClientsPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Команда изменения критерия сортировки клиентов.
        /// </summary>
        public RelayCommand ChangeSortCommand => new(o =>
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

        /// <summary>
        /// Команда перехода на предыдущую страницу.
        /// </summary>
        public RelayCommand PreviousPageCommand => new(o =>
        {
            CurrentPage--;
            UpdateData();
        }, o => CurrentPage > 1);

        /// <summary>
        /// Команда перехода на следующую страницу.
        /// </summary>
        public RelayCommand NextPageCommand => new(o =>
        {
            CurrentPage++;
            UpdateData();
        }, o => CurrentPage < PagesCount);

        /// <summary>
        /// Команда изменения количества записей на одной странице.
        /// </summary>
        public RelayCommand ChangeOnePageItemsCount => new(o =>
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

        /// <summary>
        /// Критерии фильтрации по дате рождения.
        /// </summary>
        public List<string> BirthdayFilters { get; set; } = new()
        {
            "Все",
            "В текущем месяце"
        };

        /// <summary>
        /// Индекс выбранного критерия фильтрации по дате рождения.
        /// </summary>
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

        /// <summary>
        /// Индекс первого элемента на текущей странице.
        /// </summary>
        private int CurrentPageFirstItemIndex => (CurrentPage - 1) * OnePageItemsCount;

        /// <summary>
        /// Номер текущей страницы (начиная с 1).
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Количество страниц.
        /// </summary>
        public int PagesCount
        {
            get => _pagesCount;
            set
            {
                _pagesCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Количество элементов на одной странице.
        /// </summary>
        public int OnePageItemsCount
        {
            get => _onePageItemsCount;
            set
            {
                _onePageItemsCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Определяет, отображать ли все записи.
        /// </summary>
        public bool ShowAllItems
        {
            get => _showAllItems;
            set
            {
                _showAllItems = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Количество отображаемых записей на текущей странице, учитывая фильтрацию.
        /// </summary>
        public int ShownItemsCount
        {
            get => _shownItemsCount;
            set
            {
                _shownItemsCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Общее количество клиентов в БД.
        /// </summary>
        public int TotalItemsCount
        {
            get => _totalItemsCount;
            set
            {
                _totalItemsCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Объект, обозначающий оба пола.
        /// </summary>
        private readonly Gender _allGendersGender = new()
        {
            Code = "All",
            Name = "Все"
        };
        
        public List<Gender> Genders { get; set; } = new();

        /// <summary>
        /// Все клиенты.
        /// </summary>
        private readonly ObservableCollection<Client> _clients = new();

        /// <summary>
        /// Отображаемые клиенты.
        /// </summary>
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

        /// <summary>
        /// Поисковый запрос по ФИО.
        /// </summary>
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

        /// <summary>
        /// Поисковый запрос по Email.
        /// </summary>
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

        /// <summary>
        /// Поисковый запрос по номеру телефона.
        /// </summary>
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

        /// <summary>
        /// Поисковый запрос по полу.
        /// </summary>
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

        /// <summary>
        /// Команда сброса всех фильтров.
        /// </summary>
        public RelayCommand ResetFilterCommand => new(o =>
        {
            FullNameSearchQuery = "";
            EmailSearchQuery = "";
            PhoneSearchQuery = "";
            GenderSearchQuery = _allGendersGender;
        });

        /// <summary>
        /// Обновляет отображаемые записи в соответствии с фильтрами и номером текущей страницы.
        /// </summary>
        private void UpdateData()
        {
            ClientsView.Refresh();
            ShownItemsCount = ClientsView.Cast<Client>().Count();
        }

        /// <summary>
        /// Толщина шрифта для кнопки сортировки по фамилии.
        /// </summary>
        public FontWeight LastNameButtonFontWeight
        {
            get => _lastNameButtonFontWeight;
            set
            {
                _lastNameButtonFontWeight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Толщина шрифта для кнопки сортировки по последнему посещению.
        /// </summary>
        public FontWeight LastVisitButtonFontWeight
        {
            get => _lastVisitButtonFontWeight;
            set
            {
                _lastVisitButtonFontWeight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Толщина шрифта для кнопки сортировки по количеству посещений.
        /// </summary>
        public FontWeight VisitsCountButtonFontWeight
        {
            get => _visitsCountButtonFontWeight;
            set
            {
                _visitsCountButtonFontWeight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Определяет, соответствует ли клиент поисковому запросу.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Выбранный клиент.
        /// </summary>
        public Client SelectedClient { get; set; }

        /// <summary>
        /// Команда добавления нового клиента.
        /// </summary>
        public RelayCommand AddClientCommand => new(o =>
        {
            var db = new AppContext();
            db.Database.Log = str => Debug.WriteLine(str);
            var clientAddDialog = new ClientEditWindow();
            clientAddDialog.ShowDialog();
            if (!clientAddDialog.Result) return;
            var dialogClient = clientAddDialog.Client;
            dialogClient.Gender = db.Genders.Find(dialogClient.Gender.Code);
            dialogClient.RegistrationDate = DateTime.Now;
            db.Clients.Add(dialogClient);
            db.SaveChanges();
            dialogClient.SetTags(clientAddDialog.ClientTags, db);
            _clients.Add(dialogClient);
        });

        /// <summary>
        /// Команда редактирования выбранного клиента.
        /// </summary>
        public RelayCommand EditClientCommand => new(o =>
        {
            var db = new AppContext();
            db.Database.Log = str => Debug.WriteLine(str);
            var clientEditDialog = new ClientEditWindow(SelectedClient);
            clientEditDialog.ShowDialog();
            if (!clientEditDialog.Result) return;
            var dialogClient = clientEditDialog.Client;
            var dbClient = db.Clients
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.ID == dialogClient.ID);
            dbClient.CopyPropertiesFrom(dialogClient, db);
            SelectedClient.CopyPropertiesFrom(dialogClient, db);
            dbClient.SetTags(clientEditDialog.ClientTags, db);
            db.SaveChanges();
            SelectedClient.SetTagsNoDb(clientEditDialog.ClientTags);
        }, o => SelectedClient != null);

        /// <summary>
        /// Команда удаления выбранного клиента.
        /// </summary>
        public RelayCommand DeleteClientCommand => new(async o =>
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
                db.Database.ExecuteSqlCommand($"DELETE FROM TagOfClient WHERE ClientID = {dbClient.ID}");
                db.Clients.Remove(dbClient);
                await db.SaveChangesAsync();
            }

            _clients.Remove(SelectedClient);
            UpdateData();
        }, o => SelectedClient != null);

        /// <summary>
        /// Команда просмотра посещений выбранного клиента.
        /// </summary>
        public RelayCommand ClientVisitsCommand => new(o =>
        {
            var windows = App.Current.Windows.Cast<Window>().ToList();
            var windowExists = false;
            foreach (var window in windows)
            {
                if (window is ClientVisitsWindow clientVisitsWindow && clientVisitsWindow.Client.ID == SelectedClient.ID)
                {
                    clientVisitsWindow.Focus();
                    windowExists = true;
                    break;
                }
            }

            if (!windowExists)
            {
                new ClientVisitsWindow(SelectedClient).Show();
            }
        }, o => SelectedClient != null && SelectedClient.VisitsCount > 0);

        public ClientsPageViewModel()
        {
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
            ClientsView.Filter += o =>
            {
                var client = (Client)o;

                var searchMatch = GetClientSearchMatch(client);

                //TODO: Переписать.
                //Более удачный пример фильтрации с постраничным отображением: 
                //https://github.com/MRGRD56/DataFilteringWithPages

                //Клиенты, соответствующие поисковому запросу.
                var shownClients = _clients.Where(GetClientSearchMatch).ToList();

                //Отсортированные записи.
                List<Client> shownClientsOrdered;
                var sortDescription = ClientsView.SortDescriptions.FirstOrDefault();
                //Имя свойства, по которому происходит сортировка.
                var propertyName = sortDescription.PropertyName;
                //Определение, как сортировать записи.
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

                //Клиент из списка shownClients.
                var listClient = shownClients.FirstOrDefault(x => x.ID == client.ID);
                //Определяет, нужно ли отображать запись на текущей странице.
                var indexMatch = false;
                if (listClient != null)
                {
                    var index = shownClientsOrdered.IndexOf(listClient);
                    //Если запись находится между первой и последней записью НА СТРАНИЦЕ (включительно), её нужно отображать.
                    indexMatch = index >= CurrentPageFirstItemIndex && index < CurrentPageFirstItemIndex + OnePageItemsCount;
                }

                return searchMatch && indexMatch;
            };
            //Задаём сортировку по фамилии.
            ClientsView.SortDescriptions.Add(new SortDescription("LastName", ListSortDirection.Ascending));

            LoadData();
        }

        /// <summary>
        /// Загружает данные из БД.
        /// </summary>
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
                await db.DocumentByServices.LoadAsync();

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

                await db.Clients
                    .Include(x => x.Tags)
                    .Include(x => x.ClientServices
                        .Select(y => y.DocumentByServices))
                    .ForEachAsync(x =>
                {
                    synchronizationContext.Send(o => _clients.Add(x), null);
                });
            }
        }
    }
}
