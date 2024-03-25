using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.ESDNghiepVu;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using System.Collections.Generic;
using System.Linq;
using ESD.Application.Enums.DasKTNN;
using ESD.Utility;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Application.Models.ViewModels;
using ESD.Application.Services;
using DocumentFormat.OpenXml.Presentation;
using DASNotify.Application.Constants;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DongVatNghiepVuController : BaseController
    {
        #region Properties

        private readonly IDongVatNghiepVuServices _dongVatNghiepVuService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/DongVatNghiepVu";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public DongVatNghiepVuController(IMapper mapper, IDongVatNghiepVuServices dongVatNghiepVuService, ILoggerManager logger, IExcelServices excel)
        {
            _dongVatNghiepVuService = dongVatNghiepVuService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: DongVatNghiepVu
        public async Task<IActionResult> Index(DongVatNghiepVuCondition condition)
        {
            if (condition.PhanLoai.IsEmpty())
            {
                if (condition.Type == (int)MenuDVNV.Loai)
                {
                    condition.PhanLoai = new[] { (int)PhanLoaiDVNV.Loai, (int)PhanLoaiDVNV.Mat };
                }
                else
                {
                    condition.PhanLoai = new[] { (int)PhanLoaiDVNV.BinhThuong };
                }
            }
            var model = await _dongVatNghiepVuService.SearchByConditionPagging(condition);
            return PartialView(model);
        }


        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(DongVatNghiepVuCondition condition)
        {
            try
            {
                var phanLoais = Utils.GetInts(DATA, nameof(DongVatNghiepVuCondition.PhanLoai));
                condition.PhanLoai = phanLoais;
                ViewBag.Keyword = condition.Keyword;
                var model = await _dongVatNghiepVuService.SearchByConditionPagging(condition);
                return PartialView("Index_DongVatNghiepVus", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy dữ liệu");
            }
            return Redirect(defaultPath);
        }
        #endregion List

        #region Create
        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var type = Utils.GetInt(DATA, "Type");
                var model = await _dongVatNghiepVuService.Create(type);

                if (model.IsKhaiBaoMat)
                    return PartialView("Index_KhaiBaoMat", model);

                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo động vật nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateDongVatNghiepVu dongVatNghiepVu)
        {
            //Validate
            if (!ModelState.IsValid)
                SetErrorModelStateByLine();

            //if (dongVatNghiepVu.IsDaChet == 1)
            //{
            //    dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.Mat;
            //}
            //else if (dongVatNghiepVu.IsThaiLoai == 1)
            //{
            //    dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.Loai;
            //}
            //else
            //{
            //    dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.BinhThuong;
            //}

            CheckValidate(dongVatNghiepVu);

            if (ErrorFields.IsNotEmpty())
                return GetJSErrorResult();

            //CallService
            var time = Utils.GetDate(dongVatNghiepVu.StrKhaiBaoDate);
            if (time != null)
                dongVatNghiepVu.KhaiBaoDate = ((DateTime)time);
            var rs = await _dongVatNghiepVuService.Save(dongVatNghiepVu);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var type = Utils.GetInt(DATA, "Type");
                var model = await _dongVatNghiepVuService.Update(type, id);
                if (model.IsKhaiBaoMat)
                {
                    return PartialView("Index_KhaiBaoMat", model);
                }

                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật động vật nghiệp vụ");
            }
            return Redirect(defaultPath);
        }
        public async Task<IActionResult> DieuChuyen(int? id)
        {
            try
            {
                var type = Utils.GetInt(DATA, "Type");
                var model = await _dongVatNghiepVuService.Update(type, id);
                model.LyDoDieuChuyen = string.Empty;
                return PartialView("Index_DieuChuyen", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật động vật nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateDongVatNghiepVu dongVatNghiepVu)
        {
            //Validate
            if (!ModelState.IsValid)
                SetErrorModelStateByLine();

            var type = Utils.GetInt(DATA, "Type");
            var isKhaiBaoMat = type == (int)MenuDVNV.Loai;

            //if (!isKhaiBaoMat)
            //{
            //    if (dongVatNghiepVu.IsDaChet == 1)
            //    {
            //        dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.Mat;
            //    }
            //    else if (dongVatNghiepVu.IsThaiLoai == 1)
            //    {
            //        dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.Loai;
            //    }
            //    else
            //    {
            //        dongVatNghiepVu.PhanLoai = (int)PhanLoaiDVNV.BinhThuong;
            //    }
            //}

            CheckValidate(dongVatNghiepVu);

            if (ErrorFields.IsNotEmpty())
                return GetJSErrorResult();

            var rs = await _dongVatNghiepVuService.Change(dongVatNghiepVu);
            return CustJSonResult(rs);
        }

        private void CheckValidate(VMUpdateDongVatNghiepVu dongVatNghiepVu)
        {
            //if (dongVatNghiepVu.StrNgaySinh.IsEmpty())
            //{
            //    SetErrorModelState("Giá trị không được để trống", "NgaySinh");
            //}
            //if (dongVatNghiepVu.PhanLoai != (int)PhanLoaiDVNV.BinhThuong && dongVatNghiepVu.ID <= 0)
            //{
            //    SetErrorModelState("Giá trị không được để trống", "Code");
            //}

            if (dongVatNghiepVu.PhanLoai != (int)PhanLoaiDVNV.BinhThuong && dongVatNghiepVu.StrKhaiBaoDate.IsEmpty())
            {
                SetErrorModelState("Giá trị không được để trống", "KhaiBaoDate");
            }
            if (dongVatNghiepVu.PhanLoai != (int)PhanLoaiDVNV.BinhThuong && dongVatNghiepVu.SoQDThaiLoai.IsEmpty())
            {
                SetErrorModelState("Giá trị không được để trống", "SoQDThaiLoai");
            }

            //if (dongVatNghiepVu.PhanLoai != (int)PhanLoaiDVNV.BinhThuong && dongVatNghiepVu.LyDo.IsEmpty() && isKhaiBaoMat)
            //{
            //    SetErrorModelState("Giá trị không được để trống", "LyDo");
            //}

            if (dongVatNghiepVu.PhanLoaiDongVat == (int)PhanLoaiDongVat.ChoNghiepVu && (dongVatNghiepVu.IDNghiepVuDongVat ?? 0) <= 0)
            {
                SetErrorModelState("Giá trị không được để trống", "IDNghiepVuDongVat");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDieuChuyen()
        {
            var dongVatNghiepVu = Utils.Bind<VMUpdateDongVatNghiepVu>(DATA);
            if (dongVatNghiepVu.IDDonViQuanLy <= 0)
            {
                return JSErrorModelState("Giá trị không được để trống", "IDDonViQuanLy");
            }
            if (dongVatNghiepVu.LyDoDieuChuyen.IsEmpty())
            {
                return JSErrorModelState("Giá trị không được để trống", "LyDoDieuChuyen");
            }

            var rs = await _dongVatNghiepVuService.LuuDieuChuyen(dongVatNghiepVu);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(long id)
        {
            var type = Utils.GetInt(DATA, "Type");
            var rs = await _dongVatNghiepVuService.Delete(type, id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn động vật nghiệp vụ cần xoá!");

            var type = Utils.GetInt(DATA, "Type");
            var rs = await _dongVatNghiepVuService.Delete(type, ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        //[HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var type = Utils.GetInt(DATA, "Type");
                var model = await _dongVatNghiepVuService.Update(type, id);
                return View("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin động vật nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        [HttpGet]
        public async Task<IActionResult> Details_Popup(int? id)
        {
            try
            {
                var type = Utils.GetInt(DATA, "Type");
                var model = await _dongVatNghiepVuService.Update(type, id);
                return View("Index_Detail_Popup", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin động vật nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(DongVatNghiepVuCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _dongVatNghiepVuService.SearchByConditionPagging(condition);

            var gioitinhs = Utils.EnumToDic<GioiTinhDongVat>();
            var export = new ExportExtend
            {
                Data = model.DongVatNghiepVus.Select(item =>
                {
                    var giong = model.LoaiChoNghiepVus.FirstOrNewObj(n => n.ID == item.IDLoaiChoNghiepVu);
                    var donviql = model.DonViNghiepVus.FirstOrNewObj(n => n.ID == item.IDDonViQuanLy);
                    var canBo = model.ThongTinCanBos.FirstOrNewObj(n => n.ID == item.IDThongTinCanBo);
                    var phanLoai = Utils.GetDescriptionEnumByKey<PhanLoaiDongVat>((int)item.PhanLoaiDongVat);
                    return new
                    {
                        ID = item.ID,
                        Code = item.Code,
                        Ten = item.Ten,
                        PhanLoai = phanLoai,
                        Giong = giong.Ten,
                        GioiTinh = gioitinhs.GetValueOrDefault(item.GioiTinh ?? 0),
                        DVQL = donviql.Ten,
                        CanBo = canBo.TenCanBo,
                    };

                }).Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Ten"),
                    new Col("PhanLoai"),
                    new Col("Giong"),
                    new Col("GioiTinh"),
                    new Col("DVQL"),
                    new Col("CanBo"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header("Mã định danh"),
                  new Header("Tên gọi"),
                  new Header("Phân loại"),
                  new Header("Giống động vật"),
                  new Header("Giới tính"),
                  new Header("Đơn vị quản lý"),
                  new Header("Cán bộ huấn luyện"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = $"Báo cáo {(condition.Type == (int)MenuDVNV.Loai ? "động vật chết bị loại" : "động vật nghiệp vụ")}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion


        #region Events
        /// <summary> 
        /// Lấy option con khi đổi cha
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetCanBoByIDDonVi()
        {
            var selectedId = Utils.GetBigInt(DATA, "SelectedID");
            var defaultText = Utils.GetString(DATA, "DefaultText");
            var html = string.Empty;
            var canBos = await _dongVatNghiepVuService.GetCanBoHuanLuyen(selectedId);
            if (canBos.IsNotEmpty())
                html = Utils.RenderOptions(canBos.Select(n => new OptionModel { ID = n.ID, Name = n.TenCanBo }).ToList(), 0, true, defaultText, "0");
            return new JsonResult(new
            {
                Type = CommonConst.Success,
                Data = html
            });
        }

        #endregion
    }
}
