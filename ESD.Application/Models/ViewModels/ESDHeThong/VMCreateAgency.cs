using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMCreateAgency : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Mã đơn vị")]
        [Required(ErrorMessage = "Mã đơn vị không được để trống")]
        [MaxLength(20)]
        public string Code { get; set; }

        [DisplayName("Tên đơn vị")]
        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        public string Name { get; set; }

        public int Status { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Đơn vị cha")]
        public string ParentName { get; set; }

        public string ParentIdStr { get; set; }

        public int ParentId { get; set; }

        public IEnumerable<Agency> Parents { get; set; }

        [DisplayName("Cơ quan")]
        [Required(ErrorMessage = "Cơ quan không được để trống")]
        //[Range(typeof(int), "1", "??", ErrorMessage = "Cơ quan không được để trống")]
        public int? IDOrgan { get; set; }

        public List<Organ> Organs { get; set; }

        public string ParentPath { get; set; }
    }
}
