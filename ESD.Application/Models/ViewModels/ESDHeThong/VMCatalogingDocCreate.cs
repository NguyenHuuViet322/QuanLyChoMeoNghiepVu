using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMCatalogingDocCreate : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        [Required(ErrorMessage = "Không xác định được User")]
        public int IDCatalogingProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public VMCatalogingProfile VMCatalogingProfile { get; set; }
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public VMDocType VMDocType { get; set; }
        public List<VMCatalogingDocField> VMCatalogingDocFields { get; set; }
        public IEnumerable<VMDocType> VMDocTypes { get; set; }
        public VMStgFile VMStgFile { get; set; }
        public Dictionary<string, string> DocFieldValues { get; set; }
        public string LanguageName { get; set; }
        public IEnumerable<VMCatalogingBorrow> CatalogingBorrows { get; set; }
        public IEnumerable<VMCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; } //Ds dang cho muon
        public IEnumerable<VMCatalogingBorrowDoc> CatalogingBorrowedDocs { get; set; } //Ds da dc muon
        public IEnumerable<VMCatalogingBorrow> CatalogingBorroweds { get; set; }
        public List<int> BorrowingDocIds { get; set; }
    }
}
