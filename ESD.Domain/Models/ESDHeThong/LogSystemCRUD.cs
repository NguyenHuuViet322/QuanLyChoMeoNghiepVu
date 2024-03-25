using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class LogSystemCRUD : Auditable
    {
        [Key]
        [Required]
        public string ID { get; set; }
        public int TagName { get; set; }
        public string Entity { get; set; }
        public string Action { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedValue { get; set; }
        public int IDOrgan { get; set; }

    }
}
