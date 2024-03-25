using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VmLargeFile
    {
        public string Message { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long Uploaded { get; set; }

        public long ContentLength { get; set; }

        public bool Flag { get; set; }
    }
}
