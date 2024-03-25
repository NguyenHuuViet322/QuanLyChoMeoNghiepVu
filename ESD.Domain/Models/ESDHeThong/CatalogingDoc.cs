using ESD.Domain.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESD.Domain.Models.DAS
{
    /// <summary>
    /// https://m.thuvienphapluat.vn/van-ban/linh-vuc-khac/Thong-tu-02-2019-TT-BNV-tieu-chuan-du-lieu-thong-tin-dau-vao-406241.aspx
    /// Van ban - tai lieu
    /// </summary>
    public class CatalogingDoc : Auditable
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int IDChannel { get; set; } = 0;

        public long IDFile { get; set; }

        public long IDDoc { get; set; }

        [Required]
        public int IDCatalogingProfile { get; set; } //ID bang ho so 

        public int IDDocType { get; set; } = 0; //IDLoai tai lieu

        public int Status { get; set; } = 1;
        public bool IsPublic { get; set; } = true; //Tài liệu public?

        //for Destructtion
        public int InUsing { get; set; } = 1; //EnumCataloging.InUse

    }
}