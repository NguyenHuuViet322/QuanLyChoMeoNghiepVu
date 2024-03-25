using ESD.Domain.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESD.Application.Models.ViewModels
{
    public class VMReportReceiveArchive
    {
        public int IDPlan { get; set; }
        public int IDAgency { get; set; }
        public string PlanName { get; set; }
        public string AgencyName { get; set; }
        public int CountProfile { get; set; }
        public int CountDoc { get; set; }

    }
}
