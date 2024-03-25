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

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DongVatMat_BiLoaiController : BaseController
    {
        #region Properties

        private readonly IDongVatNghiepVuServices _dongVatNghiepVuService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/DongVatMat_BiLoai";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public DongVatMat_BiLoaiController(IMapper mapper, IDongVatNghiepVuServices dongVatNghiepVuService, ILoggerManager logger, IExcelServices excel)
        {
            _dongVatNghiepVuService = dongVatNghiepVuService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: DongVatNghiepVu
        public async Task<IActionResult> Index(DongVatMat_BiLoaiCondition condition)
        {
            var model = await _dongVatNghiepVuService.SearchReportByConditionPagging(condition);
            return PartialView(model);
        }


        [HasPermission((int)EnumModule.Code.DongVatNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(DongVatMat_BiLoaiCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Year;
                var model = await _dongVatNghiepVuService.SearchReportByConditionPagging(condition);
                return PartialView("Index_DongVatMat_BiLoai", model);
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

        #region Exports
        public async Task<IActionResult> Export(DongVatMat_BiLoaiCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _dongVatNghiepVuService.SearchReportByConditionPagging(condition);

            var gioitinhs = Utils.EnumToDic<GioiTinhDongVat>();
            var export = new ExportExtend
            {
                Data = model.DongVatNghiepVus.Select(item =>
                {
                    var giong = model.LoaiChoNghiepVus.FirstOrNewObj(n => n.ID == item.IDLoaiChoNghiepVu);
                    var donvihl = model.DonViNghiepVus.FirstOrNewObj(n => n.ID == item.IDDonViNghiepVu);
                    var canBo = model.ThongTinCanBos.FirstOrNewObj(n => n.ID == item.IDThongTinCanBo);
                    var hysinh = item.PhanLoai == 3 ? 1 : 0;
                    var thailoai = item.PhanLoai == 2 ? 1 : 0;
                    string chuyenKhoa = "";

                    DateTime time;
                    if (item.KhaiBaoDate == null)
                        time = DateTime.Now;
                    else
                        time = (DateTime)item.KhaiBaoDate;
                    
                    try
                    {
                        if (item.IDNghiepVuDongVat != null)
                            chuyenKhoa = model.NghiepVuDongVats.FirstOrNewObj(p => p.ID == item.IDNghiepVuDongVat).Code;
                    } catch(Exception ex) { } 

                    return new
                    {
                        ID = item.ID,
                        Code = item.Code,
                        Ten = item.Ten,
                        Giong = giong.Ten,
                        GioiTinh = gioitinhs.GetValueOrDefault(item.GioiTinh ?? 0),
                        DVHL = donvihl.Ten,
                        CanBo = canBo.TenCanBo,
                        LyDo = item.LyDo,
                        CK = chuyenKhoa,
                        SoQDBS = item.SoQDBS,
                        HySinh = hysinh,
                        ThaiLoai = thailoai,
                        Time = time.ToString("dd/MM/yyyy"),
                        GhiChu = item.GhiChu
    };

                }).Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("CanBo"),
                    new Col("DVHL"),
                    new Col("Ten"),
                    new Col("Giong"),
                    new Col("GioiTinh"),
                    new Col("CK"),
                    new Col("SoQDBS"),
                    new Col("HySinh"),
                    new Col("ThaiLoai"),
                    new Col("LyDo"),
                    new Col("Time"),
                    new Col("GhiChu"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header("Họ tên CBHL"),
                  new Header("Đơn vị"),
                  new Header("Tên Chó NV"),
                  new Header("Giống chó"),
                  new Header("Tính biệt"),
                  new Header("CK"),
                  new Header("Số hiệu"),
                  new Header("Chết"),
                  new Header("Thải loại"),
                  new Header("Nguyên nhân"),
                  new Header("Thời gian"),
                  new Header("Ghi chú"),

                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = $"Báo cáo động vật chết, bị loại.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion

    }
}
