using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESD.Domain.Models.ESDNghiepVu;

namespace ESD.Application.Models.ViewModels.ESDNghiepVu
{
    public class VMUpdateCoSoVatChat : VMCoSoVatChat
    {
        [Required]
        [Display(Name = "Đối tượng sử dụng")]
        public int IDDonViNghiepVu { get; set; }
        public IEnumerable<DonViNghiepVu> DonViNghiepVus { get; set; } = new List<DonViNghiepVu>();
    }
}