namespace BeautySaloon.Model.DbModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Text.RegularExpressions;

    [Table("DocumentByService")]
    public partial class DocumentByService
    {
        public int ID { get; set; }

        public int ClientServiceID { get; set; }

        [Required]
        [StringLength(1000)]
        public string DocumentPath { get; set; }

        public string FullDocumentPath => Path.Combine("Images\\", DocumentPath);

        /// <summary>
        /// Имя файла без пути к нему.
        /// </summary>
        public string DocumentFileName => Regex.Match(DocumentPath, @"[\\\/]?([^\\\/]+)$").Groups[1].Value;

        public virtual ClientService ClientService { get; set; }
    }
}
