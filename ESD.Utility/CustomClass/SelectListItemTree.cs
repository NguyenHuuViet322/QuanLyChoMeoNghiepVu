using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Utility.CustomClass
{
    public class SelectListItemTree: SelectListItem
    {
        public int? Level { get; set; } = 0;
    }
}
