using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace ESD.Domain.Models.DAS
{
    public class Module
    {
        [Key]
        [Required]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int ParentId { get; set; }
        public string ParentPath { get; set; }

        [MaxLength(10)]
        public int Code { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Url { get; set; }
        [MaxLength(30)]
        public string Icon { get; set; }

        public int SortOrder { get; set; }

        public int Status { get; set; } = 1;
        public int IsShow { get; set; } = 1;
        [MaxLength(250)]
        [Description("Tên Controller")]
        public string Controller { get; set; }
        [MaxLength(500)]
        [Description("Tên RouterName")]
        public string RouterName { get; set; }
    }
}
