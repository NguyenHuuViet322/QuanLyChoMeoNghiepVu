using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESD.Application.Models.ImportModel
{
    public class VMIndexImportInputInfo
    {

        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(29 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".xlsx" })]
        public IFormFile File { get; set; }
            [Display(Name = "IDTable")]
        public long? IDTable { get; set; }
    }
}
