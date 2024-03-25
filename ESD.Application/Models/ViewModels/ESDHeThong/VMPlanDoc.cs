using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMPlanDoc : Auditable
    {
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        [Required(ErrorMessage = "Không xác định được User")]
        public int IDProfile { get; set; } //ID bang ho so 
        public int IDDocType { get; set; } = 0; //IDLoai tai lieu
        public int Status { get; set; } = 1;
        public List<VMDocTypeField> VMDocTypeFields { get; set; }
        public VMDocType VMDocType { get; set; }
        public List<VMDocField> VMDocFields { get; set; }
        public Dictionary<string, string> dictCodeValue { get; set; }

        #region Custom field
        public string FileCode { get; set; }
        public string Title { get; set; }
        public int IDStorage { get; set; }
        public string StorageName { get; set; }
        public int IDShelve { get; set; }
        public string ShelveName { get; set; }
        public int IDBox { get; set; }
        public string BoxName { get; set; }
        public int IDProfileTemplate { get; set; }
        public string ProfileTemplateName { get; set; }
        public Dictionary<int, string> DictProfileTemplate { get; set; }
        #endregion Custom field
    }
}
