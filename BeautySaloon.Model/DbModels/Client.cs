namespace BeautySaloon.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("Client")]
    public partial class Client : ICloneable, IDataErrorInfo
    {
        public string this[string columnName]
        { 
            get
            {
                return columnName switch
                {
                    
                    _ => ""
                };
            }
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Patronymic { get; set; }

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
        public DateTime? Birthday { get; set; }

        public DateTime RegistrationDate { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(1)]
        public string GenderCode { get; set; }

        [StringLength(1000)]
        public string PhotoPath { get; set; }

        public Gender Gender { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<ClientService> ClientServices { get; set; } = new List<ClientService>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<Tag> Tags { get; set; } = new List<Tag>();

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
                Tags.ForEach(x => html += $"<div style=\"color: #{x.Color};\">{x.Title}</div>");
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

        public string Error => throw new NotImplementedException();

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
                PhotoPath = PhotoPath
            };
        }
    }
}
