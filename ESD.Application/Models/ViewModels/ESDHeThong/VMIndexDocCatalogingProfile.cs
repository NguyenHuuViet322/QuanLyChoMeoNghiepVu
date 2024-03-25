using ESD.Utility;
using System.Collections.Generic;
using System.Linq;


namespace ESD.Application.Models.ViewModels
{
    public class VMIndexDocCatalogingProfile
    {
        public SearchProfileCondition SearchProfileCondition { get; set; }
        //Hồ sơ
        public VMCatalogingProfile VMCatalogingProfile { get;  set; }
        public VMUpdateCatalogingProfile VMUpdateCatalogingProfile { get; set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        //Tài liệu
        public PaginatedList<VMPlanDoc> vMPlanDocs { get; set; }
        public List<VMDocType> VMDocTypes { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
    }   
}
