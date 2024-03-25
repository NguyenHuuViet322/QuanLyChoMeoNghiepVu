using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DAS.Application.Enums;
using DAS.Utility.CustomClass;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileUpdateCatalogingBorrow : IValidatableObject
    {
        [JsonIgnore]
        public int Id { get; set; }

        //[Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        //public int IdReader { get; set; } = 0;

        [JsonIgnore]
        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public string Reader { get; set; }

        [Display(Name = "Hình thức khai thác", Prompt = "Hình thức khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? ReaderType { get; set; }

        [Display(Name = "Mục đích khai thác", Prompt = "Mục đích khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được vượt quá {1} ký tự")]
        public string Purpose { get; set; }

        [JsonIgnore]
        public List<EnumToList> ListEnumBorrowTypes { get; set; }

        [Description("Hình thức khai thác")]
        [Display(Name = "Hình thức khai thác", Prompt = "Hình thức khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int BorrowType { get; set; }

        [Description("Khai thác từ ngày")]
        [Display(Name = "Khai thác từ ngày", Prompt = "Khai thác từ ngày")]
        public string StrStartDate { get; set; }

        [Description("Khai thác đến ngày")]
        [Display(Name = "Khai thác đến ngày", Prompt = "Khai thác đến ngày")]
        public string StrEndDate { get; set; }

        [Description("Số lượng")]
        [Display(Name = "Số lượng", Prompt = "Số lượng")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public int[] Ids { get; set; } //Id phiếu

        public List<RequestCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; } //For detail, update, create

        /// <summary>
        /// Validate nâng cao
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Validate format Mã hố sơ
            if ((BorrowType == (int)EnumBorrow.BorrowType.Copy) && Quantity <= 0)
            {
                yield return new ValidationResult("Số lượng khai thác phải lớn hơn 0", new List<string> { "Quantity" });
            }

            if (BorrowType == (int)EnumBorrow.BorrowType.Online || BorrowType == (int)EnumBorrow.BorrowType.Borrow)
            {
                if (string.IsNullOrEmpty(StrStartDate) && string.IsNullOrEmpty(StrEndDate))
                {
                    yield return new ValidationResult("Khai thác từ ngày - đến ngày không được để trống", new List<string> { "StrStartDate - StrEndDate" });
                }
                else if (string.IsNullOrEmpty(StrStartDate))
                {
                    yield return new ValidationResult("Khai thác từ ngày không được để trống", new List<string> { "StrStartDate" });
                }
                else if (string.IsNullOrEmpty(StrEndDate))
                {
                    yield return new ValidationResult("Khai thác đến ngày không được để trống", new List<string> { "StrEndDate" });
                }
            }
        }
    }

    public class RequestCatalogingBorrowDoc
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int IdOrgan { get; set; }
        [JsonIgnore]
        public int BorrowType { get; set; }
        [JsonIgnore]
        public DateTime? StartDate { get; set; }
        [JsonIgnore]
        public DateTime? EndDate { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }

        public int IdProfile { get; set; } //ID bang ho so 

        public int IdDoc { get; set; } //ID tài liệu 
    }
}
