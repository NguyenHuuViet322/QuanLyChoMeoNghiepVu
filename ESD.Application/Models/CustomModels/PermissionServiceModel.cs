using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{
    public class UserPermissionModel
    {
        public int IdPermission { get; set; }
        public int IdModule { get; set; }
        public int CodeModule { get; set; }
        public int Type { get; set; }
    }
    
    //public class UserPermissionModel
    //{
    //    public int IdPermission { get; set; }
    //    public int IdModule { get; set; }
    //    public string CodeModule { get; set; }
    //    public int Type { get; set; }
    //}

}
