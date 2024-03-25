
namespace DASNotify.Application.Models.ViewModels
{
    public class VMSendNotificationPortal
    {
        public int[] idsUser { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public int IDImpactUser { get; set; } = 0;
        public int IDAffectedObject { get; set; } = 0;
        public int AffectedObjectType { get; set; } = 0;
        public int IDImpactAgency { get; set; } = 0;
        public int IDImpactOrgan { get; set; } = 0;
    }
}
