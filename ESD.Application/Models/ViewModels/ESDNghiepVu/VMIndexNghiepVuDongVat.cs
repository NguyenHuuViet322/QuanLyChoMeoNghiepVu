using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMIndexNghiepVuDongVat
    {
        public PaginatedList<VMNghiepVuDongVat> NghiepVuDongVats { get; set; } 
        public NghiepVuDongVatCondition SearchParam { get; set; } 


    }
}