using ESD.Domain.Models.DAS;
using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexBorrowCart
    {
        public BorrowCartCondition Condition { get; set; }
        public PaginatedList<VMCatalogingDoc> VMCatalogingDocs { get; set; }
        public PaginatedList<VMCatalogingBorrowDoc> VMCatalogingBorrowDocs { get; set; }
        public VMCatalogingProfile VMCatalogingProfile { get; internal set; }
        public Dictionary<int, string> DictStatus { get; set; }
        public Dictionary<int, string> DictReader { get; set; }
        public Dictionary<int, string> DictUser { get; set; }
        public VMUpdateCatalogingBorrow VMUpdateCatalogingBorrow { get; set; }
    }

    public class BorrowCartCondition
    {
        public BorrowCartCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string FromCreateDate { get; set; }
        public string ToCreateDate { get; set; }
        public int IDProfile { get; set; }
        public int IsSearch { get; set; }
        public int? Status { get; set; }
        public int IDReader { get; set; }  
    }
}