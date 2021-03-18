namespace BeautySaloon.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ServicePhoto")]
    public partial class ServicePhoto
    {
        public int ID { get; set; }

        public int ServiceID { get; set; }

        [Required]
        [StringLength(1000)]
        public string PhotoPath { get; set; }

        public virtual Service Service { get; set; }
    }
}
