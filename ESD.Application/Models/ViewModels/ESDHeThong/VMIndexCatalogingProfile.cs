using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexCatalogingProfile
    {
        public Dictionary<int, string> DictOrgan { get; set; }
        public PaginatedList<VMCatalogingProfile> VMCatalogingProfiles { get; set; }
        public CatalogingProfileCondition Condition { get; set; }
        public VMPlan VMPlan { get; internal set; }
        public Dictionary<int, string> DictUser { get; set; }
        public VMPlanAgency VMPlanAgency { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        public Dictionary<int, string> DictAgencty { get; set; }
        public string AgenctyName { get; set; }
        public bool IsApprover { get; set; }
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public Dictionary<int, string> DictStatus { get; set; }
        public Dictionary<int, int> TotalDocs { get;  set; }
        public Dictionary<int, string> DictProfileType { get; set; }
    }

    public class CatalogingProfileCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; } 
        public int IDStorage { get; set; }
        public int IDShelve { get; set; }
        public int IDStatus { get; set; }
        public int IDBox { get; set; }
        public int? IsStoraged { get; set; }
        public int[] ExcludeIds { get; set; }

        public CatalogingProfileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}