using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESD.Domain.Models.ESDNghiepVu;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMUpdateThongTinCanBo : VMThongTinCanBo
    {
       
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();
        public IEnumerable<ChuyenMonKiThuat> ChuyenMonKiThuats { get; set; } = new List<ChuyenMonKiThuat>();
    }
}