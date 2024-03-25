using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Application.Enums
{
    public static class EnumElectronicSign
    {
        public enum Type
        {
            //Phân loại chữ ký số theo chuẩn của ban cơ yếu chính phủ. Chi tiết truy cập: http://ca.gov.vn
            [Description("Lãnh đạo ký phê duyệt")]
            BossSign = 1,
            [Description("Văn thư ký phát hành")]
            ReleaseSign = 2,
            [Description("Văn thư ký công văn đến")]
            IncomeDocumentSign = 3,
            [Description("Ký comment")]
            CommentSign = 4,
            [Description("Phụ lục/ Đính kèm")]
            Attachment = 5,
            [Description("Ký số Bản sao điện tử")]
            ElectronicCopySign = 6,
        }
    }
}
