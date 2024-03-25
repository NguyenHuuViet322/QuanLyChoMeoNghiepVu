using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMUpdateCatalogingBorrow
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public int IDReader { get; set; } = 0;

        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public int IDUser { get; set; } = 0;

        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public string Reader { get; set; }

        [Display(Name = "Hình thức mượn", Prompt = "Hình thức mượn")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? ReaderType { get; set; }

        [Display(Name = "Mục đích khai thác", Prompt = "Mục đích khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được vượt quá {1} ký tự")]
        public string Purpose { get; set; }

        [Display(Name = "Mượn từ ngày", Prompt = "Mượn từ ngày")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string StrStartDate { get; set; }

        [Display(Name = "Đến ngày", Prompt = "Đến ngày")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string StrEndDate { get; set; }

        [Display(Name = "Mượn bản cứng?", Prompt = "Mượn bản cứng?")]
        public int IsOriginal { get; set; }

        public Dictionary<int, string> DictUser { get; set; }
        public Dictionary<int, string> DictReader { get; set; }
        public int[] IDs { get; set; } //Id phiếu
        public IEnumerable<VMCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; } //For detail, update, create
    }
}