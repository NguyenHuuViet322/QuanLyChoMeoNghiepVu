using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMCatalogingDocField
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [Required]
        public int IDCatalogingDoc { get; set; }

        [Required]
        public int IDDocTypeField { get; set; }

        public string Value { get; set; }
        public int Status { get; set; } = 1;
        public bool IsReadonly { get;  set; }

        [NotMapped]
        public string Code { get;  set; } //Mã trường 
    }
}
