using ESD.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexProfile
    {
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        public PaginatedList<VMProfile> VMProfiles { get; set; } 
        public ProfileCondition ProfileCondition { get; set; } 
    }

    public class ProfileCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string Title { get; set; }
        public string IDProfileTemplates { get; set; } //Phong
        public int IDAgency { get; set; }
        public List<string> ArrIDProfileTemplates {

            get
            {
                if (IDProfileTemplates.IsNotEmpty())
                    return IDProfileTemplates.Split(",").ToList();
                return new List<string>();
            }
            set { }
        } //Phong

        public ProfileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}