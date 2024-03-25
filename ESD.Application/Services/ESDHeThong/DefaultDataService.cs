using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.DAS;
using ESD.Utility;
using System.Collections.Generic;

namespace ESD.Application.Services
{
    public interface IDefaultDataService
    {
        List<VMDocTypeField> GetDefaultDocTypeFields(IEnumerable<CategoryType> categoryTypes, VMDocType vmDocType, int type);
        List<VMUpdateCategoryTypeField> GetDefaultCategoryFields(IEnumerable<CategoryType> categoryTypes, string typeName, string codeType);
    }

    public class DefaultDataService : BaseMasterService, IDefaultDataService
    {
        private readonly IMapper _mapper;
        public DefaultDataService(
            IMapper mapper,
            IDasRepositoryWrapper dasRepository,
             IESDNghiepVuRepositoryWrapper dasKTNN
            ) : base(dasRepository, dasKTNN)
        {
            _mapper = mapper;
        }

        #region Get

        #region DocTypes
        /// <summary>
        /// Get cau hinh tam
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vmDocType">EnumDocType.Type</param>
        /// <returns></returns>
        public List<VMDocTypeField> GetDefaultDocTypeFields(IEnumerable<CategoryType> categoryTypes, VMDocType vmDocType, int type)
        {
            var temp = new List<VMDocTypeField>();
            if (type == 0)
            {
                //Fix dữ liệu
                temp = new List<VMDocTypeField>() {
                        new VMDocTypeField
                        {
                            ID= 1,//fix để lấy dữ liệu DocField fake
                            Name = $"Mã {vmDocType.Name}",
                            Code = "Code",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsRequire = 1,
                            Priority = 1,
                            Minlenght = 0,
                            Maxlenght = 50,
                            IsBase = 1
                        },
                    new VMDocTypeField
                        {
                            ID= 2,//fix để lấy dữ liệu DocField fake
                            Name = $"Tên {vmDocType.Name}",
                            Code = "Name",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 2,
                            Minlenght = 0,
                            Maxlenght = 250,
                            IsBase = 1
                        }};
            }
            else if (type == (int)EnumDocType.Type.Doc)
            {
                //Fix dữ liệu
                temp = new List<VMDocTypeField>() {
                        new VMDocTypeField
                        {
                            ID= 1,
                            Name = "Mã định danh văn bản",
                            Code = "DocCode",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsRequire = 1,
                            IsSearchGrid = 1,
                            Priority = 1,
                            Minlenght = 0,
                            Maxlenght = 25,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 2,
                            Name = "Mã hồ sơ",
                            Code = "FileCode",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 2,
                            Minlenght = 0,
                            Maxlenght = 30,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 3,
                            Name = "Mã cơ quan lưu trữ lịch sử",
                            Code = "Identifier",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 3,
                            Minlenght = 0,
                            Maxlenght = 13,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 4,
                            Name = "Mã phông/công trình/sưu tập lưu trữ",
                            Code = "Organld",
                            InputType = (int)EnumDocType.InputType.ProfileTemplate,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 4,
                            Minlenght = 0,
                            Maxlenght = 13,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                         {
                             ID= 5,
                             Name = "Mục lục số hoặc năm hình thành hồ sơ",
                             Code = "FileCatalog",
                             InputType = (int)EnumDocType.InputType.InpNumber,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 5,
                             Minlenght = 0,
                             Maxlenght = 4,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 6,
                             Name = "Số và ký hiệu hồ sơ",
                             Code = "FileNotation",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 6,
                             Minlenght = 0,
                             Maxlenght = 20,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 7,
                             Name = "Số thứ tự văn bản trong hồ sơ",
                             Code = "DocOrdinal",
                             InputType = (int)EnumDocType.InputType.InpNumber,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 7,
                             Minlenght = 0,
                             Maxlenght = 4,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 8,
                             Name = "Tên loại văn bản",
                             Code = "TypeName",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             IsRequire = 1,
                             Priority = 8,
                             Minlenght = 0,
                             Maxlenght = 100,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 9,
                             Name = "Số của văn bản",
                             Code = "CodeNumber",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 9,
                             Minlenght = 0,
                             Maxlenght = 11,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 10,
                             Name = "Ký hiệu của văn bản",
                             Code = "CodeNotation",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 10,
                             Minlenght = 0,
                             Maxlenght = 30,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 11,
                             Name = "Ngày, tháng, năm văn bản",
                             Code = "IssuedDate",
                             InputType = (int)EnumDocType.InputType.InpDate,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 11,
                             Format = CommonConst.DfDateFormat,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                        {
                            ID= 12,
                            Name = "Tên cơ quan, tổ chức ban hành văn bản",
                            Code = "OrganName",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 12,
                            Minlenght = 0,
                            Maxlenght = 200,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 13,
                            Name = "Trích yếu nội dung",
                            Code = "Subject",
                            InputType = (int)EnumDocType.InputType.InpTextArea,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 13,
                            Minlenght = 0,
                            Maxlenght = 500,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 14,
                            Name = "Ngôn ngữ",
                            Code = "Language",
                            InputType = (int)EnumDocType.InputType.CategoryType,
                            IDCategoryTypeRelated = categoryTypes.FirstOrNewObj(n=>n.Code == EnumCategoryType.Code.DM_NgonNgu.ToString()).ID,
                            IsShowGrid = 1,
                            Priority = 14,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                         {
                             ID= 15,
                             Name = "Số lượng trang của văn bản",
                             Code = "PageAmount",
                             InputType = (int)EnumDocType.InputType.InpNumber,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 15,
                             Minlenght = 0,
                             Maxlenght = 4,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 16,
                             Name = "Ghi chú",
                             Code = "Description",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 16,
                             Minlenght = 0,
                             Maxlenght = 500,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                        {
                            ID= 17,
                            Name = "Ký hiệu thông tin",
                            Code = "InforSign",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 17,
                            Minlenght = 0,
                            Maxlenght = 30,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                         {
                             ID= 18,
                             Name = "Từ khóa",
                             Code = "Keyword",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 18,
                             Minlenght = 0,
                             Maxlenght = 100,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                        {
                            ID= 19,
                            Name = "Chế độ sử dụng",
                            Code = "Mode",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 19,
                            Minlenght = 0,
                            Maxlenght = 20,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                          {
                              ID= 20,
                              Name = "Mức độ tin cậy",
                              Code = "ConfidenceLevel",
                              InputType = (int)EnumDocType.InputType.InpText,
                              IsShowGrid = 1,
                              IsSearchGrid = 1,
                              Priority = 20,
                              Minlenght = 0,
                              Maxlenght = 30,
                              IsBase = 1
                          }
                        ,new VMDocTypeField
                         {
                             ID= 21,
                             Name = "Bút tích",
                             Code = "Autograph",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 21,
                             Minlenght = 0,
                             Maxlenght = 2000,
                             IsBase = 1
                         }
                        ,new VMDocTypeField
                         {
                             ID= 22,
                             Name = "Tình trạng vật lý",
                             Code = "Format",
                             InputType = (int)EnumDocType.InputType.InpText,
                             IsShowGrid = 1,
                             IsSearchGrid = 1,
                             Priority = 22,
                             Minlenght = 0,
                             Maxlenght = 50,
                             IsBase = 1
                         }
                    };
            }
            else if (type == (int)EnumDocType.Type.Photo)
            {
                //Fix dữ liệu
                temp = new List<VMDocTypeField>() {
                         new VMDocTypeField
                        {
                            ID= 1,
                            Name = "Mã cơ quan lưu trữ",
                            Code = "Identifier",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 1,
                            Minlenght = 0,
                            Maxlenght = 13,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 2,
                            Name = "Số lưu trữ",
                            Code = "ArchivesNumber",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 2,
                            Minlenght = 0,
                            Maxlenght = 50,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 3,
                            Name = "Ký hiệu thông tin",
                            Code = "InforSign",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 3,
                            Minlenght = 0,
                            Maxlenght = 30,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 4,
                            Name = "Tên sự kiện",
                            Code = "EventName",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 4,
                            Minlenght = 0,
                            Maxlenght = 500,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 5,
                            Name = "Tiêu đề phim/ảnh",
                            Code = "ImageTitle",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 5,
                            Minlenght = 0,
                            Maxlenght = 500,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 6,
                            Name = "Ghi chú",
                            Code = "Description",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 6,
                            Minlenght = 0,
                            Maxlenght = 500,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 7,
                            Name = "Tác giả",
                            Code = "Photographer",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 7,
                            Minlenght = 0,
                            Maxlenght = 300,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 8,
                            Name = "Địa điểm chụp",
                            Code = "PhotoPlace",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 8,
                            Minlenght = 0,
                            Maxlenght = 300,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 9,
                            Name = "Thời gian chụp",
                            Code = "PhotoTime",
                            InputType = (int)EnumDocType.InputType.InpDate,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 9,
                            Format = CommonConst.DfDateFormat,
                            IsBase = 1
                        }
                        ,new VMDocTypeField
                        {
                            ID= 10,
                            Name = "Màu sắc",
                            Code = "Colour",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 10,
                            Minlenght = 0,
                            Maxlenght = 50,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 11,
                            Name = "Cỡ phim/ảnh",
                            Code = "FilmSize",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 11,
                            Minlenght = 0,
                            Maxlenght = 5,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 12,
                            Name = "Tài liệu đi kèm",
                            Code = "DocAttached",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 12,
                            Minlenght = 0,
                            Maxlenght = 300,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 13,
                            Name = "Chế độ sử dụng",
                            Code = "Mode",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 13,
                            Minlenght = 0,
                            Maxlenght = 20,
                            IsBase = 1
                        }
                         ,new VMDocTypeField
                        {
                            ID= 14,
                            Name = "Tình trạng vật lý",
                            Code = "Format",
                            InputType = (int)EnumDocType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            Priority = 14,
                            Minlenght = 0,
                            Maxlenght = 50,
                            IsBase = 1
                        }
                    };
            }
            else if (type == (int)EnumDocType.Type.Video)
            {
                //Fix dữ liệu
                temp = new List<VMDocTypeField>() {
                    new VMDocTypeField
                    {
                        ID = 1,
                        Name = "Mã cơ quan lưu trữ",
                        Code = "Identifier",
                        InputType = (int)EnumDocType.InputType.InpText,
                        IsShowGrid = 1,
                        IsSearchGrid = 1,
                        IsRequire = 1,
                        Priority = 1,
                        Minlenght = 1,
                        Maxlenght = 13,
                        IsBase = 1
                    }
                    ,new VMDocTypeField
                    {
                        ID = 2,
                        Name = "Số lưu trữ",
                        Code = "ArchivesNumber",
                        InputType = (int)EnumDocType.InputType.InpText,
                        IsShowGrid = 1,
                        IsSearchGrid = 1,
                        IsRequire = 1,
                        Priority = 2,
                        Minlenght = 1,
                        Maxlenght = 50,
                        IsBase = 1
                    }
                    ,new VMDocTypeField
                     {
                         ID = 3,
                         Name = "Ký hiệu thông tin",
                         Code = "InforSign",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 3,
                         Minlenght = 1,
                         Maxlenght = 30,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 4,
                         Name = "Tên sự kiện",
                         Code = "EventName",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 4,
                         Minlenght = 1,
                         Maxlenght = 500,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 5,
                         Name = "Tiêu đề phim/âm thanh",
                         Code = "MovieTitle",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         IsRequire = 1,
                         Priority = 5,
                         Minlenght = 1,
                         Maxlenght = 500,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 6,
                         Name = "Ghi chú",
                         Code = "Description",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 6,
                         Minlenght = 1,
                         Maxlenght = 500,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 7,
                         Name = "Tác giả",
                         Code = "Recorder",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 7,
                         Minlenght = 1,
                         Maxlenght = 300,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 8,
                         Name = "Địa điểm",
                         Code = "RecordPlace",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 8,
                         Minlenght = 1,
                         Maxlenght = 300,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 9,
                         Name = "Thời gian",
                         Code = "RecordDate",
                         InputType = (int)EnumDocType.InputType.InpDate,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 9,
                         Format = "dd/MM/yyyy",
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 10,
                         Name = "Ngôn ngữ",
                         Code = "Language",
                         InputType = (int)EnumDocType.InputType.CategoryType,
                         IDCategoryTypeRelated = categoryTypes.FirstOrNewObj(n=>n.Code == EnumCategoryType.Code.DM_NgonNgu.ToString()).ID,
                         IsShowGrid = 1,
                         Priority = 10,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 11,
                         Name = "Thời lượng",
                         Code = "PlayTime",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 11,
                         Minlenght = 1,
                         Maxlenght = 8,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 12,
                         Name = "Tài liệu đi kèm",
                         Code = "DocAttached",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 12,
                         Minlenght = 1,
                         Maxlenght = 300,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 13,
                         Name = "Chế độ sử dụng",
                         Code = "Mode",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         IsRequire = 1,
                         Priority = 13,
                         Minlenght = 1,
                         Maxlenght = 20,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 14,
                         Name = "Chất lượng",
                         Code = "Quality",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 14,
                         Minlenght = 1,
                         Maxlenght = 50,
                         IsBase = 1
                     }
                    ,new VMDocTypeField
                     {
                         ID = 15,
                         Name = "Tình trạng vật lý",
                         Code = "Format",
                         InputType = (int)EnumDocType.InputType.InpText,
                         IsShowGrid = 1,
                         IsSearchGrid = 1,
                         Priority = 15,
                         Minlenght = 1,
                         Maxlenght = 50,
                         IsBase = 1
                     }
                    };
            }
            return temp;
        }
        #endregion DocTypes

        #region CategoryTypes

        /// <summary>
        /// Get du lieu danh muc muc dinh 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="categoryTypes"></param>
        /// <param name="vmCategpruyType"></param>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public List<VMUpdateCategoryTypeField> GetDefaultCategoryFields(IEnumerable<CategoryType> categoryTypes, string typeName, string codeType)
        {
            var temp = new List<VMUpdateCategoryTypeField>();
            //if (codeType.IsEmpty())
            //{
            //    //Fix dữ liệu
            //    return GetCodeAndNameField(typeName);
            //}
            if (codeType == EnumCategoryType.Code.DM_Kho.ToString())
            {
                //Fix dữ liệu
                temp = GetCodeAndNameField(typeName);
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 3,//fix để lấy dữ liệu categoryField fake
                    Name = "Đơn vị",
                    InputType = (int)EnumCategoryType.InputType.Agency,
                    IsShowGrid = 1,
                    Priority = 3,
                    DefaultValueType = (int)EnumCategoryType.DefaultValue.ByUser
                });
            }
            else if (codeType == EnumCategoryType.Code.DM_Gia.ToString())
            {
                //Fix dữ liệu
                temp = GetCodeAndNameField(typeName);
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 3,//fix để lấy dữ liệu categoryField fake
                    Name = "Kho",
                    Code = "Kho",
                    InputType = (int)EnumCategoryType.InputType.CategoryType,
                    IDCategoryTypeRelated = categoryTypes.FirstOrNewObj(n => n.Code == EnumCategoryType.Code.DM_Kho.ToString()).ID,
                    IsShowGrid = 1,
                    IsRequire = 1,
                    Priority = 3,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 4,//fix để lấy dữ liệu categoryField fake
                    Name = "Vị trí giá/kệ",
                    InputType = (int)EnumCategoryType.InputType.InpText,
                    Priority = 4,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 5,//fix để lấy dữ liệu categoryField fake
                    Name = "Mô tả",
                    InputType = (int)EnumCategoryType.InputType.InpTextArea,
                    Priority = 5,
                });
            }
            else if (codeType == EnumCategoryType.Code.DM_HopSo.ToString())
            {
                //Fix dữ liệu
                temp = GetCodeAndNameField(typeName);
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 3,//fix để lấy dữ liệu categoryField fake
                    Name = "Kho",
                    Code = "Kho",
                    InputType = (int)EnumCategoryType.InputType.CategoryType,
                    IDCategoryTypeRelated = categoryTypes.FirstOrNewObj(n => n.Code == EnumCategoryType.Code.DM_Kho.ToString()).ID,
                    IsShowGrid = 1,
                    IsRequire = 1,
                    Priority = 3,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 4,//fix để lấy dữ liệu categoryField fake
                    Name = "Giá/kệ",
                    Code = "Gia",
                    InputType = (int)EnumCategoryType.InputType.CategoryType,
                    IDCategoryTypeRelated = categoryTypes.FirstOrNewObj(n => n.Code == EnumCategoryType.Code.DM_Gia.ToString()).ID,
                    IsShowGrid = 1,
                    IsRequire = 1,
                    Priority = 4,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 5,//fix để lấy dữ liệu categoryField fake
                    Name = "Vị trí hộp",
                    InputType = (int)EnumCategoryType.InputType.InpText,
                    Priority = 5,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 6,//fix để lấy dữ liệu categoryField fake
                    Name = "Mô tả",
                    InputType = (int)EnumCategoryType.InputType.InpTextArea,
                    Priority = 6,
                });
            }
            else if (codeType == EnumCategoryType.Code.DM_NgonNgu.ToString())
            {
                //Fix dữ liệu
                temp = GetCodeAndNameField(typeName);
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 3,//fix để lấy dữ liệu categoryField fake
                    Name = "Mô tả",
                    InputType = (int)EnumCategoryType.InputType.InpTextArea,
                    Priority = 3,
                });
            }
            else if (codeType == EnumCategoryType.Code.DM_PhanLoaiHS.ToString())
            {
                //Fix dữ liệu
                temp = GetCodeAndNameField(typeName);
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 3,//fix để lấy dữ liệu categoryField fake
                    Name = "Mô tả",
                    Code = "Describe",
                    InputType = (int)EnumCategoryType.InputType.InpTextArea,
                    IsShowGrid = 1,
                    Priority = 3,
                });
                temp.Add(new VMUpdateCategoryTypeField
                {
                    ID = 4,//fix để lấy dữ liệu categoryField fake
                    Name = "Cấp cha",
                    Code = "Parent",
                    InputType = (int)EnumCategoryType.InputType.Parent,
                    IsShowGrid = 1,
                    Priority = 4,
                });
            }
            else
            {
                return GetCodeAndNameField(typeName);
            }
            return temp;
        }

        private List<VMUpdateCategoryTypeField> GetCodeAndNameField(string typeName)
        {
            typeName = typeName?.ToLower();
            return new List<VMUpdateCategoryTypeField>() {
                        new VMUpdateCategoryTypeField
                        {
                            ID= 1,//fix để lấy dữ liệu categoryField fake
                            Name = $"Mã {typeName}",
                            Code = "Code",
                            InputType = (int)EnumCategoryType.InputType.InpText,
                            IsShowGrid = 1,
                            IsRequire = 1,
                            Priority = 1,
                            Minlenght = 1,
                            Maxlenght = 50
                        },
                        new VMUpdateCategoryTypeField
                        {
                            ID= 2,//fix để lấy dữ liệu categoryField fake
                            Name = $"Tên {typeName}",
                            Code = "Name",
                            InputType = (int)EnumCategoryType.InputType.InpText,
                            IsShowGrid = 1,
                            IsSearchGrid = 1,
                            IsRequire = 1,
                            Priority = 2,
                            Minlenght = 1,
                            Maxlenght = 250
                        }};
        }

        #endregion CategoryTypes
        #endregion CategoryTypes

    }
}