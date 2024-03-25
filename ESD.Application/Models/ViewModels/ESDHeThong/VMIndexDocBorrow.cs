using ESD.Domain.Models.DAS;
using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexDocBorrow
    {
        public DocBorrowCondition Condition { get; set; }
        public PaginatedList<VMCatalogingDoc> VMCatalogingDocs { get; set; }
        public PaginatedList<VMCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; }
        public List<int> BorrowingDocIds { get; set; }
        public VMCatalogingProfile VMCatalogingProfile { get; set; }
        public Dictionary<int, string> DictStatus { get; set; }
        public Dictionary<int, string> DictReader { get; set; }
        public PaginatedList<VMCatalogingBorrow> VMCatalogingBorrows { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        public Dictionary<int, string> DictProfileCategory { get; set; }
        public Dictionary<int, string> DictLanguage { get; set; }
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public Dictionary<int, string> DictAgencies { get; set; }
        public Dictionary<int, string> DictUser { get; set; }
        public Dictionary<int, string> DictBorrowType { get; set; }
    }

    public class DocBorrowCondition
    {
        public DocBorrowCondition()
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
        public int IDCatalogingBorrow { get; set; }
        public int BorrowType { get; set; }
        public int IsAdvSearch { get; set; }
    }
}