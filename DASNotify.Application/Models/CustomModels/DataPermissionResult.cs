using DASNotify.Application.Constants;

namespace DASNotify.Application.Models.CustomModels
{
    public class DataPermissionResult
    {
        public bool HavePermission { get; set; }
        public string RedirectUrl { get; set; }
    }
}