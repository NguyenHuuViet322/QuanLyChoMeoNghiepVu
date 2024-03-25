using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ESD.Domain.Models.Abstractions;

namespace ESD.Application.Models.ViewModels
{
    public class VMTemplateParam : Auditable
    {
        public int ID { get; set; }
        public int IDTemplate { get; set; }
        public string Code { get; set; }
        [Description("Keyword for Template")]
        public string Name { get; set; }
        public int? Index { get; set; }
        public bool IsDelete { get; set; } = true;
        public bool IsUpdate { get; set; }
        public bool IsDetail { get; set; }
        public int IDOrgan { get; set; }
    }
}
