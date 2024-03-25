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
using DocumentFormat.OpenXml.Office2013.Excel;
using System.IO;
using NLog.LayoutRenderers.Wrappers;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore.Internal;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DonViNghiepVuController : BaseController
    {
        #region Properties

        private readonly IDonViNghiepVuServices _donViNghiepVuService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string defaultPath = "/DonViNghiepVu";
        private readonly IExcelServices _excelService;

        #endregion Properties

        #region Ctor

        public DonViNghiepVuController(IMapper mapper, IDonViNghiepVuServices donViNghiepVuService, ILoggerManager logger, IExcelServices excel)
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
            var model = await _donViNghiepVuService.SearchByConditionPagging(condition);
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(DonViNghiepVuCondition condition)
        {
            try
            {
                ViewBag.Keyword = condition.Keyword;
                ViewBag.PhanLoai = condition.PhanLoai;
                var model = await _donViNghiepVuService.SearchByConditionPagging(condition);
                return PartialView("Index_DonViNghiepVus", model);
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
        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create(int PhanLoai = 0)
        {
            try
            {

                var model = await _donViNghiepVuService.Create(PhanLoai);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi khởi tạo đơn vị nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(VMUpdateDonViNghiepVu vmDonViNghiepVu)
        {
            //Validate
            if (!ModelState.IsValid)
                SetErrorModelStateByLine();

            if (vmDonViNghiepVu.SoDongVatNghiepVu < 0)
            {
                SetErrorModelState("Giá trị phải lớn hơn 0", "SoDongVatNghiepVu");
            }
            if (vmDonViNghiepVu.SoCanBo < 0)
            {
                SetErrorModelState("Giá trị phải lớn hơn 0", "SoCanBo");
            }
            if (vmDonViNghiepVu.NamThanhLap.HasValue)
            {
                if (vmDonViNghiepVu.NamThanhLap < 1900)
                {
                    SetErrorModelState("Giá trị phải lớn hơn 1900", "NamThanhLap");
                }
                else if (vmDonViNghiepVu.NamThanhLap > DateTime.Now.Year)
                {
                    SetErrorModelState("Giá trị không được lớn hơn năm hiện tại", "NamThanhLap");
                }
            }

            if (ErrorFields.IsNotEmpty())
                return GetJSErrorResult();

            //CallService
            var rs = await _donViNghiepVuService.Save(vmDonViNghiepVu);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var model = await _donViNghiepVuService.Update(id);
                return PartialView("Index_Update", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin cập nhật đơn vị nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(VMUpdateDonViNghiepVu vmDonViNghiepVu)
        {
            //Validate
            if (!ModelState.IsValid)
                SetErrorModelStateByLine();

            if (vmDonViNghiepVu.SoDongVatNghiepVu < 0)
            {
                SetErrorModelState("Giá trị phải lớn hơn 0", "SoDongVatNghiepVu");
            }
            if (vmDonViNghiepVu.SoCanBo < 0)
            {
                SetErrorModelState("Giá trị phải lớn hơn 0", "SoCanBo");
            }
            if (vmDonViNghiepVu.NamThanhLap.HasValue)
            {
                if (vmDonViNghiepVu.NamThanhLap < 1900)
                {
                    SetErrorModelState("Giá trị phải lớn hơn 1900", "NamThanhLap");
                }
                else if (vmDonViNghiepVu.NamThanhLap > DateTime.Now.Year)
                {
                    SetErrorModelState("Giá trị không được lớn hơn năm hiện tại", "NamThanhLap");
                }
            }

            if (ErrorFields.IsNotEmpty())
                return GetJSErrorResult();

            //CallService
            var rs = await _donViNghiepVuService.Change(vmDonViNghiepVu);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(long id)
        {
            var rs = await _donViNghiepVuService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(long[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn đơn vị nghiệp vụ cần xoá!");
            var rs = await _donViNghiepVuService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        [HasPermission((int)EnumModule.Code.DonViNghiepVu, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var model = await _donViNghiepVuService.Update(id);
                return PartialView("Index_Detail", model);
            }
            catch (LogicException ex)
            {
                SetError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                SetError("Có lỗi khi lấy thông tin đơn vị nghiệp vụ");
            }
            return Redirect(defaultPath);
        }

        #endregion

        #region Exports
        public async Task<IActionResult> Export(DonViNghiepVuCondition condition)
        {
            condition.PageSize = 5000;
            var model = await _donViNghiepVuService.SearchByConditionPagging(condition);
            var export = new ExportExtend
            {
                Data = model.DonViNghiepVus.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Ten"),
                    new Col("DiaChi"),
                    new Col("SoDongVatNghiepVu"),
                    new Col("SoCanBo"),
                },
                Headers = new List<Header>
                {
                  new Header("STT", 5),
                  new Header($"Mã {model.TenPhanLoai}"),
                  new Header($"Tên {model.TenPhanLoai}"),
                  new Header("Địa chỉ"),
                  new Header("Số ĐVNV"),
                  new Header("Số cán bộ"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Báo cáo");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = $"Báo cáo {model.TenPhanLoai}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        public async Task<IActionResult> ExportReport(int id)
        {
            var model = await _donViNghiepVuService.ExportReport(id);

            var singleDict = new Dictionary<string, string>();
            var tableDict = new Dictionary<string, Dictionary<long, Dictionary<string, string>>>();

            var tableRows = new Dictionary<long, Dictionary<string, string>>();
            var index = 0;
            var sttCanBo = 0;
            var donVi = model.DonVi;
            model.DongVats = model.DongVats ?? new List<VMDongVatNghiepVu>();
            if (model.CanBos.IsNotEmpty())
            {
                var idCanboQLs = model.DongVats.Select(n => n.IDThongTinCanBo).Distinct();
                var sortCanBos = model.CanBos.OrderBy(cb => idCanboQLs.Contains(cb.ID) ? 0 : 1); //Cán bộ quản lý đv lên trước
                foreach (var canBo in sortCanBos)
                {
                    var dongVats = model.DongVats.Where(n => n.IDThongTinCanBo == canBo.ID);
                    if (dongVats.IsEmpty())
                    {
                        dongVats = new List<VMDongVatNghiepVu>()
                        {
                            new VMDongVatNghiepVu()
                        };
                    }
                    var sttDv = 0;
                    foreach (var dongVat in dongVats)
                    {
                        sttDv++;
                        index++;
                        var isFirstRow = sttDv == 1; //Là động vật quản lý thứ 1 
                        if (isFirstRow)
                        {
                            sttCanBo++; //Chỉ tăng stt cán bộ với động vật quản lý thứ 1 
                        }
                        var dv = new Dictionary<string, string>();
                        var cmnv = model.ChuyenMonKiThuats.FirstOrDefault(n => n.ID == canBo.IDChuyenMonKiThuat) ?? new VMChuyenMonKiThuat();
                        var giong = model.LoaiChoNghiepVus.FirstOrDefault(n => n.ID == dongVat.IDLoaiChoNghiepVu) ?? new VMLoaiChoNghiepVu();
                        var nvdv = model.NghiepVuDongVats.FirstOrDefault(n => n.ID == dongVat.IDNghiepVuDongVat) ?? new VMNghiepVuDongVat();
                        var phanLoai = Utils.GetDescriptionEnumByKey<PhanLoaiDongVat>(dongVat.PhanLoaiDongVat??0);

                        dv.Add("Index", isFirstRow ? sttCanBo.ToString() : ""); //Chỉ hiện STT cán bộ với động vật quản lý thứ 1 
                        dv.Add("TenCanBo", isFirstRow ? canBo.TenCanBo : "--"); //Chỉ hiện Tên cán bộ với động vật quản lý thứ 1 
                        dv.Add("CMNV", cmnv.Ten);
                        dv.Add("TenChoNV", dongVat.Ten);
                        dv.Add("PhanLoai", phanLoai);
                        dv.Add("Giong", giong.Ten);
                        dv.Add("ChuyenKhoa", nvdv.Ten);
                        dv.Add("Code", dongVat.Code);
                        dv.Add("NamTN", dongVat.NgaySinh.HasValue ? dongVat.NgaySinh.Value.Year.ToString() : string.Empty);
                        dv.Add("GhiChu", dongVat.GhiChu);

                        tableRows.Add(index, dv);
                    }
                }
            }

            tableDict.Add("tbDv", tableRows);

            AddKey(singleDict, "tieude", donVi.DonViTrucThuoc.ToUpper());
            AddKey(singleDict, "ten", donVi.Ten);
            AddKey(singleDict, "diachi", donVi.DiaChi);
            AddKey(singleDict, "namthanhlap", donVi.NamThanhLap > 0 ? donVi.NamThanhLap.ToString() : string.Empty);

            AddKey(singleDict, "truongphongTen", donVi.LanhDaoDonVi);
            AddKey(singleDict, "truongphongSdt", donVi.Sdt_LanhDaoDonVi);

            AddKey(singleDict, "lanhdaoTen", donVi.LanhDaoPhuTrachTT);
            AddKey(singleDict, "lanhdaoSdt", donVi.Sdt_LanhDaoPhuTrachTT);

            AddKey(singleDict, "doitruongTen", donVi.DoiTruong);
            AddKey(singleDict, "doitruongSdt", donVi.Sdt_DoiTruong);

            AddKey(singleDict, "doiphoTen", donVi.DoiPho);
            AddKey(singleDict, "doiphoSdt", donVi.Sdt_DoiPho);


            AddKey(singleDict, "soCBCS", model.CanBos.Count().ToString());
            AddKey(singleDict, "soCDV", model.DongVats.Count().ToString());



            AddKey(singleDict, "soDoiTruong", model.CanBos
               .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT1").ID).ToString());

            AddKey(singleDict, "soDoiPho", model.CanBos
                .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT2").ID).ToString());

            AddKey(singleDict, "soCBHL", model.CanBos
                .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT3").ID).ToString());

            AddKey(singleDict, "soCB", model.CanBos
                .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT4").ID).ToString());

            AddKey(singleDict, "soBSTY", model.CanBos
                .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT5").ID).ToString());

            AddKey(singleDict, "soCapDuong", model.CanBos
                .Count(n => n.IDChuyenMonKiThuat == model.ChuyenMonKiThuats.FirstOrNewObj(n => n.Code == "CMKT6").ID).ToString());

            ///
            AddKey(singleDict, "sodvCuuNan", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "TKCN").ID).ToString());

            AddKey(singleDict, "sodvMaTuy", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "MT").ID).ToString());

            AddKey(singleDict, "sodvThuocNo", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "TN").ID).ToString());

            AddKey(singleDict, "sodvGBMH", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "GBMH").ID).ToString());

            AddKey(singleDict, "sodvSinhSan", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "SS").ID
                || n.PhanLoaiDongVat == (int)PhanLoaiDongVat.ChoSinhSanCon
                || n.PhanLoaiDongVat == (int)PhanLoaiDongVat.ChoSinhSanBoMe
                ).ToString());

            AddKey(singleDict, "sodvBVTV", model.DongVats
                .Count(n => n.IDNghiepVuDongVat == model.NghiepVuDongVats.FirstOrNewObj(n => n.Code == "BVTTMH").ID).ToString());

            AddKey(singleDict, "sodvNhap", model.DongVats.Count(n => n.PhanLoaiDongVat == (int)PhanLoaiDongVat.ChoNhap).ToString());
            AddKey(singleDict, "sodvDuBi", model.DongVats.Count(n => n.PhanLoaiDongVat == (int)PhanLoaiDongVat.ChoDuBi).ToString());


            AddKey(singleDict, "cauTap", donVi.CauTap.ToString());
            AddKey(singleDict, "sanTap", donVi.SanTap.ToString());

            AddKey(singleDict, "nhaGb", donVi.NhaGb.ToString());
            AddKey(singleDict, "phuongTien", donVi.PhuongTien.ToString());

            var rand = DateTime.Now.Ticks.ToString();

            var rs = ExportUtils.ExportWord("mau xuat don vi.docx", singleDict, rand, tableDict);
            if (rs == null)
            {
                return NotFound();
            }
            else
            {
                var fileName = $"Báo cáo {donVi.Ten}.docx";
                byte[] byteArray = System.IO.File.ReadAllBytes(rs);
                ClearDirectory(rs, DateTime.Now.AddDays(1));
                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                return File(byteArray, contentType, fileName);
            }
        }


        public void ClearDirectory(string path, DateTime dayDelelete)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    foreach (var subDirectory in Directory.GetDirectories(path))
                    {
                        var d = new DirectoryInfo(subDirectory);
                        if (d.CreationTime < dayDelelete)
                            d.Delete(true);
                    }
                }
            }
            catch { }
        }
        public void AddKey(Dictionary<string, string> dic, string key, string val)
        {
            dic.Add($"{{{key}}}", val);
        }

        #endregion

    }
}
