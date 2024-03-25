using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexAdvanceSearch
    {
        public AdvanceSearchCondition Condition { get; set; }
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
        public List<VMDocTypeField> SearchFields { get; set; }
        public List<VMDocType> VMDocTypes { get;  set; }
    }

    public class AdvanceSearchCondition
    {
        public AdvanceSearchCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int IsSearch { get; set; }
        public int IsAdvSearch { get; set; }
        public int ReaderType { get; set; }
        public int IDDocType { get; set; }
        public int TypeOfDocType { get; set; }
        public AdvanceSearchConditions[] Conditions { get; set; }
    }

    public class AdvanceSearchConditions 
    {
        public int IDField { get; set; }
        public string Keyword { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
    }
}