using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LanguageController : BaseController
    {
        #region Properties
        private readonly ILanguageServices _languageService;
        private readonly IExcelServices _excelService;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public LanguageController(ILanguageServices languageService, IMapper mapper, IExcelServices excel)
        {
            _languageService = languageService;
            _mapper = mapper;
            _excelService = excel;
        }
        #endregion

        #region List
        public async Task<IActionResult> Index(LanguageCondition condition)
        {
            var breadcrum = new Dictionary<string, string>
            {
                { "/Language", "Ngôn ngữ" }
            };
            ViewData["Breadcrumb"] = breadcrum;          
            ViewBag.Keyword = condition.Keyword;
            var model = await _languageService.SearchByConditionPagging(condition);
            return PartialView(model);
        }
        public async Task<IActionResult> SearchByConditionPagging(LanguageCondition condition)//[Bind("Keyword")] RoleCondition condition
        {
            ViewBag.Keyword = condition.Keyword;
            PaginatedList<VMLanguage> paging = await _languageService.SearchByConditionPagging(condition);
            return PartialView("Index_Language", paging);
        }
        public async Task<IActionResult> DetailPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _languageService.GetLanguageDetail(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("Index_Detail", model);
        }

        [HttpGet]
        public async Task<IActionResult> Export(LanguageCondition condition)
        {
            var list = await _languageService.GetListByCondition(condition);
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Name"),
                    new Col("Description"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Mã ngôn ngữ"),
                    new Header("Tên ngôn ngữ"),
                    new Header("Mô tả",70),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục ngôn ngữ");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucNN.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }

        public async Task<IActionResult> ExportNew(LanguageCondition condition)
        {
            var list = await _languageService.GetListByCondition(condition);
            if (list.Count() ==0)
            {
                return CustJSonResult(new ServiceResultError("Không có dữ liệu"));
            }
            var export = new ExportExtend
            {
                Data = list.Cast<dynamic>().ToList(),
                Cols = new List<Col>
                {
                    new Col{
                        DataType = 5
                    },
                    new Col("Code"),
                    new Col("Name"),
                    new Col("Description"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Mã ngôn ngữ"),
                    new Header("Tên ngôn ngữ"),
                    new Header("Mô tả",70),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục ngôn ngữ");
            if (rs is ServiceResultError)
            {
                return CustJSonResult(rs);
            }
            else
            {
                var fileName = "DanhMucNN.xlsx";
                var xx = new FileBinaryInfo {
                    FileContents = (byte[])rs.Data,
                    FileName = fileName
                };
                return new ObjectResult(xx);

            }
        }
        #endregion

        #region Edit
        public async Task<IActionResult> EditPopup(int? id)
        {
            if (id == null)
                return NotFound();
            var model = await _languageService.GetLanguage(id.Value);
            if (model == null)
                return NotFound();
            return PartialView("Index_Edit", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("ID, IDChannel, Code, Name, Description, Status")] VMLanguage vmRole)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _languageService.UpdateLanguage(vmRole);
            return CustJSonResult(rs);
        }
        #endregion

        #region Create
        public IActionResult CreatePopup()
        {
            VMLanguage vMLanguage = new VMLanguage();
            return PartialView("Index_Edit", vMLanguage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID, IDChannel, Code, Name, Description, Status")] VMLanguage vmLanguage)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var rs = await _languageService.CreateLanguage(vmLanguage);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _languageService.DeleteLanguage(id);
            return CustJSonResult(rs);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMulti(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn ngôn ngữ cần xóa");
            var rs = await _languageService.DeleteMultiLanguage(ids);
            return CustJSonResult(rs);
        }
        #endregion

    }
}
