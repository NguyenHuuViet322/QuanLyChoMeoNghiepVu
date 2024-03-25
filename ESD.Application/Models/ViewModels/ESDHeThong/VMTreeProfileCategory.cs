using ESD.Domain.Models.DAS;
using ESD.Utility;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels
{
    public class VMTreeProfileCategory
    {

        public List<VMNodeProfileCategory> Nodes { get; set; }
    }
    public class VMNodeProfileCategory
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}