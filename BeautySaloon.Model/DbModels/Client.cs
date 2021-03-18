namespace BeautySaloon.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("Client")]
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            //ClientServices = new HashSet<ClientService>();
            //Tags = new HashSet<Tag>();
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
    }
}
