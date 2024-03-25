using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Utility.CustomClass
{
    public class Pagination
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }
    }
}
