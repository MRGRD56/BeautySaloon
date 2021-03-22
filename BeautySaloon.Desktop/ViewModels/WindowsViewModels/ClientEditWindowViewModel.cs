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

        public bool IsEditMode { get; set; }

        public Client EditingClient { get; set; }

        public bool Result { get; set; }

        /// <summary>
        /// Теги, которые можно добавить клиенту.
        /// </summary>
        public ObservableCollection<Tag> AvailableTags { get; set; } = new();

        /// <summary>
        /// Теги клиента.
        /// </summary>
        public ObservableCollection<Tag> ClientTags { get; set; } = new();

        public RelayCommand LoadPhotoCommand => new(o =>
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp";
            var dialogSuccess = openFileDialog.ShowDialog();
            if (dialogSuccess == true)
            {
                var fileName = openFileDialog.FileName;
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Length > 1024 * 1024)
                {
                    MBox.ShowError("Размер изображения не должен превышать 2 МБ");
                    return;
                }

                var fileShortName = fileInfo.Name;
                var newFilePath = Path.Combine("Клиенты\\", fileShortName);
                var newFileAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), "Images\\", newFilePath);
                File.Copy(fileName, newFileAbsolutePath, true);
                EditingClient.PhotoPath = newFilePath;
                OnPropertyChanged(nameof(IsPhotoLoaded));
            }
        });

        public bool IsPhotoLoaded => !string.IsNullOrWhiteSpace(EditingClient.PhotoPath);

        public RelayCommand OkCommand => new(o =>
        {
            var window = (ClientEditWindow)o;

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
            EditingClient.Gender = gender;
            window.Close();
        });

        public RelayCommand CancelCommand => new(o =>
        {
            var window = (ClientEditWindow)o;

            window.Close();
        });

        public bool IsMale
        {
            get => _isMale;
            set
            {
                _isMale = value;
                OnPropertyChanged();
            }
        }

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

        private async void InitializeTags(bool isEditMode)
        {
            var synchronizationContext = SynchronizationContext.Current;
            using var db = new AppContext();
            await db.Tags.ForEachAsync(tag =>
            {
                synchronizationContext.Send(o => 
                {
                    if (!isEditMode || EditingClient.Tags.All(x => x.ID != tag.ID))
                    {
                        AvailableTags.Add(tag);
                    }
                    else
                    {
                        ClientTags.Add(tag);
                    }
                }, null);
            });
        }

        public RelayCommand AddTagCommand => new(o =>
        {
            var tag = (Tag) o;
            ClientTags.Add(tag);
            AvailableTags.Remove(tag);
        });

        public RelayCommand RemoveTagCommand => new(o =>
        {
            var tag = (Tag) o;
            ClientTags.Remove(tag);
            AvailableTags.Add(tag);
        });
    }
}
