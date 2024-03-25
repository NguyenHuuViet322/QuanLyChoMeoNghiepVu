using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexProfileWaitDestruction
    {
        public PaginatedList<VMPlanProfile> VMPlanProfiles { get; set; }
        public ProfileWaitDestructionCondition Condition { get; set; }

        //Dict
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public Dictionary<int, string> DictProfileType { get; set; }
        public Dictionary<int, string> DictAgency { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        //kho, giá hộp cặp
        public Dictionary<int, string> DictStorage { get; set; }
        public Dictionary<int, string> DictSheleve { get; set; }
        public Dictionary<int, string> DictBox { get; set; }

    }

    public class ProfileWaitDestructionCondition
    {
        public ProfileWaitDestructionCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int IDStatus { get; set; } = -1;
        public int IDExpiryDate { get; set; } = -1;
        public int IDStorage { get; set; } = -1;
        public int IDBox { get; set; } = -1;
        public int IDShelve { get; set; } = -1;
        public int Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
