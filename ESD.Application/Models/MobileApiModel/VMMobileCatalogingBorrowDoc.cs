using System;
using System.Collections.Generic;
using System.Text;
using DAS.Domain.Models.Abstractions;
using DAS.Utility.CustomClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileCatalogingBorrowDoc : Auditable
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
        public string ApproveName { get; set; }
        public string ReaderName { get; set; }
        public int OrganID { get; set; }
        public string OrganName { get; set; }
        public int IDCatalogingBorrow { get; set; }
        public bool IsReturned { get; set; }
        public bool IsFreeze { get; set; }
        #endregion
    }
}
