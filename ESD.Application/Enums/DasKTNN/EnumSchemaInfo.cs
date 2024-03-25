using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums.DasKTNN
{
    public static class EnumSchemaInfo
    {
        /// <summary>
        /// Kiểm schema > Kiểu bảng 
        /// </summary>
        public enum SchemaType
        {
            [Description("Tự định nghĩa")]
            Dynamic =0 ,
            [Description("Danh mục")]
            Category = 1,
        }
    }
    public static class EnumSoucreInfo
    {
        public enum TypeSoucre
        {
            [Description("CSDL SQL Server")]
            SQLServer = 1,
            [Description("CSDL Oracle")]
            Oracle = 2,
            [Description("CSDL MySQL")]
            MySQL = 3,
            [Description("CSDL Cassanda")]
            Cassanda = 4,
            [Description("API")]
            API = 5,
            [Description("File XML")]
            FileXML = 6,
            [Description("File CSV")]
            FileCSV = 7,
        }
        public enum TypeStatus
        {
            [Description("Hoạt động")]
            HoatDong = 1,
            [Description("Dừng hoạt động")]
            DungHoatDong = 2,
        }
    }
}