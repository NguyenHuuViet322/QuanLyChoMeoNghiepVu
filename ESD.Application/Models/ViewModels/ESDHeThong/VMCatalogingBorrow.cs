using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMCatalogingBorrow : Auditable
    {

        public int ID { get; set; }

        public string Code { get; internal set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        public int IDReader { get; set; }
        public string Purpose { get; set; }
        public string ReasonToReject { get; set; }

        public int? ApproveBy { get; set; }
        public DateTime? ApproveDate { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public int Status { get; set; } = 1;

        #region Results column
        public string OrganName { get; set; }
        public string ReaderName { get; set; }
        public string BorrowType { get; set; }
        public IEnumerable<VMCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; }
        public int ReaderType { get; set; }
        public bool IsReturned { get;  set; }
        public DateTime? ReturnDate { get;  set; }
        public bool IsOriginal { get;  set; }
        public int[] IDs { get; internal set; }
        public string Url { get; set; }

        #endregion
    }
}
