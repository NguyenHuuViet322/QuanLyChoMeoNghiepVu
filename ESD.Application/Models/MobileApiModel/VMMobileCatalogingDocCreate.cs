using DAS.Application.Models.ViewModels;
using DAS.Domain.Models.Abstractions;
using DAS.Domain.Models.DAS;
using DAS.Utility.CustomClass;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DAS.Application.Models.MobileApiModel
{
    public class VMMobileCatalogingDocCreate : Auditable
    {
        public int ID { get; set; }

        public long IDFile { get; set; }

        public int IDCatalogingProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public CatalogingProfile VMCatalogingProfile { get; set; }
        
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
  
        public List<VMCatalogingDocField> VMCatalogingDocFields { get; set; }
        public VMStgFile VMStgFile { get; set; }
        public List<EnumToList> ListDocFieldValue { get; set; }
    
        public IEnumerable<VMMobileCatalogingBorrowDoc> CatalogingBorrowDocs { get; set; } //Ds dang cho muon
        public string LanguageName { get; set; }
        public bool IsBorrowed { get; set; }
    }

    public class VMMobileDetailCatalogingDoc
    {
        public int ID { get; set; }

        public long IDFile { get; set; }

        public int IDCatalogingProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public VMMobileCatalogingProfile VMCatalogingProfile { get; set; }
        public IEnumerable<VMMobileCatalogingBorrowDoc> WaitingCatalogingBorrowByReader { get; set; } //Ds dang cho muon
        public string LanguageName { get; set; }
        public bool IsBorrowed { get; set; }
        public List<EnumToList> ListDocFieldValue { get; set; }
        public FileBinaryInfo VMStgFile { get; set; }
        public int TotalFile { get; set; }
    }
}
