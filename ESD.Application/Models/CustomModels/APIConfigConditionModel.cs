using ESD.Application.Enums.DasKTNN;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{
    public class APIConfigConditionModel
    {
        public int IDColumn { get; set; }
        public int Operator { get; set; }
        public string Value { get; set; }
    }
}