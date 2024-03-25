using ESD.Domain.Models.Abstractions;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMDeliveryRecord
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Số biên bản không được để trống")]
        [MaxLength(20, ErrorMessage = "Số biên bản không được vượt quá 20 ký tự")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Tiêu đề biên bản không được để trống")]
        [MaxLength(250, ErrorMessage = "Tiêu đề biên bản không được vượt quá 250 ký tự")]
        public string Title { get; set; }
        public DateTime RecordCreateDate { get; set; }
        [Required(ErrorMessage = "Ngày tạo biên bản không được để trống")]
        public string RecordCreateDateStr { get; set; }
        [Required(ErrorMessage = "Kế hoạch không được để trống")]
        public int IDPlan { get; set; }
        public string PlanName { get; set; }
        [Required(ErrorMessage = "Đơn vị không được để trống")]
        public int IDAgency { get; set; }
        public string AgencyName { get; set; }
        public string OrganName { get; set; }
        #region Sender
        [Required(ErrorMessage = "Người giao không được để trống")]
        public int IDSendUser { get; set; }
        public string SenderPosition { get; set; }
        public string AccountSendUser { get; set; }
        public string NameSendUser { get; set; }
        #endregion
        #region Receiver
        [Required(ErrorMessage = "Người nhận không được để trống")]
        public int IDReceiveUser { get; set; }
        public string ReceiverPosition { get; set; }
        public string AccountReceiveUser { get; set; }
        public string NameReceiveUser { get; set; }
        #endregion
        #region Document
        public string DocumentName { get; set; }
        public string DocumentTime { get; set; }
        public string DocumentReceiveStatus { get; set; }
        public string Department { get; set; }
        public int TotalDocument { get; set; }
        public int TotalDocumentFile { get; set; }
        #endregion
        public int Status { get; set; }
        public string Reason { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public int IDTemplate { get; set; }
        public Dictionary<int, int> TotalProfiles { get; set; }
        public string lstDeliveryPlanProfile { get; set; }
        public PaginatedList<VMPlanProfile> VMPlanProfiles { get; set; }
        public bool IsQuickDetail { get; set; } = false;
        public List<string> IDPlanProfileStrs { get; set; }
        public DeliveryRecordCondition condition { get; set; }
    }

    public class DeliveryRecordCondition
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int IDAgency { get; set; }
        public string Agency { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string DeliveryPlanProfile { get; set; }
        public DeliveryRecordCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        public List<string> lstAgency
        {
            get
            {
                if (Agency.IsNotEmpty())
                    return Agency.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }

        public List<string> lstDeliveryPlanProfile
        {
            get
            {
                if (DeliveryPlanProfile.IsNotEmpty())
                    return DeliveryPlanProfile.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
    }
}
