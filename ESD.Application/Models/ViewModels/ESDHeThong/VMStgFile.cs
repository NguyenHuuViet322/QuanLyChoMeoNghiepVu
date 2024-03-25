using ESD.Domain.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using ESD.Utility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ESD.Application.Models.ViewModels
{
    public class VMStgFile : Auditable
    {
        public long ID { get; set; }

        public int IDChannel { get; set; } = 0;

        [MaxLength(250)]
        public string FileName { get; set; }

        [MaxLength(1000)]
        public string PhysicalPath { get; set; }

        public int FileType { get; set; } // Avatar, văn bản, âm thanh, hình ảnh, video,...

        public decimal Size { get; set; }

        public bool IsTemp { get; set; } = false;

        public bool? IsEncrypted { get; set; }

        public int Status { get; set; } = 1;

        [NotMapped]
        public IFormFile File { get; set; }

        [NotMapped]
        public string Message { get; set; }
        public string CreateDateStr { get; set; }
        public int SignType { get; set; } = 0;
        public long? IDOldFile { get; set; }
        public bool IsSign { get; set; } = false;
        public int IDSigner { get; set; }
        public string FileExtension { get; set; }
    }

    public class FileBinaryInfo
    {
        public byte[] FileContents { get; set; }

        public string FileName { get; set; }

        public long? IDFile { get; set; }

        public string PhysicalFilePath { get; set; }

        public string MimeType => FileUltils.GetMimeType(Path.GetExtension(this.FileName));
    }

    public class StgFileCondition
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string CreateDateStr { get; set; }
        public StgFileCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
