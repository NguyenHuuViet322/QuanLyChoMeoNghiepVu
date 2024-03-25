using ESD.Application.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESD.Application.Models.CustomModels
{
    public static class ModuleOfRole
    {
        public static int[] SuperAdmin => new int[] {
                (int)EnumModule.Code.M20010 // cơ quan
                , (int)EnumModule.Code.S9023 // quản trị cơ quan
                //, (int)EnumModule.Code.M20130, (int)EnumModule.Code.CDBM, (int)EnumModule.Code.TGBQ,  (int)EnumModule.Code.M20030 // dm dùng chung
                , (int)EnumModule.Code.NKHT //nhật ký hệ thống
                , (int)EnumModule.Code.NKND //nhật ký người dùng
                , (int)EnumModule.Code.CHTS // cấu hình tham số hệ thống
                , (int)EnumModule.Code.S9030 // quản lý nhóm quyền 
                , (int)EnumModule.Code.QLMENU // quản lý menu
                , (int)EnumModule.Code.M20020 // quản lý đơn vị
                , (int)EnumModule.Code.S9020 // người dùng
                , (int)EnumModule.Code.S9010 // quản lý nhóm người dùng
                , (int)EnumModule.Code.QLPQ // quản lý phân quyền
                //, (int)EnumModule.Code.QLRH // quản lý lịch sử phát hành
            };

        public static int[] Admin => new int[] {
                 (int)EnumModule.Code.M20020 // quản lý đơn vị
                , (int)EnumModule.Code.S9020 // người dùng
                , (int)EnumModule.Code.S9010 // nhóm người dùng
                , (int)EnumModule.Code.NKND //nhật ký người dùng
                , (int)EnumModule.Code.S9030 //Quản lý nhóm quyền
                , (int)EnumModule.Code.CHTS // Cấu hình tham số cơ quan
                , (int)EnumModule.Code.QLMENU
            };
    }
}
