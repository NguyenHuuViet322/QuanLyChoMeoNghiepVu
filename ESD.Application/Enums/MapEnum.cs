using System.Collections.Generic;

namespace ESD.Application.Enums
{
    public static class MapEnum
    {
        public static Dictionary<int, List<EnumPermission.Type>> MatrixPermission
        {
            get
            {
                var Map = new Dictionary<int, List<EnumPermission.Type>>();

                //Quản trị hệ thống
                Map.Add((int)EnumModule.Code.M20010, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });

                Map.Add((int)EnumModule.Code.S9023, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });

                Map.Add((int)EnumModule.Code.NKND, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export });

                Map.Add((int)EnumModule.Code.NKHT, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export });

                Map.Add((int)EnumModule.Code.CHTS, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Create });
                Map.Add((int)EnumModule.Code.M20020, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });

                Map.Add((int)EnumModule.Code.S9030, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create });

                Map.Add((int)EnumModule.Code.S9010, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Grant });

                Map.Add((int)EnumModule.Code.S9020, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import, EnumPermission.Type.Grant });
                Map.Add((int)EnumModule.Code.QLPQ, new List<EnumPermission.Type> { });
                Map.Add((int)EnumModule.Code.M20030, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Print, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.QLMENU, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create });

                //Quản lý Cơ sở dũ liệu

                Map.Add((int)EnumModule.Code.DongVatNghiepVu, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.DonViNghiepVu, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.CoSoVatChat, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.QLDM, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.ThongTinCanBo, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                Map.Add((int)EnumModule.Code.NghiepVuDongVat, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
               
                Map.Add((int)EnumModule.Code.CapPhatCSVC, new List<EnumPermission.Type> { EnumPermission.Type.Read, EnumPermission.Type.Export, EnumPermission.Type.Update, EnumPermission.Type.Deleted, EnumPermission.Type.Create, EnumPermission.Type.Import });
                 
                return Map;
            }
        }



    }
}
