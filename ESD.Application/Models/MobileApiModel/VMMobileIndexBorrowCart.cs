using DAS.Application.Enums;
using DAS.Application.Models.ViewModels;
using DAS.Domain.Models.Abstractions;
using DAS.Utility.CustomClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileIndexBorrowCart
    {
        public MobileBorrowCartCondition Condition { get; set; }
        public PaginatedList<VMCatalogingBorrowDocMobile> VMCatalogingBorrowDocs { get; set; }
        //public VMUpdateCatalogingBorrowMobile VMUpdateCatalogingBorrow { get; set; }
    }

    public class VMCatalogingBorrowDocMobile : Auditable
    {

        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public int IDOrgan { get; set; }

        public long IDFile { get; set; }

        public int IDProfile { get; set; } //ID bang ho so 

        public int IDDoc { get; set; } //ID tài liệu 
        public int IDDocType { get; set; } = 0; //IDLoai tai lieu (lấy theo doc)

        public int IDReader { get; set; }

        public int? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string StrStartDate { get; set; }

        public string StrEndDate { get; set; }

        public int Quantity { get; set; }

        public List<EnumToList> ListEnumBorrowTypes { get; set; }

        public int BorrowType { get; set; }

        public int Status { get; set; } = 1;

        public string DocCode { get; set; }


        #region Results column
        [NotMapped]
        public string ProfileCode { get; set; }
        [NotMapped]
        public string ProfileName { get; set; }
        [NotMapped]
        public VMDocType VMDocType { get; set; }
        [NotMapped]
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        [NotMapped]
        public List<VMCatalogingDocField> VMCatalogingDocFields { get; set; }
        [NotMapped]
        public List<EnumToList> dictCodeValue { get; set; }
        public string ApproveName { get; set; }
        public string ReaderName { get; set; }
        public int OrganID { get; set; }
        public string OrganName { get; set; }
        public int IDCatalogingBorrow { get; set; }
        public bool IsReturned { get; set; }
        public bool IsFreeze { get; set; }
        public string CodeElementProfile { get; set; }
        public string ElementProfile { get; set; }
        public int NumbericalOrder { get; set; }
        #endregion
    }

    public class VMUpdateCatalogingBorrowMobile : IValidatableObject
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;
        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public int IDReader { get; set; } = 0;

        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public int IDUser { get; set; } = 0;

        [Display(Name = "Tên độc giả", Prompt = "Tên độc giả")]
        public string Reader { get; set; }

        [Display(Name = "Hình thức khai thác", Prompt = "Hình thức khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? ReaderType { get; set; }

        [Display(Name = "Mục đích khai thác", Prompt = "Mục đích khai thác")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(500, ErrorMessage = "{0} không được vượt quá {1} ký tự")]
        public string Purpose { get; set; }

        public List<EnumToList> DictUser { get; set; }

        public List<EnumToList> DictReader { get; set; }

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

        public int[] IDs { get; set; } //Id phiếu

        public List<VMCatalogingBorrowDocMobile> CatalogingBorrowDocs { get; set; } //For detail, update, create

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
                yield return new ValidationResult("Số lượng không được để trống", new List<string> { "Quantity" });
            }
            if ((BorrowType == (int)EnumBorrow.BorrowType.Online || BorrowType == (int)EnumBorrow.BorrowType.Borrow))
            {
                if (string.IsNullOrEmpty(StrStartDate))
                {
                    yield return new ValidationResult("Khai thác từ ngày không được để trống", new List<string> { "StrStartDate" });
                }
                if (string.IsNullOrEmpty(StrEndDate))
                {
                    yield return new ValidationResult("Đến ngày không được để trống", new List<string> { "StrEndDate" });
                }
            }
        }
    }
}
