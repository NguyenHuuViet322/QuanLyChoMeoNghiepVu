using Newtonsoft.Json;
using System.Collections.Generic;

namespace ESD.Application.Models.ViewModels
{
    public class VMUserBookMark
    {
        public int ID { get; set; }
        public int IDUser { get; set; }
        public string BookMark { get; set; }
        public List<int> Modules
        {
            get
            {
                if (string.IsNullOrEmpty(BookMark))
                {
                    return new List<int>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<List<int>>(BookMark);
                }
            }
            set { Modules = value; }
        }
    }
}
