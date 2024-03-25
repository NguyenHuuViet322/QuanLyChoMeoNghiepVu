using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ESD.Domain.Models.DAS
{
    public class RoleGroupPer
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDRole { get; set; }

        [Required]
        public int IDGroupPermission { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public int Status { get; set; } = 1;

        //public DateTime? UpdatedDate { get; set; }

        //public int? UpdatedBy { get; set; }
    }
}