using System.Collections.Generic;

namespace DASNotify.Application.Models.CustomModels
{
    public class ExportExtend
    {
        public List<dynamic> Data { get; set; }
        public List<Col> Cols { get; set; }
        public List<Header> Headers { get; set; }
    }

    public class Col
    {
        public bool isWrapText { get; set; } = true;
        public string Field { get; set; }
        public int DataType { get; set; } = 0;//Text = 0,Number = 1,Boolean/Enum = 2,DateTime = 3,TimeSpan = 4, Index = 5
        public string BackGround { get; set; }// Color Name
        public string Color { get; set; }
        public string Font { get; set; } = "Times New Roman";
        public int Size { get; set; } = 11;
        public bool IsBold { get; set; } = false;
        public bool IsItalic { get; set; } = false;
        public bool IsBorder { get; set; } = true;
        public Dictionary<int, string> DefineEnum { get; set; }
        public Col(string field)
        {
            Field = field;
        }
        public Col(string field, Dictionary<int, string> defineEnum)
        {
            Field = field;
            DefineEnum = defineEnum;
            DataType = 2; // Check khi DataType là define trạng thái
        }
        public Col() { }
    }

    public class Header
    {
        public string Name { get; set; }
        public string BackGround { get; set; } = "LightBlue";
        public string Color { get; set; }
        public string Font { get; set; } = "Times New Roman";
        public bool IsBold { get; set; } = true;
        public bool IsItalic { get; set; } = false;
        public int Size { get; set; } = 12;
        public bool IsBorder { get; set; } = true;
        public int Width { get; set; } = 25;
        public Header(string name)
        {
            Name = name;
        }
        public Header(string name, int width)
        {
            Name = name;
            Width = width;
        }
        public Header() { }
    }
}
