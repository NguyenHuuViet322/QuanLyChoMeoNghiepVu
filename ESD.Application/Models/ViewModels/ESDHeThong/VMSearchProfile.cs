using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMSearchProfileDoc : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        [Required(ErrorMessage = "Không xác định được User")]
        public int IDProfile { get; set; } //ID bang ho so 
        public int IDCatalogingProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public VMCatalogingProfile VMCatalogingProfile { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public VMDocType VMDocType { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
        public IEnumerable<VMDocType> VMDocTypes { get; set; }
        public VMStgFile VMStgFile { get; set; }
    }
}
