using ESD.Utility;
using System.Collections.Generic;
using System.Linq;


namespace ESD.Application.Models.ViewModels
{
    public class VMIndexCatalogingDoc
    {
        internal PaginatedList<VMCatalogingDoc> vMPlanCatalogingDocs;

        public PlanDocCondition PlanDocCondition { get; set; }
        //Hồ sơ
        public VMCatalogingProfile VMCatalogingProfile { get;  set; }
        public Dictionary<int, string> DictExpiryDate { get; set; }
        //Tài liệu
        public List<VMDocType> VMDocTypes { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public CatalogingDocCondition CatalogingDocCondition { get;  set; }
        public PaginatedList<VMCatalogingDoc> VMCatalogingDocs { get;  set; }
        public bool IsQuickDetail { get; set; }
        public Dictionary<int, string> DictLanguage { get;  set; }
        public Dictionary<int, string> DictProfileTemplate { get;  set; }
        public Dictionary<int, string> DictProfileCategory { get;  set; }
        public Dictionary<int, string> DictAgencies { get;  set; }
        public Dictionary<int, string> DictUsers { get;  set; }

    }

    public class CatalogingDocCondition
    {
        public int IDAgency { get; set; }
        public int IDProfile { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
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
        public CatalogingDocCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
