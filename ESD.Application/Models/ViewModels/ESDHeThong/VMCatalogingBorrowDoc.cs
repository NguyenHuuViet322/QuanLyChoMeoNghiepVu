using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMCatalogingBorrowDoc : Auditable
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


        public int Status { get; set; } = 1;


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
        public Dictionary<string, string> dictCodeValue { get; set; }
        public string ApproveName { get; set; }
        public string ReaderName { get; set; }
        public int OrganID { get; set; }
        public string OrganName { get; set; }
        public int IDCatalogingBorrow { get; set; }
        public bool IsReturned { get; set; }
        #endregion
    }
}
