using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAS.CodeGenerator.Model
{
    public class TbColumn
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string DataType { get; set; }
        public bool Nullable { get; set; }
        public int? MaxLength { get; set; }
        public bool IsRefColumn
        {
            get
            {
                return Name != null && Name != "ID" && Name.ToUpper().IndexOf("ID") == 0;
            }
        }
        public string RefColumn
        {
            get
            {
                return IsRefColumn ? Name.Substring(2) : null; //Bỏ ID ở đầu
            }
        }

    }
}
