using ESD.Application.Enums.DasKTNN;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{

    public class InputUploadModel
    {
        public string Title { get; set; } = "Đính kèm file";
        public string Name { get; set; } = "File";
        public string BtnClass { get; set; } = "btn btn-info";
        public string Accept { get; set; }
        public bool Multiple { get; set; } = true;
        public bool ReadOnly { get; set; } 
        public IEnumerable<InputUploadFileModel> Files { get; set; } = new List<InputUploadFileModel>();
    }
    public class InputUploadFileModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool AllowDelete { get; set; } = true;
    }
}