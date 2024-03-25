using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMUpdateDocPlan
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        [Required(ErrorMessage = "Không xác định được hồ sơ")]
        public int IDProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public List<VMDocField> VMDocFields { get; set; }
    }
}
