using System.ComponentModel;

namespace ESD.Application.Enums
{
    public static class EnumFile
    {
        public enum Type
        {
            [Description("Avatar")]
            Avatar = 1,
            [Description("Văn bản")]
            Doc = 2,
            [Description("Âm thanh")]
            Audio = 3,
            [Description("Video")]
            Video = 4,
            [Description("Database")]
            Backup = 5,
            [Description("Template")]
            Template = 6,
        }
    }
}
