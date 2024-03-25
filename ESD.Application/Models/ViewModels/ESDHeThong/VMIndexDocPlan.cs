using ESD.Utility;
using System.Collections.Generic;
using System.Linq;


namespace ESD.Application.Models.ViewModels
{
    public class VMIndexDocPlan
    {
        public PlanDocCondition PlanDocCondition { get; set; }
        //Hồ sơ
        public VMPlanProfile VMPlanProfile { get; set; }
        public VMUpdatePlanProfile VMUpdatePlanProfile { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        //Tài liệu
        public PaginatedList<VMPlanDoc> vMPlanDocs { get; set; }
        public List<VMDocType> VMDocTypes { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
        public bool NotQuickDetail { get; set; } = true;
        public bool IsQuickDetail { get; set; } = false;
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public Dictionary<int, string> DictProfileCategory { get; set; }
        public Dictionary<int, string> DictLanguage { get; set; }
        public Dictionary<int, string> DictAgencies { get; set; }
        public Dictionary<int, string> DictUsers { get; set; }
    }

    public class PlanDocCondition
    {
        public int IDAgency { get; set; }
        public int IDProfile { get; set; }
        public int IDStatus { get; set; } = -1;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int IDDeliveryRecord { get; set; }
        public string Keyword { get; set; }
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
        public string Statuses { get; set; }
        public PlanDocCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
