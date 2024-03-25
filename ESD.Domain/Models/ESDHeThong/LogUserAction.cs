using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    public class LogUserAction : Auditable
    {
        [Key]
        [Required]
        public string ID { get; set; }
        public string Action { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }
        public int IDOrgan { get; set; }

    }
}
