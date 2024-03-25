using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMReportSendArchive
    {
        //Doc
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
        //Profile
        public string Profile_Title { get; set; }
        public string Profile_FileCode { get; set; }
        //Plan
        public string Plan_Name { get; set; }
    }
}
