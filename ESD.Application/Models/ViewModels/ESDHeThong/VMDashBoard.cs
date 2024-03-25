using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.ViewModels
{
    public class VMDashBoardPlan
    {
        public int IDPlan { get; set; }
        public string PlanName { get; set; }
        public int TotalProfile { get; set; }
        public int TotalProfileApproved { get; set; }
    }

    public class VMDashBoardStorage
    {
        public string StorageDictStr { get; set; }
    }

    public class VMDashBoardProfile
    {
        public string ProfileDictStr { get; set; }
    }

    public class VMDashBoardExpiryDate
    {
        public string ExpiryDateDictStr { get; set; }
    }
}
