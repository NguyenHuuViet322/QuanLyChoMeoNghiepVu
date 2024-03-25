using DAS.Application.Models.ViewModels;
using System.Text.Json.Serialization;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileAdvanceSearch
    {
        public MobileAdvanceSearchCondition Condition { get; set; }
        public PaginatedList<VMMobileCatalogingDoc> VmCatalogingDocs { get; set; }
    }

    public class MobileAdvanceSearchCondition
    {
        public MobileAdvanceSearchCondition()
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
        [JsonIgnore]
        public int IdDocType { get; set; }
        public int TypeOfDocType { get; set; }
        public MobileAdvanceSearchConditions[] AdvanceSearchConditions { get; set; }
    }

    public class MobileAdvanceSearchConditions
    {
        public int IdField { get; set; }
        public string Keyword { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
    }
}
