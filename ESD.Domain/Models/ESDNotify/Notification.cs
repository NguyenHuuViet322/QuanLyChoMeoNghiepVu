using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DASNotify
{
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        public int UserId { get; set; }

        [MaxLength(500)]
        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Url { get; set; }

    }

   

}
