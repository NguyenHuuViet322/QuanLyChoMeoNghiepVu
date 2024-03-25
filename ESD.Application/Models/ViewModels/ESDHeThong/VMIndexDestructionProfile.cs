using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexDestructionProfile
    {
        public PaginatedList<VMDestructionProfile> VMDestructionProfiles { get; set; }
        public DestructionProfileCondition Condition { get; set; }
        public Dictionary<int, string> DictUser { get; set; }
    }
    public class DestructionProfileCondition
    {
        public DestructionProfileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int IDStatus { get; set; } = -1;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
