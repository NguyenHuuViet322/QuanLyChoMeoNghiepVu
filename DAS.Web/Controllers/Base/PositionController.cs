using AutoMapper;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using DAS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PositionController : BaseController
    {
        #region Properties

        private readonly IPositionServices _positionService;
        private readonly IMapper _mapper;
        private readonly IExcelServices _excelService;
        #endregion Properties

        #region Ctor

        public PositionController(IMapper mapper
          , IPositionServices PositionService
            , IExcelServices excel)
        {
            _positionService = PositionService;
            _mapper = mapper;
            _excelService = excel;
        }

        #endregion Ctor

        #region List

        // GET: Positions
        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Index(PositionCondition condition)
        {
            var model = await _positionService.SearchByConditionPagging(condition);
            ViewBag.Keyword = condition.Keyword;
            return PartialView(model);
        }

        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Read })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchByCondition(PositionCondition condition)
        {
            ViewBag.Keyword = condition.Keyword;
            var model = await _positionService.SearchByConditionPagging(condition);
            return PartialView("Index_Positions", model);
        }

        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Read })]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var position = await _positionService.Get(id.Value);
            if (position == null)
                return NotFound();

            var vmPos = _mapper.Map<VMPosition>(position);
            ViewBag.Positions = await _positionService.GetPostionByTree(vmPos);
            return PartialView("Index_Detail", vmPos);
        }

        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Export })]
        [HttpGet]
        public async Task<IActionResult> Export(PositionCondition condition)
        {
            var list = await _positionService.GetListByCondition(condition,true);
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
                    new Col("ParentName"),
                },
                Headers = new List<Header>
                {
                    new Header("STT",8),
                    new Header("Mã chức vụ"),
                    new Header("Tên chức vụ"),
                    new Header("Mô tả",70),
                    new Header("Cấp trên"),
                }
            };
            var rs = await _excelService.ExportExcel(export, "Danh mục chức vụ");
            if (rs is ServiceResultError)
            {
                return NotFound();
            }
            else
            {
                var fileName = "DanhMucChucVu.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File((byte[])rs.Data, contentType, fileName);
            }
        }
        #endregion List

        #region Edit
        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Update })]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var position = await _positionService.Get(id.Value);
            if (position == null)
                return NotFound();

            var vmPos = _mapper.Map<VMPosition>(position);
            ViewBag.Positions = await _positionService.GetPostionByTree(vmPos);
            return PartialView("Index_Update", vmPos);
        }

        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Update })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VMPosition vMPosition)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _positionService.Update(vMPosition);
            return CustJSonResult(rs);
        }

        #endregion Edit

        #region Create
        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Create })]
        public async Task<IActionResult> Create()
        {
            var vmPosition = new VMPosition();
            ViewBag.Positions = await _positionService.GetPostionByTree(vmPosition);
            return PartialView("Index_Update", vmPosition);
        }


        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Create })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMPosition dt)
        {
            //Validate
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            //CallService
            var rs = await _positionService.Create(dt);
            return CustJSonResult(rs);
        }
        #endregion Create

        #region Delete
        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _positionService.Delete(id);
            return CustJSonResult(rs);
        }

        [HasPermission((int)EnumModule.Code.M20030, new int[] { (int)EnumPermission.Type.Deleted })]
        public async Task<IActionResult> Deletes(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return JSErrorResult("Vui lòng chọn chức vụ cần xoá!");
            var rs = await _positionService.Delete(ids);
            return CustJSonResult(rs);
        }

        #endregion Delete
         
    }
}