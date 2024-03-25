using System.Collections;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels
{
    public class VMIndexCategory
    {
        public CategoryCondition CategoryCondition { get; set; }
        public List<VMCategoryTypeField> VMCategoryTypeFields { get; set; }
        public PaginatedList<VMCategory> VMCategorys { get; set; } 
        public VMCategoryType VMCategoryType { get; set; }
        public Hashtable DataSearch { get; set; }
        public List<VMCategoryField> VMCategoryFields { get;  set; }
        public List<VMCategoryField> VMCategoryFieldSearchs { get;  set; }
        public IEnumerable<VMCategoryType> VMCategoryTypes { get;  set; }
    }

    public class CategoryCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Name { get; set; }

        public int Parent { get; set; } //ID category cha: cùng cấp

        public int IDParentCategory { get; set; } //ID category cha: cấp trên

        public string CodeType { get; set; }

        public CategoryCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        } 
    }
}