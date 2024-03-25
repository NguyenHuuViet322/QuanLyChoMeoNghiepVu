using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Models.ViewModels
{
    public class VMTemplate
    {
        public int ID { get; set; }
        public long? IDStgFile { get; set; }
        [Required(ErrorMessage = "Mã mẫu biên bản không được để trống")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; } = 1;
        public IFormFile File { get; set; }
        public IEnumerable<VMTemplateParam> TemplateParam { get; set; }
        public bool IsDelete { get; set; }
        public bool IsUpdate { get; set; }
        public int IDOrgan { get; set; }
    }

    public class TemplateCondition
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public TemplateCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
