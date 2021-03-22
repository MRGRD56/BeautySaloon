namespace BeautySaloon.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Linq;
    using BeautySaloon.Library;

    [Table("Client")]
    public partial class Client : BaseModel, ICloneable, IDataErrorInfo
    {
        private string _photoPath;
        private string _firstName;
        private string _lastName;
        private string _patronymic;
        private DateTime? _birthday;
        private string _email;
        private string _phone;
        private Gender _gender;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(LastName) => Validation.IsValidName(LastName) ? "" : "Некорректная фамилия",
                    nameof(FirstName) => Validation.IsValidName(FirstName) ? "" : "Некорректное имя",
                    nameof(Patronymic) => Validation.IsValidName(Patronymic) ? "" : "Некорректное отчество",
                    nameof(Email) => Validation.IsValidEmail(Email) ? "" : "Некорректный email",
                    nameof(Phone) => Validation.IsValidPhone(Phone) ? "" : "Некорректный номер телефона",
                    _ => ""
                };
            }
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        [Required]
        [StringLength(50)]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        [StringLength(50)]
        public string Patronymic
        {
            get => _patronymic;
            set
            {
                _patronymic = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get
            {
                if (FirstName == null || LastName == null || Patronymic == null)
                {
                    return "";
                }

                return $"{LastName} {FirstName} {Patronymic}";
            }
        }

        [Column(TypeName = "date")]
        public DateTime? Birthday
        {
            get => _birthday;
            set
            {
                _birthday = value;
                OnPropertyChanged();
            }
        }

        public DateTime RegistrationDate { get; set; }

        [StringLength(255)]
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        [Required]
        [StringLength(20)]
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        [Required]
        [StringLength(1)]
        public string GenderCode { get; set; }

        [StringLength(1000)]
        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                _photoPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PhotoName));
                OnPropertyChanged(nameof(PhotoRelativePath));
                OnPropertyChanged(nameof(PhotoAbsolutePath));
            }
        }

        public string PhotoName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoPath))
                {
                    return "не загружено";
                }

                return new FileInfo(PhotoPath).Name;
            }
        }

        public string PhotoRelativePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoPath))
                {
                    return null;
                }

                return Path.Combine("Images\\", PhotoPath);
            }
        }

        public string PhotoAbsolutePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PhotoPath))
                {
                    return null;
                }

                return Path.Combine(Directory.GetCurrentDirectory(), PhotoRelativePath);
            }
        }

        public Gender Gender { get => _gender; set => _gender = value; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ClientService> ClientServices { get; set; } = new List<ClientService>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<Tag> Tags { get; set; } = new();

        public string TagsHtml
        {
            get
            {
                if (!Tags.Any())
                {
                    return "";
                }

                var html = "";
                html += "<style>*, body { margin: 0; padding: 0; }</style>";
                html += "<div style=\"display: flex; flex-wrap: wrap;\">";
                Tags.ToList().ForEach(x => html += $"<div style=\"color: #{x.Color};\">{x.Title}</div>");
                html += "</div>";
                return html;
            }
        }

        public ClientService LastClientService
        {
            get
            {
                if (!ClientServices.Any())
                {
                    return null;
                }

                return ClientServices
                    .OrderBy(x => x.StartTime)
                    .Last();
            }
        }

        public DateTime LastVisitDate => LastClientService?.StartTime ?? default;

        public string LastVisitDateString
        {
            get
            {
                if (!ClientServices.Any() || LastClientService == null || LastClientService.StartTime == default)
                {
                    return "нет";
                }

                return LastClientService.StartTime.ToString("dd.MM.yyyy");
            }
        }

        public int VisitsCount => ClientServices.Count;

        private static string NotNullAddNewLine(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str + "\n";
            }

            return "";
        }

        public string Error
        {
            get
            {
                return NotNullAddNewLine(this[nameof(LastName)])
                     + NotNullAddNewLine(this[nameof(FirstName)])
                     + NotNullAddNewLine(this[nameof(Patronymic)])
                     + NotNullAddNewLine(this[nameof(Email)])
                     + NotNullAddNewLine(this[nameof(Phone)]);
            }
        }

        public object Clone()
        {
            return new Client
            {
                ID = ID,
                LastName = LastName,
                FirstName = FirstName,
                Patronymic = Patronymic,
                Email = Email,
                Phone = Phone,
                Birthday = Birthday,
                Gender = Gender,
                GenderCode = GenderCode,
                PhotoPath = PhotoPath,
                Tags = Tags
            };
        }
    }
}
