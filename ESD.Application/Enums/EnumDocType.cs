using System.ComponentModel;

namespace ESD.Application.Enums
{
    public class EnumDocType
    {
        public enum Status
        {
            [Description("Không hiệu lực")]
            InActive = 0,
            [Description("Hiệu lực")]
            Active = 1,
        }
        public enum InputType
        {
            [Description("Chữ")]
            InpText = 0,
            [Description("Số nguyên")]
            InpNumber = 1, //int
            [Description("Số thập phân")]
            InpFloat = 2, //float
            [Description("Tiền tệ")]
            InpMoney = 3, //float
            [Description("Ngày tháng")]
            InpDate = 4,
            [Description("TextArea")]
            InpTextArea = 5,
            [Description("Danh mục động")]
            CategoryType = 6,
            [Description("Danh mục cơ quan")]
            Agency = 7,
            [Description("Danh mục phông")]
            ProfileTemplate = 8,
            [Description("Danh mục mục lục")]
            ProfileList = 9
        }

        public enum Type
        {
            [Description("Văn bản")]
            Doc = 1,
            [Description("Ảnh")]
            Photo = 2,
            [Description("Video")]
            Video = 3,
        }
    }
}