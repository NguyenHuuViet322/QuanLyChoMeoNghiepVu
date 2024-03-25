using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ESD.Domain.Models.Abstractions;
using System;
namespace ESD.Domain.Models.DAS
{
    public class DeliveryRecord : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }
        [Required]
        public DateTime RecordCreateDate { get; set; }
        [Required]
        public int IDPlan { get; set; }
        [Required]
        public int IDAgency { get; set; }
        [Required]
        public int IDSendUser { get; set; }
        [Required]
        public int IDReceiveUser { get; set; }
        public int Status { get; set; }
        public string DocumentName { get; set; }
        public string DocumentTime { get; set; }
        public string DocumentReceiveStatus { get; set; }
        public string Reason { get; set; }
        public int IDTemplate { get; set; }
        public string lstDeliveryPlanProfile { get; set; }
    }
}
