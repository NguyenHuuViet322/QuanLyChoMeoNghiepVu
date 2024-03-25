using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.LogUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;



namespace ESD.Application.Services
{
    public class CategoryService : BaseMasterService, ICategoryServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDefaultDataService _defaultDataService;
        protected ICacheManagementServices _cacheManagementServices;

        private readonly string[] _defaultCodes = new[] { "Name", "Code" };
        private class LinqCondParam
        {
            public int IDField { get; set; }
            public string FieldName { get; set; }
            public string Value { get; set; }
        }
        #endregion

        #region Ctor
        public CategoryService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService iUserPrincipalService
            , IDefaultDataService defaultDataService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = iUserPrincipalService;
            _defaultDataService = defaultDataService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion

        #region Get
        public async Task<IEnumerable<Category>> GetsActive(string codeType)
        {
            return await _dasRepo.Category.GetAll().Where(n => codeType.IsNotEmpty() && n.CodeType == codeType
            && n.Status == (int)EnumCategory.Status.Active
            && n.IDOrgan == _userPrincipalService.IDOrgan).OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetsActive(int idCategory)
        {
            return await _dasRepo.Category.GetAllListAsync(n => idCategory > 0
            && n.IdCategoryType == idCategory
            && n.Status == (int)EnumCategory.Status.Active
            && n.IDOrgan == _userPrincipalService.IDOrgan);
        }

        public async Task<VMCategoryType> GetCategoryType(string codeType)
        {
            return _mapper.Map<VMCategoryType>(await _dasRepo.CategoryType.FirstOrDefaultAsync(n => n.Code == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan)) ?? new VMCategoryType();
        }

        /// <summary>
        /// Lấy ds danh mục con
        /// </summary>
        /// <param name="selected">Danh mục cha</param>
        /// <param name="parentCategoryType">Loại danh mục cha</param>
        /// <param name="childCategoryType">Loại danh mục đích</param>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetByParent(int selected, int parentCategoryType, int childCategoryType)
        {
            var temp = from c in _dasRepo.Category.GetAll().Where(c => c.IDOrgan == _userPrincipalService.IDOrgan || c.IDOrgan == 0)
                       join cf in _dasRepo.CategoryField.GetAll() on c.ID equals cf.IDCategory
                       join ctf in _dasRepo.CategoryTypeField.GetAll() on cf.IDCategoryTypeField equals ctf.ID

                       where cf.IntVal == selected && c.IdCategoryType == childCategoryType && ctf.IDCategoryTypeRelated == parentCategoryType

                       select new Category
                       {
                           ID = c.ID,
                           Name = c.Name
                       };

            return await temp.ToListAsync();
        }

        /// <summary>
        /// Get theo cha
        /// </summary>
        /// <param name="codeType">Code type cần lấy</param>
        /// <param name="inputType">Kiểu nhập của trường theo cha cần lấy</param>
        /// <param name="parentId">ID cha</param>
        /// <param name="idCategoryTypeRelated">ID CategoryType nếu loại nhập là danh mục động</param>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetByParent(string codeType, int inputType, int parentId, int idCategoryTypeRelated)
        {
            var temp = from c in _dasRepo.Category.GetAll()
                       join cf in _dasRepo.CategoryField.GetAll() on c.ID equals cf.IDCategory
                       join ctf in _dasRepo.CategoryTypeField.GetAll() on cf.IDCategoryTypeField equals ctf.ID
                       where c.Status == (int)EnumCategory.Status.Active && c.CodeType == codeType
                       && ctf.InputType == inputType
                       && (ctf.IDCategoryTypeRelated == idCategoryTypeRelated || inputType != (int)EnumCategoryType.InputType.CategoryType)
                       && cf.IntVal == parentId
                       && c.IDOrgan == _userPrincipalService.IDOrgan
                       select new Category
                       {
                           ID = c.ID,
                           Name = c.Name
                       };

            return await temp.ToListAsync();
        }

        public async Task<VMIndexCategory> SearchByConditionPagging(CategoryCondition condition, Hashtable DATA)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var vmCategoryType = await GetCategoryType(condition.CodeType);
            var categoryTypeFields = await GetCategoryTypeFields(vmCategoryType) ?? new List<VMCategoryTypeField>();
            var searchFields = categoryTypeFields.Where(n => n.IsSearchGrid).ToList(); //Lấy field search

            //Tìm theo ID category theo câp cha (Cate
            if (condition.IDParentCategory > 0)
            {
                //var category = await _dasRepo.Category.Get
                var parentCategory = await _dasRepo.Category.GetAsync(condition.IDParentCategory) ?? new Category();
                var parentCategoryTypeField = categoryTypeFields.FirstOrNewObj(n => n.InputType == (int)EnumCategoryType.InputType.CategoryType && n.IDCategoryTypeRelated == parentCategory.IdCategoryType);
                searchFields.Add(parentCategoryTypeField);
                var field = "Field" + parentCategoryTypeField.ID;
                if (!DATA.ContainsKey(field))
                {
                    DATA.Add(field, condition.IDParentCategory);
                }
            }


            //Check có cấp cha con> phân cấp
            var isTreeView = await _dasRepo.Category.AnyAsync(n => n.ParentId > 0 && n.IdCategoryType == vmCategoryType.ID);
            if (isTreeView)
                condition.PageSize = -1; //Nopaging

            var conditionString = $"c.IdCategoryType = {vmCategoryType.ID} AND  c.IDOrgan = {_userPrincipalService.IDOrgan} ";

            if (DATA.ContainsKey("Name") || DATA.ContainsKey("Code"))
            {
                var name = Utils.GetString(DATA, "Name");
                if (name.IsNotEmpty())
                {
                    conditionString += $" AND c.Name LIKE N'%{name}%'";
                }
                var code = Utils.GetString(DATA, "Code");
                if (code.IsNotEmpty())
                {
                    conditionString += $" AND c.Code LIKE N'%{code}%'";
                }
                var parentId = Utils.GetInt(DATA, "ParentId");
                if (parentId > 0)
                {
                    conditionString += $" AND c.ParentId = {parentId}";
                }
            }
            var conditionCount = 0;

            //Get conditions
            var searchCond = new List<string>();
            foreach (var searchField in searchFields)
            {
                var rangeCond = new List<string>(); //Chứa các đk từ..đến

                var field = "Field" + searchField.ID;
                if (DATA.ContainsKey(field) || DATA.ContainsKey("From_" + field) || DATA.ContainsKey("To_" + field))
                {
                    var fieldName = "StringVal";
                    var val = Utils.GetString(DATA, field);
                    var valueSearch = $" LIKE N'%{val}%'";
                    if (searchField.InputType == (int)EnumCategoryType.InputType.InpDate)
                    {
                        //Các trường datetime/số thì phải gen ra Từ...Đến (Name input phải có Form_..., To_....)
                        fieldName = "DateTimeVal";
                        var fromD = Utils.GetDate(DATA, "From_" + field, searchField.Format ?? CommonConst.DfDateFormat);
                        var toD = Utils.GetDate(DATA, "To_" + field, searchField.Format ?? CommonConst.DfDateFormat);
                        if (fromD.HasValue)
                            rangeCond.Add($"cf.IDCategoryTypeField = {searchField.ID} AND cf.{fieldName} IS NOT NULL AND cf.{fieldName} >= '{Utils.DateToString(fromD, "yyyy-MM-dd")}' ");
                        if (toD.HasValue)
                            rangeCond.Add($"cf.IDCategoryTypeField = {searchField.ID} AND cf.{fieldName} IS NOT NULL AND cf.{fieldName} <= '{Utils.DateToString(toD, "yyyy-MM-dd")}' ");

                        if (rangeCond.IsNotEmpty())
                            searchCond.Add($"({string.Join(" AND ", rangeCond)})");
                    }

                    else if (searchField.InputType == (int)EnumCategoryType.InputType.InpFloat
                        || searchField.InputType == (int)EnumCategoryType.InputType.InpNumber
                        || searchField.InputType == (int)EnumCategoryType.InputType.InpMoney)
                    {
                        fieldName = GetFieldName(searchField);

                        var fromN = Utils.GetString(DATA, "From_" + field);
                        var toN = Utils.GetString(DATA, "To_" + field);
                        if (searchField.InputType == (int)EnumCategoryType.InputType.InpMoney)
                        {
                            fromN = fromN.Replace(",", string.Empty);
                            toN = toN.Replace(",", string.Empty);
                        }

                        if (fromN.IsNumber())
                            rangeCond.Add($"cf.IDCategoryTypeField = {searchField.ID} AND cf.{fieldName} IS NOT NULL AND cf.{fieldName} >=  {fromN} ");
                        if (toN.IsNumber())
                            rangeCond.Add($"cf.IDCategoryTypeField = {searchField.ID} AND cf.{fieldName} IS NOT NULL AND cf.{fieldName} <= {toN} ");

                        if (rangeCond.IsNotEmpty())
                            searchCond.Add($"({string.Join(" AND ", rangeCond)})");
                    }
                    else
                    {
                        if (val.IsNotEmpty())
                        {
                            switch (searchField.InputType)
                            {
                                case (int)EnumCategoryType.InputType.CategoryType:
                                case (int)EnumCategoryType.InputType.Agency:
                                //case (int)EnumCategoryType.InputType.ProfileList:
                                case (int)EnumCategoryType.InputType.ProfileTemplate:
                                    fieldName = "IntVal";
                                    valueSearch = $" = {val}";
                                    break;
                            }
                            searchCond.Add($"(cf.IDCategoryTypeField = {searchField.ID} AND cf.{fieldName} IS NOT NULL AND cf.{fieldName} {valueSearch} )");
                        }
                    }
                }
            }
            if (searchCond.IsNotEmpty())
            {
                conditionString += "AND " + string.Join(" OR ", searchCond);
                conditionCount = searchCond.Count;
            }

            var totalFilter = new SqlParameter
            {
                ParameterName = "@TotalFilter",
                Direction = ParameterDirection.Output,
                Value = 0
            };
            object[] parameters =
             {
                 new SqlParameter(  ){
                 ParameterName = "@ConditionString",
                 SqlDbType = SqlDbType.NVarChar,
                 Value = conditionString
                 },
                 new SqlParameter("@CountOfFieldSearch", conditionCount),
                 new SqlParameter("@PageNumber", condition.PageIndex),
                 new SqlParameter("@PageSize" ,condition.PageSize),
                 new SqlParameter("@Status", (int)EnumCategory.Status.Active),
                 totalFilter
             };

            var categories = await _dasRepo.Category.ExecuteStoreProc("EXECUTE [dbo].[GetDynamicCategoryPaging] @ConditionString, @CountOfFieldSearch, @PageNumber, @PageSize, @Status, @TotalFilter OUT", parameters).Select(n => _mapper.Map<VMCategory>(n)).ToListAsync();
            var total = Utils.IsNumber(totalFilter.Value?.ToString()) ? (int)totalFilter.Value : 0; //Lấy ra số bản ghi
            if (total == 0) //KO có kq return luôn
                return new VMIndexCategory()
                {
                    VMCategoryFields = new List<VMCategoryField>(),
                    VMCategoryType = vmCategoryType,
                    VMCategoryTypeFields = categoryTypeFields,
                    VMCategorys = new PaginatedList<VMCategory>(new List<VMCategory>(), (int)total, condition.PageIndex, condition.PageSize)
                };

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            //var cates = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var categoryFields = await GetCategoryFields(categoryTypeFields, vmCategoryType, categories);
            var gridFields = categoryTypeFields.Where(n => n.IsShowGrid);

            if (categories.IsNotEmpty())
            {
                foreach (var cate in categories)
                {
                    cate.VMCategoryFields = categoryFields.Where(n => n.IDCategory == cate.ID).ToList();
                    foreach (var field in gridFields)
                    {
                        var data = cate.VMCategoryFields.FirstOrNewObj(n => n.IDCategoryTypeField == field.ID);
                        data.DisplayVal = GetCategoryField(cate, field, data);
                        if (field.InputType == (int)EnumCategoryType.InputType.Parent)
                        {
                            data.IntVal = cate.ParentId;
                        }
                    }
                }
            }
            var categoreidSearch = new List<VMCategoryField>();

            foreach (var field in categoryTypeFields.Where(n => n.IsSearchGrid))
            {
                var fieldName = "Field" + field.ID;
                if (field.Code == "Name" || field.Code == "Code")
                {
                    fieldName = field.Code;
                }
                if (DATA.ContainsKey(fieldName))
                {
                    var val = Utils.GetString(DATA, fieldName);
                    var data = new VMCategoryField
                    {
                        IDCategoryTypeField = field.ID,
                        IntVal = Utils.GetBigInt(DATA, fieldName),
                        StringVal = Utils.GetString(DATA, fieldName),
                        FloatVal = Utils.GetFloat(DATA, fieldName),
                        DateTimeVal = Utils.GetDatetime(DATA, fieldName, field.Format),
                        IDOrgan = userData.IDOrgan,
                    };
                    data.DisplayVal = GetCategoryField(new VMCategory()
                    {
                        Code = Utils.GetString(DATA, "Code"),
                        Name = Utils.GetString(DATA, "Name"),
                        ParentId = Utils.GetIntNullable(DATA, "ParentId"),
                    }, field, data);
                    categoreidSearch.Add(data);
                }
            }

            var model = new VMIndexCategory()
            {
                VMCategoryFieldSearchs = categoreidSearch,
                VMCategoryFields = categoryFields,
                VMCategoryType = vmCategoryType,
                VMCategoryTypeFields = categoryTypeFields,
                VMCategoryTypes = await GetCategoryTypes()
            };

            if (isTreeView)
            {
                //Render tree
                var treeModels = Utils.RenderTree(categories.Select(n => new TreeModel<VMCategory>
                {
                    ID = n.ID,
                    Name = n.Name,
                    Parent = n.ParentId ?? 0,
                    ParentPath = n.ParentPath ?? "0",
                    Item = n
                }).ToList(), null);
                model.VMCategorys = new PaginatedList<VMCategory>(treeModels.Select(n => n.Item).ToList(), (int)total, condition.PageIndex, condition.PageSize);
            }
            else
            {
                model.VMCategorys = new PaginatedList<VMCategory>(categories, (int)total, condition.PageIndex, condition.PageSize);
            }
            return model;
        }

