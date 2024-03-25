using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Dynamic;
using ESD.Application.Enums;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace DAS.Web.Controllers
{
    [Route("[controller]/{action=Index}/{CodeType?}")]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CategoryController : BaseController
    {
        #region Properties
        private readonly IExcelServices _excelService;
        private readonly ICategoryServices _categoryServices;
        #endregion
        #region Ctor
        public CategoryController(ICategoryServices categoryServices, IExcelServices excel)
        {
            _categoryServices = categoryServices;
            _excelService = excel;
        }
        #endregion

        #region List
        public async Task<IActionResult> Index(CategoryCondition condition)
        {
            ViewBag.CodeType = condition.CodeType;
            var model = await _categoryServices.SearchByConditionPagging(condition, DATA);
            model.VMCategoryType ??= new VMCategoryType();
            model.DataSearch = DATA;
     
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(CategoryCondition condition)
        {
            ViewBag.CodeType = condition.CodeType;
            var model = await _categoryServices.SearchByConditionPagging(condition, DATA);
            return PartialView("Index_Categories", model);
        }

        /// <summary>
        /// Dánh sách dm dùng cho cấu hình dm động
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetOptions()
        {
            var options = await _categoryServices.GetCategoryOptions(DATA);
            return JSSuccessResult(string.Empty, options);
        }
        #endregion


        #region Creates
        public async Task<IActionResult> Create(string CodeType)
        {
            var model = await _categoryServices.Create(CodeType);
            SetTitle("Thêm mới danh mục " + model.VMCategoryType.Name?.ToLower());
            return PartialView("Index_Update", model);
        }

        //POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _categoryServices.Create(DATA);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _categoryServices.Update(id);
            SetTitle("Cập nhật danh mục " + model.VMCategoryType.Name?.ToLower());
            return PartialView("Index_Update", model);
        }

        //POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit()
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _categoryServices.Update(DATA);
            return CustJSonResult(rs);
        }

        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            var model = await _categoryServices.Update(id);
            SetTitle("Chi tiết danh mục " + model.VMCategoryType.Name?.ToLower());
            return PartialView("Index_Detail", model);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _categoryServices.Delete(id);
            return CustJSonResult(rs);
        }

        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn danh mục cần xoá!");
            var rs = await _categoryServices.Delete(ids);
            return CustJSonResult(rs);
        }
        
        #endregion Delete

        #region Export
        [HttpGet]
        public async Task<IActionResult> Export(CategoryCondition condition)
        {

            condition.PageIndex = -1;
            var model = await _categoryServices.SearchByConditionPagging(condition, DATA);
            var header = new List<Header>
            {
                new Header("STT", 8)
            };
            var col = new List<Col>
            {
                new Col
                {
                    DataType = 5
                }
            };
            var gridFields = model.VMCategoryTypeFields.Where(n => n.IsShowGrid).OrderBy(n => n.Priority);

            foreach (var item in gridFields)
            {
                header.Add(new Header(item.Name));
                col.Add(new Col("Field" + item.ID.ToString()));
            }

            var data = new List<dynamic>();
            foreach (var cate in model.VMCategorys)
            {
                dynamic item = new ExpandoObject();
                var itemDic = (ICollection<KeyValuePair<string, object>>)item;
                var dict = new Dictionary<string, object>();
                foreach (var field in gridFields)
                {
                    var categoryField = cate.VMCategoryFields.FirstOrNewObj(n => n.IDCategoryTypeField == field.ID);
                    dict.Add("Field" + categoryField.IDCategoryTypeField.ToString(), categoryField.DisplayVal);
                    itemDic.Add(dict.LastOrDefault());
                }
                data.Add(item);
            }
            var export = new ExportExtend
            {
                Data = data,
                Cols = col,
                Headers = header,
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMuc" + (condition.CodeType ?? "") + ".xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion
    }
}