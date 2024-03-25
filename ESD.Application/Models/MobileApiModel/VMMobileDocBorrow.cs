using System.Text.Json.Serialization;
using DAS.Application.Models.ViewModels;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileDocBorrow
    {
        public MobileDocBorrowCondition Condition { get; set; }
        public int? TotalRecordsCatalogingBorrow { get; set; }
        public PaginatedList<VMCatalogingBorrow> VmCatalogingBorrows { get; set; }
        public bool IsFullProcess { get; set; }
    }

    public class MobileDocBorrowCondition
    {
        public MobileDocBorrowCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        [JsonIgnore]
        public string DocCode { get; set; }
        public string FromCreateDate { get; set; }
        public string ToCreateDate { get; set; }
        [JsonIgnore]
        public int IdProfile { get; set; }
        public int IsSearch { get; set; }
        public int? Status { get; set; }
        public int IdReader { get; set; }
        [JsonIgnore]
        public int IdCatalogingBorrow { get; set; }
        [JsonIgnore]
        public int ReaderType { get; set; }
        [JsonIgnore]
        public int BorrowType { get; set; } = -1;
    }
}
