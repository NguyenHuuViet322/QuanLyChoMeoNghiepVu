using ESD.Domain.Models.DAS;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMSearchProfileAndDoc
    {
        public int ID { get; set; }

        public string FileCode { get; set; }

        public string Title { get; set; }

        public string TypeDocName { get; set; }

        public string Subject { get; set; }
    }

    public class VMIndexProfileAndDoc
    {
        public PaginatedList<VMPlanDoc> VMPlanDoc { get; set; }
        public Dictionary<int, int> TotalDocs { get; set; }
        public List<VMDocType> VMDocTypes { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
        public SearchProfileCondition Condition { get; set; }
    }


    public class SearchProfileCondition
    {
        public SearchProfileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int IDProfile { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int IDStorage { get; set; }
        public int IDShelve { get; set; }
        public int IDProfileTemplate { get; set; }
        public int IDStatus { get; set; }
        public int IDBox { get; set; }
    }
}
