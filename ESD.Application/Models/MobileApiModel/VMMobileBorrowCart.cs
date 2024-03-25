using System;
using System.Collections.Generic;
using System.Text;
using DAS.Application.Models.ViewModels;

namespace DAS.Application.Models.MobileApiModel
{
    public class MobileBorrowCartCondition
    {
        public MobileBorrowCartCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public int IdProfile { get; set; }
        public List<int> IdDocs { get; set; } = new List<int>();
    }
}
