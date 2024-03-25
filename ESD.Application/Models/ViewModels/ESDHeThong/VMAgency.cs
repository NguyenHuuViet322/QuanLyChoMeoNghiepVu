using ESD.Domain.Models.Abstractions;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ESD.Application.Models.ViewModels
{
    public class VMAgency : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [DisplayName("Mã đơn vị")]
        [MaxLength(20)]
        public string Code { get; set; }

        [DisplayName("Tên đơn vị")]
        public string Name { get; set; }

        public int Status { get; set; }

        [DisplayName("Đơn vị cha")]
        public string ParentName { get; set; }

        public string ParentIdStr { get; set; }

        public int ParentId { get; set; }

        public IEnumerable<Agency> Parents { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        public int OrganId { get; set; }

        [DisplayName("Cơ quan")]
        public string OrganName { get; set; }

        public string ParentPath { get; set; }
    }

    public class AgencyCondition
    {
        public AgencyCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class HierachyAgency
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public bool HasChild { get; set; }
    }
}
