using System;

namespace ESD.Application.Models.ViewModels
{
    public class VMDownloadLink
    {
        public Guid ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public string DownloadHash { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public long? IDFile { get; set; }

        public int? IDFolder { get; set; }

        public long? CreatedBy { get; set; }

    }
}
