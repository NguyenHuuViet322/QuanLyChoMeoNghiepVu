using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexPlan
    {
        public Dictionary<int, string> DictStatus { get; set; }
        public PaginatedList<VMPlan> VMPlans { get; set; }
        public List<VMPlanProfile> VMProfiles { get; set; }
        public VMCreatePlan VMCreatePlan { get; set; }
        public PlanCondition PlanCondition { get; set; }
        public Dictionary<int, string> DictUser { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        public Dictionary<int, string> DictPlans { get;  set; }
        public Dictionary<int, string> DictAgencies { get;  set; }
        public bool IsApprover { get; set; }
        public Dictionary<int, int> TotalProfiles { get; set; }
        public Dictionary<int, ProfileCount> CountProfiles { get; set; }
    }

    public class PlanCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int IDStatus { get; set; } = -1;
        public int IDPlan { get; set; } = -1;
        public int IDAgeny { get; set; } = -1;
        public bool IsReceived { get; set; }
        public PlanCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }

    public class ProfileCount
    {
        public int ID { get; set; }
        public int TotalCount { get; set; }
        public bool HasReject { get; set; } = false;
    }
}