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
using Microsoft.IdentityModel.Tokens;
using ESD.Utility;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ThongTinCanBoController : BaseController
    {
        #region Properties

        private readonly IThongTinCanBoServices _thongTinCanBoService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/ThongTinCanBo";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public ThongTinCanBoController(IMapper mapper, IThongTinCanBoServices thongTinCanBoService, ILoggerManager logger, IExcelServices excel)
        {
            _thongTinCanBoService = thongTinCanBoService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: ThongTinCanBoFields
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(ThongTinCanBoCondition condition)
        {
            var model = await _thongTinCanBoService.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(ThongTinCanBoCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                ViewBag.DonVi = condition.IDDonViNghiepVu;
                var model = await _thongTinCanBoService.SearchByConditionPagging(condition);
                return PartialView("Index_ThongTinCanBos", model);
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
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            try
            {
                var type = Utils.GetInt(DATA, "type");
                var model = await _thongTinCanBoService.Create();
                model.PhanLoai = type;
                model.GioiTinh = 1;
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo thông tin cán bộ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateThongTinCanBo vmThongTinCanBo)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _thongTinCanBoService.Save(vmThongTinCanBo);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _thongTinCanBoService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật thông tin cán bộ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateThongTinCanBo thongTinCanBo)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _thongTinCanBoService.Change(thongTinCanBo);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(long id)
        {
            var rs = await _thongTinCanBoService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn thông tin cán bộ cần xoá!");
            var rs = await _thongTinCanBoService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _thongTinCanBoService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin thông tin cán bộ");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        [HasPermission((int)EnumModule.Code.ThongTinCanBo, new int[] { (int)EnumPermission.Type.Export })]
        public async Task<IActionResult> Export(ThongTinCanBoCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _thongTinCanBoService.SearchByConditionPagging(condition);
            var export = new ExportExtend
            {
                Data = model.ThongTinCanBos.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("TenCanBo"),
                    new Col("BirthDay"),
                    new Col("Gender"),
                    new Col("TenDonViNghiepVu"),
                    new Col("TenChuyenMonKyThuat"),
                    new Col("SoLuongCho"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header("Mã cán bộ"),
                  new Header("Tên cán bộ"),
                  new Header("Ngày sinh"),
                  new Header("Giới tính"),
                  new Header("Đơn vị quản lý"),
                  new Header("Chuyên môn kỹ thuật"),
                  new Header("Số lượng chó quản lý"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Thông tin cán bộ");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "Danh sách thông tin cán bộ.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion
    }
}
