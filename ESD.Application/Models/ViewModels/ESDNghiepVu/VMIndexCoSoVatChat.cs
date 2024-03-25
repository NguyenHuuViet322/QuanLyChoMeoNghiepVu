using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexCoSoVatChat
    {
        public PaginatedList<VMCoSoVatChat> CoSoVatChats { get; set; } 
        public CoSoVatChatCondition SearchParam { get; set; }

        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();
        public IEnumerable<CoSoVatChat_DonVi> CoSoVatChat_DonVis { get; set; } = new List<CoSoVatChat_DonVi>();
    }
}