        public async Task<string> GetCategoryOptions(Hashtable DATA)
        {
            var options = string.Empty;
            var inputType = Utils.GetInt(DATA, "InputType");
            var categoryID = Utils.GetInt(DATA, "CategoryID");
            var categoryParents = Utils.GetString(DATA, "CategoryParents");
            var categoryTypeID = Utils.GetInt(DATA, "CategoryTypeID");
            var codeType = Utils.GetString(DATA, "CodeType");
            var selected = Utils.GetInt(DATA, "SelectedID");
            var defaultText = Utils.GetString(DATA, "DefaultText");
            var defaultValue = Utils.GetString(DATA, "DefaultValue");
            switch (inputType)
            {

                case (int)EnumCategoryType.InputType.Agency:
                    var agencies = await GetActiveAcgencies();
                    options = Utils.RenderOptions(agencies.OrderBy(n => n.Name), selected, true, defaultText, defaultValue);
                    break;
                //case (int)EnumCategoryType.InputType.ProfileList:
                //    var profileList = await _dasRepo.ProfileList.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active /*&& n.IDOrgan == _userPrincipalService.IDOrgan*/);
                //    profileList = profileList.Where(pl => pl.IDAgency == _userPrincipalService.IDAgency || pl.IDAgency == 0);
                //    options = Utils.RenderOptions(profileList.OrderBy(n => n.Name), selected, true, defaultText, defaultValue);
                //    break;

                case (int)EnumCategoryType.InputType.ProfileTemplate:
                    var profileTemplate = await _dasRepo.ProfileTemplate.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan);
                    //profileTemplate = profileTemplate.Where(pt => pt.IDOrgan == _userPrincipalService.IDOrgan || pt.IDOrgan == 0);
                    options = Utils.RenderOptions(profileTemplate.Select(n => new Category { ID = n.ID, Name = n.FondName }).OrderBy(n => n.Name), selected, true, defaultText, defaultValue);
                    break;

                //case (int)EnumCategoryType.InputType.CategoryType:
                default:
                    var categories = categoryTypeID == 0 && codeType.IsNotEmpty() ? await GetsActive(codeType) : await GetsActive(categoryTypeID);
                    if (inputType == (int)EnumCategoryType.InputType.Parent)
                    {
                        var prPath = $"{categoryParents}|{categoryID}";
                        if (categoryID > 0)
                            categories = categories.Where(n => n.ID != categoryID && n.ParentId != categoryID && (n.ParentPath ?? "0").IndexOf(prPath) == -1);
                        //Render tree
                        var treeModels = Utils.RenderTree(categories.Select(n => new TreeModel<VMPosition>
                        {
                            ID = n.ID,
                            Name = n.Name,
                            Parent = n.ParentId ?? 0,
                            ParentPath = n.ParentPath ?? "0",
                        }).ToList(), null, "--");


                        options = Utils.RenderOptions(treeModels, selected, true, defaultText, defaultValue);
                    }
                    else
                    {
                        options = Utils.RenderOptions(categories.OrderBy(n => n.Name), selected, true, defaultText, defaultValue);
                    }
                    break;
            }

