using ESD.Domain.Models.Abstractions;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESD.Application.Models.ViewModels
{
    public class VMApproveStorage
    {
        public int ID { get; set; }
        public string FileNotation { get; set; }
        public string Title { get; set; }
        public string DocumentTime { get; set; }
        public string NumberPaperAndPage { get; set; }
        public string ProfileNote { get; set; }
        public int IDExpiryDate { get; set; }
        public int IDSecurityLevel { get; set; }
        public int Status { get; set; }
        public string ExpiryDate { get; set; }
        public string SecurityLevel { get; set; }
        public DateTime? DocumentStartDate { get; set; }
        public DateTime? DocumentEndDate { get; set; }
        public Dictionary<int, int> TotalProfiles { get; set; }
    }

    public class ApproveStorageCondition
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int IDExpiryDate { get; set; }
        public int IDSecurityLevel { get; set; }
        public string DocumentName { get; set; }
        public string ExpiryDate { get; set; }
        public string SecurityLevel { get; set; }
        public ApproveStorageCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        public List<string> lstDocumentName
        {
            get
            {
                if (DocumentName.IsNotEmpty())
                    return DocumentName.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> lstExpiryDate
        {
            get
            {
                if (ExpiryDate.IsNotEmpty())
                    return ExpiryDate.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
        public List<string> lstSecurityLevel
        {
            get
            {
                if (SecurityLevel.IsNotEmpty())
                    return SecurityLevel.Split(",").ToList();
                return new List<string>();
            }
            set { }
        }
    }
}
