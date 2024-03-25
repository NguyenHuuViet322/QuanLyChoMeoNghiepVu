using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Dynamic;
using ESD.Application.Enums;
using Microsoft.AspNetCore.Authorization;
using ESD.Application.Constants;
namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AutocompleteController : BaseController
    {
        #region Properties
        private readonly ICategoryServices _categoryServices;
        private readonly IProfileTemplateServices _profileTemplateServices;
        private readonly IProfileListService _profileListService;
        #endregion

        #region Ctor
        public AutocompleteController(ICategoryServices categoryServices
            , IProfileTemplateServices profileTemplateServices
            , IProfileListService profileListService){
            _categoryServices = categoryServices;
            _profileTemplateServices = profileTemplateServices;
            _profileListService = profileListService;
        }
        #endregion

        #region Get

        /// <summary> 
        /// Lấy option con khi đổi cha
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetChildOptions()
        {
            var codeType = Utils.GetString(DATA, "CodeType");
            var selectedId = Utils.GetInt(DATA, "SelectedID");
            var defaultText = Utils.GetString(DATA, "DefaultText");


            var html = string.Empty;
            var categories = new List<VMCategory>();
            if (codeType == EnumCategoryType.Code.DM_Kho.ToString())
            {
                //Get phông
                categories = (await _profileTemplateServices.GetProfileTemplateByStorage(new int[] { selectedId })).Select(n => new VMCategory { ID = n.ID, Name = n.FondName }).ToList();
            }

            else if (codeType == EnumCategoryType.Code.DM_Phong.ToString())
            {
                //Get mục lục
                categories = (await _profileListService.GetListByCondition(new ProfileListCondition() { ProfileTemplates = selectedId.ToString() })).Select(n => new VMCategory { ID = n.ID, Name = n.Name }).ToList();
            }

            //else if (codeType == EnumCategoryType.Code.DM_MucLuc.ToString())
            //{
            //    //Get hộp số
            //    categories = (await _categoryServices.GetByParent(EnumCategoryType.Code.DM_HopSo.ToString(), (int)EnumCategoryType.InputType.ProfileList, selectedId, 0)).Select(n => new VMCategory { ID = n.ID, Name = n.Name }).ToList();
            //}

            if (categories.IsNotEmpty())
                html = Utils.RenderOptions(categories, 0, true, defaultText, "");
            return new JsonResult(new
            {
                Type = CommonConst.Success,
                Data = html
            });
        }

        public async Task<IActionResult> GetStorageOptions()
        {
            var codeType = Utils.GetString(DATA, "CodeType");
            var selectedId = Utils.GetInt(DATA, "SelectedID");
            var defaultText = Utils.GetString(DATA, "DefaultText");

            var html = string.Empty;
            var categories = new List<VMCategory>();
            if (codeType.ToUpper() == EnumCategoryType.Code.DM_Kho.ToString().ToUpper())
            {

                var type = await _categoryServices.GetCategoryType(EnumCategoryType.Code.DM_Kho.ToString());
                //Get giá
                categories = (await _categoryServices.GetByParent(EnumCategoryType.Code.DM_Gia.ToString(), (int)EnumCategoryType.InputType.CategoryType, selectedId, type.ID)).Select(n => new VMCategory { ID = n.ID, Name = n.Name }).ToList();
            }

            else if (codeType.ToUpper() == EnumCategoryType.Code.DM_Gia.ToString().ToUpper())
            {
                var type = await _categoryServices.GetCategoryType(EnumCategoryType.Code.DM_Gia.ToString());

                //Hop/cap
                categories = (await _categoryServices.GetByParent(EnumCategoryType.Code.DM_HopSo.ToString(), (int)EnumCategoryType.InputType.CategoryType, selectedId, type.ID)).Select(n => new VMCategory { ID = n.ID, Name = n.Name }).ToList();
            }

            if (categories.IsNotEmpty())
                html = Utils.RenderOptions(categories, 0, true, defaultText, "");
            return new JsonResult(new
            {
                Type = CommonConst.Success,
                Data = html
            });
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
    }
}