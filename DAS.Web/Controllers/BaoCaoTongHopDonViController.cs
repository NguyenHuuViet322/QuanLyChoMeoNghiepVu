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
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Utility;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaoCaoTongHopDonViController : BaseController
    {
        #region Properties

        private readonly IDonViNghiepVuServices _donViNghiepVuService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/BaoCaoTongHopDonVi";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public BaoCaoTongHopDonViController(IMapper mapper, IDonViNghiepVuServices donViNghiepVuService, ILoggerManager logger, IExcelServices excel)
        {
            _donViNghiepVuService = donViNghiepVuService;
            _mapper = mapper;
            _excelService = excel;
            _logger = logger;
        }

        #endregion Ctor

        #region List

        // GET: DonViNghiepVuFields
        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(DonViNghiepVuCondition condition)
        {
            var model = await _donViNghiepVuService.SearchReportByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(DonViNghiepVuCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                var model = await _donViNghiepVuService.SearchReportByConditionPagging(condition);
                return PartialView("Index_BaoCaoTongHopDonVi", model);
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
        public async Task<IActionResult> Export(DonViNghiepVuCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _donViNghiepVuService.SearchReportByConditionPagging(condition); 
            var export = new ExportExtend
            {
                Data = model.DonViNghiepVus.Select(item =>
                {
                    return new
                    {
                        Code = item.Code,
                        Ten = item.Ten,
                        SoDongVatNghiepVu = item.SoDongVatNghiepVu,
                        Chet = item.Chet,
                        ThaiLoai = item.ThaiLoai,
                    };
                }).Cast<dynamic>().ToList(),

                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Ten"),
                    new Col("SoDongVatNghiepVu"),
                    new Col("Chet"),
                    new Col("ThaiLoai"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header($"Mã đơn vị"),
                  new Header($"Tên đơn vị"),
                  new Header("Tổng số lượng"),
                  new Header("Số lượng hi sinh"),
                  new Header("Số lượng thải loại"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = $"Báo cáo Tổng hợp.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        #endregion

    }
}
