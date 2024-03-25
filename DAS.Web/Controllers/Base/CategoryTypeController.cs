using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{ 
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CategoryTypeController : BaseController
    {
        #region Properties

        private readonly ICategoryTypeServices _categoryTypeService;
        private readonly IMapper _mapper;

        #endregion Properties

        #region Ctor

        public CategoryTypeController(IMapper mapper
          , ICategoryTypeServices CategoryTypeService)
        {
            _categoryTypeService = CategoryTypeService;
            _mapper = mapper;
        }

        #endregion Ctor

        #region List

        // GET: CategoryTypeFields
        public async Task<IActionResult> Index(CategoryTypeCondition condition)
        {
            var model = await _categoryTypeService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(CategoryTypeCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _categoryTypeService.SearchByConditionPagging(condition);
            return PartialView("Index_CategoryTypes", model);
        }
        #endregion List

        #region Create
        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            var model = await _categoryTypeService.Create();
            ViewBag.InputTypes = model.DictInputTypes.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();
            ViewBag.DefaultValueTypes = model.DictDefaultValueTypes.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();
            ViewBag.CategoryTypes = model.DictCategoryTypes.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();
            ViewBag.Parents = model.DictParents.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();

            return PartialView("Index_Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMUpdateCategoryType vmCategoryType)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            if (!CheckCategoryTypeFieldValidate(vmCategoryType, out string mss, out List<object> errObj))
            {
                if (errObj.IsNotEmpty())
                    return JSErrorResult(mss, errObj);
                return JSErrorResult(mss);
            }
            //CallService
            var rs = await _categoryTypeService.Create(vmCategoryType);
            return CustJSonResult(rs);
        }

        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _categoryTypeService.Update(id);

            return PartialView("Index_Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VMUpdateCategoryType categoryType)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            if (!CheckCategoryTypeFieldValidate(categoryType, out string mss, out List<object> errObj))
            {
                if (errObj.IsNotEmpty())
                    return JSErrorResult(mss, errObj);
                return JSErrorResult(mss);
            }
            //CallService
            var rs = await _categoryTypeService.Update(categoryType);
            return CustJSonResult(rs);
        }
        #endregion

        #region Delete 
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _categoryTypeService.Delete(id);
            return CustJSonResult(rs);
        }

        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn loại danh mục cần xoá!");
            var rs = await _categoryTypeService.Delete(ids);
            return CustJSonResult(rs);
        }
        #endregion Delete

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            var model = await _categoryTypeService.Update(id);
            ViewBag.InputTypes = model.DictInputTypes.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();
            ViewBag.CategoryTypes = model.DictCategoryTypes.Select(n => new SelectListItem
            {
                Text = n.Value,
                Value = n.Key.ToString()
            }).ToList();
            return PartialView("Index_Detail", model);
        }

        #endregion

        #region Functions
        private bool CheckCategoryTypeFieldValidate(VMUpdateCategoryType vmCategoryType, out string mss, out List<object> errObj)
        {
            errObj = new List<object>();
            mss = string.Empty;
            if (vmCategoryType.CategoryTypeFields.IsNotEmpty())
            {
                var i = 0;
                foreach (var item in vmCategoryType.CategoryTypeFields)
                {
                    if (item.InputType == (int)EnumCategoryType.InputType.CategoryType && item.IDCategoryTypeRelated == 0)
                    {
                        errObj.Add(new
                        {
                            Field = $"CategoryTypeFields[{i}].IDCategoryTypeRelated",
                            Mss = "Danh mục không được để trống"
                        });
                    }
                    if (item.Minlenght > item.Maxlenght)
                    {
                        errObj.Add(new
                        {
                            Field = $"CategoryTypeFields[{i}].Maxlenght",
                            Mss = "Số ký tự tối đa phải lớn hơn Số ký tự tối thiểu"
                        });
                    }
                    if (item.MinValue > item.MaxValue)
                    {
                        errObj.Add(new
                        {
                            Field = $"CategoryTypeFields[{i}].MaxValue",
                            Mss = "Giá trị tối đa phải lớn hơn Giá trị tối thiểu"
                        });
                    }
                    i++;
                }

                var codes = vmCategoryType.CategoryTypeFields.Where(n => n.Code.IsNotEmpty()).Select(n => n.Code).ToList();
                if (codes.Count() > codes.Distinct().Count())
                {
                    mss = "Mã trường thông tin đã bị trùng";
                }
            }
            else
            {
                mss = "Cấu hình trường thông tin không được để trống";
            }
            return mss.IsEmpty() && errObj.IsEmpty();
        }
        #endregion
    }
}
