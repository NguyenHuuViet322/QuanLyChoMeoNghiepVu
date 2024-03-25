using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexPlanProfile
    {
        public Dictionary<int, string> DictOrgan { get; set; }
        public PaginatedList<VMPlanProfile> VMPlanProfiles { get; set; }
        public List<VMPlanProfile> VMListPlanProfiles { get; set; }
        public PlanProfileCondition PlanProfileCondition { get; set; }
        public VMPlan VMPlan { get; internal set; }
        public Dictionary<int, string> DictUser { get; set; }
        public VMPlanAgency VMPlanAgency { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        public Dictionary<int, string> DictAgency { get; set; }
        public string AgenctyName { get; set; }
        public bool IsApprover { get; set; }
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public Dictionary<int, string> DictStatus { get; set; }
        public Dictionary<int, int> TotalDocs { get; set; }
        public bool NotQuickDetail { get; set; } = true;
        public bool IsQuickDetail { get; set; } = false;
        public Dictionary<int, string> DictProfileType { get; set; }
    }

    public class PlanProfileCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string Title { get; set; }
        public int IDPlan { get; set; }
        public int IDAgency { get; set; }
        public int IDStatus { get; set; } = -1;
        public string Statuses { get; set; }
        public byte IsReceived { get; set; }
        public List<string> ListStatusStr
        {
            get
            {
                if (Statuses.IsNotEmpty())
                    return Statuses.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public PlanProfileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}