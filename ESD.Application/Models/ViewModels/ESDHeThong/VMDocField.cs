using System;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMDocField
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDDoc { get; set; }

        public int IDCatalogingDoc { get; set; }

        [Required]
        public int IDDocTypeField { get; set; }

        public string Value { get; set; }
        public int Status { get; set; } = 1;
    }
}