            return options;
        }

        public async Task<VMIndexCategory> GetListByCondition(CategoryCondition condition, Hashtable DATA)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var vmCategoryType = await GetCategoryType(condition.CodeType);
            var categoryTypeFields = await GetCategoryTypeFields(vmCategoryType) ?? new List<VMCategoryTypeField>();

            var condParams = new List<LinqCondParam>();

            var searchFields = categoryTypeFields.Where(n => n.IsSearchGrid);
            foreach (var searchField in searchFields)
            {
                var field = "Field" + searchField.ID;
                if (DATA.ContainsKey(field))
                {
                    var val = Utils.GetString(DATA, field);
                    condParams.Add(new LinqCondParam
                    {
                        IDField = searchField.ID,
                        FieldName = GetFieldName(searchField),
                        Value = Utils.GetString(DATA, field)
                    });
                }
            }

            var temp = from cate in _dasRepo.Category.GetAll()
                       where cate.IdCategoryType == vmCategoryType.ID
                       && (condition.Name.IsEmpty() || cate.Name.Contains(condition.Name))
                       && cate.Status == (int)EnumCategory.Status.Active
                       && (cate.IDOrgan == _userPrincipalService.IDOrgan || cate.IDOrgan == 0)
                       select _mapper.Map<VMCategory>(cate);
            var cates = await temp.ToListAsync();
            var categoryFields = await GetCategoryFields(categoryTypeFields, vmCategoryType, cates);
            var gridFields = categoryTypeFields.Where(n => n.IsShowGrid);
            if (cates.IsNotEmpty())
            {
                foreach (var cate in cates)
                {
                    cate.VMCategoryFields = categoryFields.Where(n => n.IDCategory == cate.ID && (n.IDOrgan == userData.IDOrgan)).ToList();
                    foreach (var field in gridFields)
                    {
                        var data = cate.VMCategoryFields.FirstOrNewObj(n => n.IDCategoryTypeField == field.ID);
                        data.DisplayVal = GetCategoryField(cate, field, data);
                    }
                }
            }
            return new VMIndexCategory()
            {
                VMCategoryType = vmCategoryType,
                VMCategoryTypeFields = categoryTypeFields,
                VMCategorys = new PaginatedList<VMCategory>(cates, 0, condition.PageIndex, condition.PageSize)
            };

        }
        #endregion

        #region Create
        public async Task<VMCreateCategory> Create(string codeType)
        {
            var model = new VMCreateCategory();
            model.VMCategoryTypes = await GetCategoryTypes();
            model.VMCategoryType = model.VMCategoryTypes.FirstOrNewObj(n => n.Code == codeType);
            model.VMCategoryTypeFields = await GetCategoryTypeFields(model.VMCategoryType);
            model.CategoryFields = await GetCategoryFieldsDefault(model.VMCategoryTypeFields);
            model.IdCategoryType = model.VMCategoryType.ID;
            model.CodeType = model.VMCategoryType.Code;
            model.IDOrgan = _userPrincipalService.IDOrgan;
            return model;
        }

        /// <summary>
        /// Tạo dm với dữ liệu fix trước
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="dictCategoryTypeValues">Dict du lieu mac đinh (key: codeType, value: id category)</param>
        /// <returns></returns>
        public async Task<VMCreateCategory> Create(string codeType, Dictionary<string, int> dictCategoryTypeValues)
        {
            var model = new VMCreateCategory();
            model.VMCategoryTypes = await GetCategoryTypes();

            model.VMCategoryType = model.VMCategoryTypes.FirstOrNewObj(n => n.Code == codeType);
            model.VMCategoryTypeFields = await GetCategoryTypeFields(model.VMCategoryType);
            model.CategoryFields = await GetCategoryFieldsDefault(model.VMCategoryTypeFields);

            if (dictCategoryTypeValues.IsNotEmpty())
            {
                //Set du lieu mac dinh khi mo pop up theo dictCategoryTypeValues
                var categoryTypeValues = await _dasRepo.CategoryType.GetAllListAsync(ct => dictCategoryTypeValues.Select(n => n.Key).Contains(ct.Code.ToUpper()));
                var categoryValues = await _dasRepo.Category.GetAllListAsync(ct => dictCategoryTypeValues.Select(n => n.Value).Contains(ct.ID));
                foreach (var item in categoryTypeValues)
                {
                    var field = model.VMCategoryTypeFields.FirstOrDefault(n => n.InputType == (int)EnumCategoryType.InputType.CategoryType && n.IDCategoryTypeRelated == item.ID);
                    if (field != null)
                    {
                        var data = new VMCategoryField
                        {
                            IDCategoryTypeField = field.ID
                        };
                        var dfVal = dictCategoryTypeValues.GetValueOrDefault(item.Code.ToUpper());
                        var cate = categoryValues.FirstOrNewObj(n => n.ID == dfVal);
                        data.IntVal = cate.ID;
                        data.DisplayVal = cate.Name;
                        model.CategoryFields.Add(data);
                    }
                }
            }
            model.IdCategoryType = model.VMCategoryType.ID;
            model.CodeType = model.VMCategoryType.Code;
            model.IDOrgan = _userPrincipalService.IDOrgan;
            return model;
        }
        public async Task<ServiceResult> Create(Hashtable data)
        {
            var category = Utils.Bind<Category>(data);
            if (await _dasRepo.Category.IsCodeExist(category.Code, (int)EnumProfile.Status.Active, _userPrincipalService.IDOrgan))
            {
                return new ServiceResultError("Mã danh mục đã tồn tại");
            }
            category.IDOrgan = _userPrincipalService.IDOrgan;
            BindParentPath(category);
            await _dasRepo.Category.InsertAsync(category);
            await _dasRepo.SaveAync();

            var categoryFields = new List<CategoryField>();
            var categoryTypeFields = await (from typeField in _dasRepo.CategoryTypeField.GetAll()
                                            where typeField.IDCategoryType == category.IdCategoryType
                                            && (typeField.IDOrgan == _userPrincipalService.IDOrgan || typeField.IDOrgan == 0)
                                            orderby typeField.Priority
                                            select _mapper.Map<VMCategoryTypeField>(typeField)).ToListAsync();

            if (categoryTypeFields.IsNotEmpty())
            {
                foreach (var field in categoryTypeFields)
                {
                    if (_defaultCodes.Contains(field.Code))
                        continue;

                    var cateField = new CategoryField
                    {
                        IDCategory = category.ID,
                        IDCategoryTypeField = field.ID,
                        Status = 1,
                        CreatedBy = category.CreatedBy,
                        CreateDate = DateTime.Now,
                        IDOrgan = _userPrincipalService.IDOrgan,
                    };
                    BindCategoryField(cateField, field, data);
                    categoryFields.Add(cateField);
                }
            }
            if (categoryFields.IsNotEmpty())
            {
                await _dasRepo.CategoryField.InsertAsync(categoryFields);
                await _dasRepo.SaveAync();
            }

            //Trả data để fill vào dropdownlist
            return new ServiceResultSuccess("Thêm mới danh mục thành công", new
            {
                ID = category.ID,
                Name = category.Name
            });
        }

        #endregion

        #region Update
        public async Task<VMCreateCategory> Update(int? id)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var category = await _dasRepo.Category.GetAsync(id.Value);
            var model = _mapper.Map<VMCreateCategory>(category) ?? new VMCreateCategory();
            //model.VMCategoryType = await GetCategoryType(model.IdCategoryType);
            model.VMCategoryTypes = await GetCategoryTypes();
            model.VMCategoryType = model.VMCategoryTypes.FirstOrNewObj(n => n.ID == model.IdCategoryType);
            model.VMCategoryTypeFields = (await GetCategoryTypeFields(model.VMCategoryType)).OrderBy(n => n.Priority).ToList();
            model.CategoryFields = await GetCategoryFields(model.VMCategoryTypeFields, model.VMCategoryType, new List<VMCategory> { _mapper.Map<VMCategory>(category) });
            if (model.VMCategoryTypeFields.IsNotEmpty())
            {
                foreach (var field in model.VMCategoryTypeFields)
                {
                    var data = model.CategoryFields.FirstOrNewObj(n => n.IDCategoryTypeField == field.ID);
                    data.IDOrgan = userData.IDOrgan;
                    data.DisplayVal = GetCategoryField(_mapper.Map<VMCategory>(category), field, data);
                    if (field.InputType == (int)EnumCategoryType.InputType.Parent)
                    {
                        data.IntVal = category.ParentId;
                    }
                }
            }
            return model;
        }
        public async Task<ServiceResult> Update(Hashtable data)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var category = await _dasRepo.Category.GetAsync(Utils.GetInt(data, "ID"));
            category.Bind(data);
            if (await _dasRepo.Category.IsCodeExist(category.Code, (int)EnumProfile.Status.Active, userData.IDOrgan, category.ID))
            {
                return new ServiceResultError("Mã danh mục đã tồn tại");
            }
            BindParentPath(category);
            await _dasRepo.Category.UpdateAsync(category);
            await _dasRepo.SaveAync();

            var inserts = new List<CategoryField>();
            var updates = new List<CategoryField>();
            var deletes = new List<CategoryField>();

            var categoryTypeFields = await (from typeField in _dasRepo.CategoryTypeField.GetAll()
                                            where typeField.IDCategoryType == category.IdCategoryType
                                            && (typeField.IDOrgan == category.IDOrgan || typeField.IDOrgan == 0)
                                            orderby typeField.Priority
                                            select _mapper.Map<VMCategoryTypeField>(typeField)).ToListAsync();

            var categoryFields = (await (_dasRepo.CategoryField.GetAllListAsync(cateField => cateField.IDCategory == category.ID
            && (cateField.IDOrgan == userData.IDOrgan || cateField.IDOrgan == 0)))).ToList();

            if (categoryTypeFields.IsNotEmpty())
            {
                foreach (var field in categoryTypeFields)
                {
                    if (_defaultCodes.Contains(field.Code))
                        continue;

                    var cateField = categoryFields.FirstOrDefault(n => n.IDCategoryTypeField == field.ID);
                    if (Utils.IsEmpty(cateField))
                    {
                        cateField = new CategoryField
                        {
                            IDCategory = category.ID,
                            IDCategoryTypeField = field.ID,
                            Status = 1,
                            CreatedBy = category.CreatedBy,
                            CreateDate = DateTime.Now,
                            IDOrgan = userData.IDOrgan,
                        };
                        inserts.Add(cateField);
                    }
                    else
                    {
                        cateField.UpdatedBy = category.UpdatedBy;
                        cateField.UpdatedDate = DateTime.Now;
                        updates.Add(cateField);
                    }
                    BindCategoryField(cateField, field, data);
                }
            }
            if (Utils.IsNotEmpty(categoryFields))
            {
                var updateIds = updates.Select(n => n.ID).ToArray();
                //lấy ds các bản ghi đc thêm vào db nhưng ko có trong ds id trên form => xóa
                deletes = categoryFields.Where(n => !updateIds.Contains(n.ID)).ToList();
            }

            if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
            {
                if (deletes.IsNotEmpty())
                {
                    await _dasRepo.CategoryField.DeleteAsync(deletes);
                }
                if (updates.IsNotEmpty())
                {
                    await _dasRepo.CategoryField.UpdateAsync(updates);
                }
                if (inserts.IsNotEmpty())
                {
                    await _dasRepo.CategoryField.InsertAsync(inserts);
                }
                await _dasRepo.SaveAync();
            }
            return new ServiceResultSuccess("Cập nhật danh mục thành công");
        }
        #endregion

        #region Delete
        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var categoryDelete = await _dasRepo.Category.GetAsync(id);
                if (categoryDelete == null || categoryDelete.Status == (int)EnumCategory.Status.InActive || categoryDelete.IDOrgan != userData.IDOrgan)
                    return new ServiceResultError("Danh mục này hiện không tồn tại hoặc đã bị xóa");

                //Check sử dụng
                var dictNotDeleteCategory = await GetUsedCategoryInOtherCategory(new long[] { id });
                if (dictNotDeleteCategory.IsNotEmpty())
                    return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");

                if (categoryDelete.CodeType == EnumCategoryType.Code.DM_Kho.ToString())
                {
                    var isUsed = await _dasRepo.CatalogingProfile.AnyAsync(n => n.IDStorage == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (isUsed)
                        return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");

                    isUsed = await _dasRepo.PlanProfile.AnyAsync(n => n.IDStorage == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (isUsed)
                        return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");

                    isUsed = await _dasRepo.Profile.AnyAsync(n => n.IDStorage == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (isUsed)
                        return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");
                }
                else if (categoryDelete.CodeType == EnumCategoryType.Code.DM_HopSo.ToString())
                {
                    var isUsed = await _dasRepo.CatalogingProfile.AnyAsync(n => n.IDBox == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (isUsed)
                        return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");
                }
                else if (categoryDelete.CodeType == EnumCategoryType.Code.DM_Gia.ToString())
                {
                    var isUsed = await _dasRepo.CatalogingProfile.AnyAsync(n => n.IDShelve == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (isUsed)
                        return new ServiceResultError("Danh mục này hiện đang được sử dụng, không được phép xoá");
                }

                categoryDelete.Status = (int)EnumCategoryType.Status.InActive;
                await _dasRepo.Category.UpdateAsync(categoryDelete);
                var childs = await _dasRepo.CategoryField.GetAllListAsync(n => n.IDCategory == id && categoryDelete.IDOrgan == userData.IDOrgan);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumCategory.Status.InActive;
                }
                await _dasRepo.CategoryField.UpdateAsync(childs);
                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);

            }
        }

        public async Task<ServiceResult> Delete(IEnumerable<int> ids)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var categoryDeletes = await _dasRepo.Category.GetAllListAsync(n => ids.Contains(n.ID) && n.IDOrgan == userData.IDOrgan);
                if (categoryDeletes == null || categoryDeletes.Count() == 0)
                    return new ServiceResultError("Danh mục đã chọn hiện không tồn tại hoặc đã bị xóa");

                var dictNotDeleteCategory = await GetUsedCategoryInOtherCategory(ids.Select(n => (long)n).ToArray());
                if (dictNotDeleteCategory.IsNotEmpty())
                    return new ServiceResultError(string.Join(", ", dictNotDeleteCategory.Select(n => n.Value)) + " hiện đang được sử dụng, không được phép xoá");

                var planProfileUse = await _dasRepo.PlanProfile.GetAllListAsync(n => (ids.Contains(n.IDStorage) || ids.Contains(n.IDProfileCategory)) && n.Status != (int)EnumCommon.Status.InActive);
                if (planProfileUse.IsNotEmpty())
                {
                    var storageIds = planProfileUse.Select(n => n.IDStorage).Distinct().ToArray();
                    var profileCategoryIds = planProfileUse.Select(n => n.IDProfileCategory).Distinct().ToArray();
                    var deletedNames = categoryDeletes.Where(m => storageIds.Contains(m.ID) || profileCategoryIds.Contains(m.ID)).Select(n => n.Name);
                    return new ServiceResultError("Danh mục " + string.Join(", ", deletedNames) + " hiện đang được sử dụng cho hồ sơ thu thập, không được phép xoá");
                }

                var catalogingProfileUse = await _dasRepo.CatalogingProfile.GetAllListAsync(n => (ids.Contains(n.IDStorage) || ids.Contains(n.IDBox) || ids.Contains(n.IDShelve) || ids.Contains(n.IDProfileCategory)) && n.Status != (int)EnumCommon.Status.InActive);
                if (catalogingProfileUse.IsNotEmpty())
                {
                    var storageIds = catalogingProfileUse.Select(n => n.IDStorage).Distinct().ToArray();
                    var boxIds = catalogingProfileUse.Select(n => n.IDBox).Distinct().ToArray();
                    var shelveIds = catalogingProfileUse.Select(n => n.IDShelve).Distinct().ToArray();
                    var profileCategoryIds = catalogingProfileUse.Select(n => n.IDProfileCategory).Distinct().ToArray();
                    var deletedNames = categoryDeletes.Where(m => storageIds.Contains(m.ID) || boxIds.Contains(m.ID) || shelveIds.Contains(m.ID) || profileCategoryIds.Contains(m.ID)).Select(n => n.Name);
                    return new ServiceResultError("Danh mục " + string.Join(", ", deletedNames) + " hiện đang được sử dụng cho hồ sơ biên mục, không được phép xoá");
                }


                var code = categoryDeletes.FirstOrNewObj().CodeType;
                if (code == EnumCategoryType.Code.DM_Kho.ToString())
                {
                    var categoryUsed = await _dasRepo.Profile.GetAllListAsync(n => ids.Contains(n.IDStorage) && n.Status != (int)EnumCommon.Status.InActive);
                    if (categoryUsed.IsNotEmpty())
                    {
                        var usedIds = categoryUsed.Select(n => n.IDStorage).Distinct().ToArray();
                        var deletedNames = categoryDeletes.Where(m => usedIds.Contains(m.ID)).Select(n => n.Name);
                        return new ServiceResultError("Danh mục " + string.Join(", ", deletedNames) + " hiện đang được sử dụng, không được phép xoá");
                    }
                }

                foreach (var pos in categoryDeletes)
                {
                    pos.Status = (int)EnumCategory.Status.InActive;
                }
                await _dasRepo.Category.UpdateAsync(categoryDeletes);

                var childs = await _dasRepo.CategoryField.GetAllListAsync(n => ids.Contains(n.IDCategory) && n.IDOrgan == userData.IDOrgan);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumCategory.Status.InActive;
                }
                await _dasRepo.CategoryField.UpdateAsync(childs);

                await _dasRepo.SaveAync();
                return new ServiceResultSuccess("Xóa danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion

        #region Privates
        private void BindCategoryField(CategoryField cateField, VMCategoryTypeField field, Hashtable data)
        {
            var fieldName = "Field" + field.ID;
            switch (field.InputType)
            {
                case (int)EnumCategoryType.InputType.InpNumber:
                case (int)EnumCategoryType.InputType.CategoryType:
                case (int)EnumCategoryType.InputType.Agency:
                //case (int)EnumCategoryType.InputType.ProfileList:
                case (int)EnumCategoryType.InputType.ProfileTemplate:
                    cateField.IntVal = Utils.GetBigInt(data, fieldName);
                    break;

                case (int)EnumCategoryType.InputType.InpFloat:
                case (int)EnumCategoryType.InputType.InpMoney:
                    cateField.FloatVal = Utils.GetFloat(data, fieldName);
                    break;

                case (int)EnumCategoryType.InputType.InpText:
                case (int)EnumCategoryType.InputType.InpTextArea:
                    cateField.StringVal = Utils.GetString(data, fieldName);
                    break;
                case (int)EnumCategoryType.InputType.InpDate:
                    cateField.DateTimeVal = Utils.GetDatetime(data, fieldName, field.Format ?? "dd/MM/yyyy");
                    break;
            }
        }
        private string GetFieldName(VMCategoryTypeField field)
        {
            switch (field.InputType)
            {
                case (int)EnumCategoryType.InputType.InpNumber:
                case (int)EnumCategoryType.InputType.CategoryType:
                case (int)EnumCategoryType.InputType.Agency:
                //case (int)EnumCategoryType.InputType.ProfileList:
                case (int)EnumCategoryType.InputType.ProfileTemplate:
                    return "IntVal";

                case (int)EnumCategoryType.InputType.InpFloat:
                case (int)EnumCategoryType.InputType.InpMoney:
                    return "FloatVal";

                //case (int)EnumCategoryType.InputType.InpText:
                //case (int)EnumCategoryType.InputType.InpTextArea:
                //    return "StringVal";

                case (int)EnumCategoryType.InputType.InpDate:
                    return "DateTimeVal";
            }
            return "StringVal";
        }
        private string GetCategoryField(VMCategory cate, VMCategoryTypeField field, VMCategoryField data)
        {
            if (field.Code == "Code")
                return cate.Code;

            if (field.Code == "Name")
                return cate.Name;

            switch (field.InputType)
            {
                case (int)EnumCategoryType.InputType.InpNumber:
                    return data.IntVal.HasValue ? data.IntVal.ToString() : string.Empty;

                case (int)EnumCategoryType.InputType.Parent:
                    {
                        if (cate.ParentId.HasValue)
                        {
                            var cateSelected = _dasRepo.Category.Get((int)cate.ParentId.Value) ?? new Category();
                            return cateSelected.Name ?? string.Empty;
                        }
                    }
                    break;

                case (int)EnumCategoryType.InputType.CategoryType:
                    {
                        if (data.IntVal.HasValue)
                        {
                            var cateSelected = _dasRepo.Category.Get((int)data.IntVal.Value) ?? new Category();
                            return cateSelected.Name ?? string.Empty;
                        }
                    }
                    break;

                case (int)EnumCategoryType.InputType.Agency:
                    {
                        if (data.IntVal.HasValue)
                        {
                            var cateSelected = _dasRepo.Agency.Get((int)data.IntVal.Value) ?? new Agency();
                            return cateSelected.Name ?? string.Empty;
                        }
                    }
                    break;

                //case (int)EnumCategoryType.InputType.ProfileList:
                //    {
                //        if (data.IntVal.HasValue)
                //        {
                //            var cateSelected = _dasRepo.ProfileList.Get((int)data.IntVal.Value) ?? new ProfileList();
                //            return cateSelected.Name ?? string.Empty;
                //        }
                //    }
                //    break;
                case (int)EnumCategoryType.InputType.ProfileTemplate:
                    {
                        if (data.IntVal.HasValue)
                        {
                            var cateSelected = _dasRepo.ProfileTemplate.Get((int)data.IntVal.Value) ?? new ProfileTemplate();
                            return cateSelected.FondName ?? string.Empty;
                        }
                    }
                    break;

                case (int)EnumCategoryType.InputType.InpFloat:
                    return data.FloatVal.HasValue ? data.FloatVal.Value.ToString() : string.Empty;


                case (int)EnumCategoryType.InputType.InpMoney:
                    return data.FloatVal.MoneyDisplay();

                case (int)EnumCategoryType.InputType.InpText:
                case (int)EnumCategoryType.InputType.InpTextArea:
                    return data.StringVal;

                case (int)EnumCategoryType.InputType.InpDate:
                    return Utils.DateToString(data.DateTimeVal, field.Format.IsNotEmpty() ? field.Format : CommonConst.DfDateFormat);
            }

            return string.Empty;
        }

        private async Task<IEnumerable<VMCategoryType>> GetCategoryTypes()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return await (from type in _dasRepo.CategoryType.GetAll()
                          where type.Status == (int)EnumCategoryType.Status.Active && (type.IDOrgan == userData.IDOrgan)
                          select _mapper.Map<VMCategoryType>(type)).ToListAsync();
        }
        private async Task<List<VMCategoryField>> GetCategoryFields(List<VMCategoryTypeField> categoryTypeFields, VMCategoryType vmCategoryType, List<VMCategory> categories)
        {
            if (Utils.IsNotEmpty(categoryTypeFields) && Utils.IsNotEmpty(vmCategoryType) && categories.IsNotEmpty())
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

                var vmCategoryFields = new List<VMCategoryField>();
                var idCategories = categories.Select(n => n.ID).ToArray();
                if (vmCategoryType.IsConfig > 0 && categories.IsNotEmpty())
                {
                    //Nếu có cấu hình
                    vmCategoryFields = await (from cateField in _dasRepo.CategoryField.GetAll()
                                              where idCategories.IsNotEmpty() && idCategories.Contains(cateField.IDCategory)
                                              select _mapper.Map<VMCategoryField>(cateField)).ToListAsync();
                }
                //tạo dữ liệu ảo (name và code)
                foreach (var category in categories)
                {
                    vmCategoryFields.Add(new VMCategoryField
                    {
                        IDCategory = category.ID,
                        IDCategoryTypeField = categoryTypeFields.FirstOrNewObj(n => n.Code == "Name").ID,
                        StringVal = category.Name,
                        IDOrgan = userData.IDOrgan,
                    });
                    vmCategoryFields.Add(new VMCategoryField
                    {
                        IDCategory = category.ID,
                        IDCategoryTypeField = categoryTypeFields.FirstOrNewObj(n => n.Code == "Code").ID,
                        StringVal = category.Code,
                        IDOrgan = userData.IDOrgan,
                    });
                }

                return vmCategoryFields;
            }
            return new List<VMCategoryField>();
        }
        private async Task<List<VMCategoryField>> GetCategoryFieldsDefault(List<VMCategoryTypeField> categoryTypeFields)
        {

            var vmCategoryFields = new List<VMCategoryField>();
            if (categoryTypeFields.IsEmpty())
                return vmCategoryFields;

            var fieldHasDefaultValues = categoryTypeFields.Where(n => n.DefaultValueType > 0);
            var idCUser = _userPrincipalService.UserId;
            var cUser = await _dasRepo.User.GetAsync(idCUser);

            var val = 0;
            var text = string.Empty;
            foreach (var field in fieldHasDefaultValues)
            {
                val = 0;
                text = string.Empty;
                switch (field.DefaultValueType)
                {
                    case (int)EnumCategoryType.DefaultValue.ByUser:
                        if (field.InputType == (int)EnumCategoryType.InputType.Agency)
                        {
                            val = cUser.IDAgency;
                            text = (await _dasRepo.Agency.GetAsync(val) ?? new Agency()).Name;
                        }
                        else if (field.InputType == (int)EnumCategoryType.InputType.InpText)
                        {
                            text = cUser.Name;
                        }
                        break;
                    case (int)EnumCategoryType.DefaultValue.DateTimeNow:
                        text = Utils.DateToString(DateTime.Now, field.Format ?? CommonConst.DfDateFormat);
                        break;
                }
                vmCategoryFields.Add(new VMCategoryField
                {
                    IDCategory = 0,
                    IDCategoryTypeField = field.ID,
                    IntVal = val,
                    DisplayVal = text,
                });

            }


            return vmCategoryFields;
        }

        private async Task<IEnumerable<Agency>> GetActiveAcgencies()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(n => n.Status == (int)EnumAgency.Status.Active && n.IDOrgan == userData.IDOrgan));
        }
        public async Task<List<VMCategoryTypeField>> GetCategoryTypeFields(VMCategoryType vmCategoryType)
        {
            if (Utils.IsEmpty(vmCategoryType))
                vmCategoryType = new VMCategoryType();

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            if (vmCategoryType.IsConfig > 0)
                //Su dung config >  lấy cấu hình
                return await (from typeField in _dasRepo.CategoryTypeField.GetAll()
                              where typeField.IDCategoryType == vmCategoryType.ID
                              && (typeField.IDOrgan == userData.IDOrgan || typeField.IDOrgan == 0)
                              orderby typeField.Priority
                              select _mapper.Map<VMCategoryTypeField>(typeField)).ToListAsync();

            var cateTypes = await _dasRepo.CategoryType.GetAllListAsync(n => n.Status == (int)EnumCategoryType.Status.Active && userData.IDOrgan == n.IDOrgan);
            var fields = _defaultDataService.GetDefaultCategoryFields(cateTypes, vmCategoryType.Name, vmCategoryType.Code);
            return _mapper.Map<List<VMCategoryTypeField>>(fields);
        }

        /// <summary>
        /// Lấy dm đã được sd ở các dm khác
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<int, string>> GetUsedCategoryInOtherCategory(long[] ids)
        {
            var temp = from c in _dasRepo.Category.GetAll().Where(n => n.Status == (int)EnumCategory.Status.Active)
                       join cf in _dasRepo.CategoryField.GetAll() on c.ID equals cf.IDCategory
                       join ctf in _dasRepo.CategoryTypeField.GetAll().Where(n => n.InputType == (int)EnumCategoryType.InputType.CategoryType) on cf.IDCategoryTypeField equals ctf.ID
                       join cr in _dasRepo.Category.GetAll() on cf.IntVal equals cr.ID

                       where ids.IsNotEmpty() && ids.Contains(cf.IntVal.GetValueOrDefault(0))
                       group cr by new { cr.ID, cr.Name } into g

                       select new Category
                       {
                           ID = g.Key.ID,
                           Name = g.Key.Name
                       };
            var referCates = await temp.ToListAsync() ?? new List<Category>();
            //Check dm đc làm cha

            var childCates = await _dasRepo.Category.GetAllListAsync(n => n.Status == (int)EnumCategory.Status.Active && ids.IsNotEmpty() && ids.Contains(n.ParentId ?? 0));
            if (childCates.IsNotEmpty())
                referCates.AddRange(childCates);

            return referCates.ToDictionary(n => n.ID, n => n.Name);
        }

        /// <summary>
        /// Cập nhật parent path 
        /// </summary>
        /// <param name="category"></param>
        private void BindParentPath(Category category)
        {
            category.ParentPath = "0";
            if (category.ParentId > 0)
            {
                var cate = _dasRepo.Category.Get(category.ParentId);
                if (cate != null)
                    category.ParentPath = (cate.ParentPath.IsEmpty() ? "0" : cate.ParentPath) + "|" + cate.ID;
            }
        }


        #endregion

    }
}