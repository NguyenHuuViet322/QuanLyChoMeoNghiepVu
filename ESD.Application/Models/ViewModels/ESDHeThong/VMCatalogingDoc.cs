using AutoMapper;
using ESD.Domain.Models.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Application.Models.ViewModels
{
    public class VMCatalogingDoc : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }
        public long IDDoc { get; set; }
        public int IDCatalogingProfile { get; set; } //ID bang ho so 
        public int IDDocType { get; set; } = 0; //IDLoai tai lieu
        public int Status { get; set; } = 1;


        public bool IsPublic { get; set; } //Tài liệu public?


        #region Result Column
        //[IgnoreMap]
        public string ProfileName { get; set; }

        //[IgnoreMap]
        public string ProfileCode { get;  set; }

        [IgnoreMap]
        public List<VMDocTypeField> VMDocTypeFields { get; set; }

        [IgnoreMap]
        public VMDocType VMDocType { get; set; }

        [IgnoreMap]
        public List<VMDocField> VMDocFields { get; set; }
     
        [IgnoreMap]
        public Dictionary<string, string> dictCodeValue { get; set; }
       
        [IgnoreMap]
        public List<VMCatalogingDocField> VMCatalogingDocFields { get; set; }

        [IgnoreMap]
        public IFormFile File { get; set; }
        #endregion Result Column

    }

    public class VMCatalogingDocRs 
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }
        public long IDDoc { get; set; } 
        public int IDCatalogingProfile { get; set; } //ID bang ho so 
        public int IDDocType { get; set; } = 0; //IDLoai tai lieu
        public int Status { get; set; } = 1;


        public bool IsPublic { get; set; } //Tài liệu public?


        
        public string ProfileName { get; set; }

        
        public string ProfileCode { get; internal set; }
 

    }
}
