using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using BeautySaloon.Context;
using BeautySaloon.Desktop.Extensions;
using BeautySaloon.Desktop.Views.Windows;
using BeautySaloon.Model.DbModels;
using Microsoft.Win32;
using AppContext = BeautySaloon.Context.AppContext;

namespace BeautySaloon.Desktop.ViewModels.WindowsViewModels
{
    public class ClientEditWindowViewModel : BaseViewModel
    {
        private bool _isMale;
        private bool _isFemale;

        /// <summary>
        /// Определяет, редактируется ли клиент. Если нет, значит, добавляется новый.
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Добавляемый/редактируемый клиент.
        /// </summary>
        public Client EditingClient { get; set; }

        /// <summary>
        /// Определяет, нужно ли сохранить пользователя.<br/>
        /// Равен true, если пользователь нажал 'Готово' в окне добавления/редактирования клиента.<br/>
        /// Иначе, если пользователь нажал 'Отмена' или закрыл окно, равен false.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Теги, которые можно добавить клиенту. Это все теги в БД, исключая теги из коллекции <see cref="ClientTags"/>.
        /// </summary>
        public ObservableCollection<Tag> AvailableTags { get; set; } = new();

        /// <summary>
        /// Теги клиента.
        /// </summary>
        public ObservableCollection<Tag> ClientTags { get; set; } = new();

        /// <summary>
        /// Команда загрузки фотографии клиента.
        /// </summary>
        public RelayCommand LoadPhotoCommand => new(o =>
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp";
            var dialogSuccess = openFileDialog.ShowDialog();
            if (dialogSuccess == true)
            {
                var fileName = openFileDialog.FileName;
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Length > 2 * 1024 * 1024)
                {
                    MBox.ShowError("Размер изображения не должен превышать 2 МБ");
                    return;
                }

                //Имя загружаемого файла.
                var fileShortName = fileInfo.Name;
                //Относительный путь, куда будет скопировано изображение.
                var newFilePath = Path.Combine("Клиенты\\", fileShortName);
                //Абсолютный путь, куда будет скопировано изображение.
                var newFileAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\", newFilePath);
                //Копирование файла в newFileAbsolutePath с перезаписью.
                File.Copy(fileName, newFileAbsolutePath, true);
                EditingClient.PhotoPath = newFilePath;
                OnPropertyChanged(nameof(IsPhotoLoaded));
            }
        });

        /// <summary>
        /// Определяет, имеет ли клиент фотографию.
        /// </summary>
        public bool IsPhotoLoaded => !string.IsNullOrWhiteSpace(EditingClient.PhotoPath);

        /// <summary>
        /// Команда ОК.
        /// </summary>
        public RelayCommand OkCommand => new(o =>
        {
            var window = (ClientEditWindow)o;

            //Проверка введённых данных.
            if (string.IsNullOrWhiteSpace(EditingClient.LastName) ||
                string.IsNullOrWhiteSpace(EditingClient.FirstName) ||
                string.IsNullOrWhiteSpace(EditingClient.Patronymic) ||
                string.IsNullOrWhiteSpace(EditingClient.Email) ||
                string.IsNullOrWhiteSpace(EditingClient.Phone) ||
                EditingClient.Birthday == default ||
                !IsFemale && !IsMale ||
                string.IsNullOrWhiteSpace(EditingClient.PhotoPath))
            {
                MBox.ShowError("Заполнены не все поля!");
                return;
            }

            if (!string.IsNullOrWhiteSpace(EditingClient.Error))
            {
                MBox.ShowError(EditingClient.Error);
                return;
            }

            using var db = new AppContext();

            var gender = IsMale
                ? db.Genders.Find("м")
                : db.Genders.Find("ж");

            Result = true;
            //Задаёт клиенту пол из БД.
            EditingClient.Gender = gender;
            window.Close();
        });

        /// <summary>
        /// Команда отмены. Закрывает окно добавления/редактирования клиента.
        /// </summary>
        public RelayCommand CancelCommand => new(o =>
        {
            var window = (ClientEditWindow)o;

            window.Close();
        });

        /// <summary>
        /// Определяет, отмечен ли мужской пол.
        /// </summary>
        public bool IsMale
        {
            get => _isMale;
            set
            {
                _isMale = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Определяет, отмечен ли женский пол.
        /// </summary>
        public bool IsFemale
        {
            get => _isFemale;
            set
            {
                _isFemale = value;
                OnPropertyChanged();
            }
        }
        
        public ClientEditWindowViewModel()
        {
            EditingClient = new Client();
            IsEditMode = false;
            InitializeTags(false);
        }

        public ClientEditWindowViewModel(Client client)
        {
            EditingClient = (Client)client.Clone();
            IsEditMode = true;
            IsMale = client.GenderCode == "м";
            IsFemale = client.GenderCode == "ж";
            InitializeTags(true);
        }

        /// <summary>
        /// Заполняет коллекции <see cref="AvailableTags"/> и <see cref="ClientTags"/>.
        /// </summary>
        /// <param name="isEditMode"></param>
        private async void InitializeTags(bool isEditMode)
        {
            var synchronizationContext = SynchronizationContext.Current;
            using var db = new AppContext();
            await db.Tags.ForEachAsync(tag =>
            {
                synchronizationContext.Send(o => 
                {
                    //Если режим добавления или редактируемый клиент не имеет тега tag.
                    if (!isEditMode || EditingClient.Tags.All(x => x.ID != tag.ID))
                    {
                        //Добавляем tag в доступные теги.
                        AvailableTags.Add(tag);
                    }
                    else
                    {
                        //В противном случае, добавляем в теги клиента.
                        ClientTags.Add(tag);
                    }
                }, null);
            });
        }

        /// <summary>
        /// Команда добавления тега. Тег переходит из коллекции <see cref="AvailableTags"/> в коллекцию <see cref="ClientTags"/>.
        /// </summary>
        public RelayCommand AddTagCommand => new(o =>
        {
            var tag = (Tag) o;
            ClientTags.Add(tag);
            AvailableTags.Remove(tag);
        });

        /// <summary>
        /// Команда удаления тега. Тег переходит из коллекции <see cref="ClientTags"/> в коллекцию <see cref="AvailableTags"/>.
        /// </summary>
        public RelayCommand RemoveTagCommand => new(o =>
        {
            var tag = (Tag) o;
            ClientTags.Remove(tag);
            AvailableTags.Add(tag);
        });
    }
}
