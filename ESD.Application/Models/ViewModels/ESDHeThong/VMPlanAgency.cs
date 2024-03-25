using System;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMPlanAgency
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDPlan { get; set; }

        [Required]
        public int IDAgency { get; set; }

        public int Status { get; set; } = 1;

        public DateTime? CreateDate { get; set; }
        public int? CreatedBy { get; set; }
    }

}
