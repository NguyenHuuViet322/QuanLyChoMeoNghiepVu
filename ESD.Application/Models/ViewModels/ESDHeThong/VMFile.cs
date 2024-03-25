using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMFile
    {
        [NotMapped]
        public IFormFile File { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; }

        public int FileType { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        public decimal Size { get; set; }
    }
}
