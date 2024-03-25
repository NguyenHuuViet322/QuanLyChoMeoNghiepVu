using ESD.Application.Enums.DasKTNN;
using ESD.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{
    public class DynamicAPIModel
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
    }
}