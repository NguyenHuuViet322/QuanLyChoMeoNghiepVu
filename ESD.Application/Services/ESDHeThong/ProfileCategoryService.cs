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
    public class ProfileCategoryService : BaseMasterService, IProfileCategoryServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IDefaultDataService _defaultDataService;
        private readonly ICacheManagementServices _cacheManagementServices;

        private readonly string[] _defaultCodes = new[] { "Name", "Code" };
        private class LinqCondParam
        {
            public int IDField { get; set; }
            public string FieldName { get; set; }
            public string Value { get; set; }
        }
        #endregion

        #region Ctor
        public ProfileCategoryService(IDasRepositoryWrapper dasRepository
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

        #region GetTree
        public async Task<VMTreeProfileCategory> GetTree(long id, int type)
        {
            var model = new VMTreeProfileCategory();
            model.Nodes = new List<VMNodeProfileCategory>();

            if (type == (int)EnumProfileCategory.NodeType.Category)
            {
                var categories = await GetCategories(EnumCategoryType.Code.DM_PhanLoaiHS.ToString(), id);
                if (categories.IsNotEmpty())
                    model.Nodes.AddRange(categories.Select(n => new VMNodeProfileCategory
                    {
                        ID = n.Key.ToString(),
                        Name = n.Value
                    }));

                var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(n => n.IDProfileCategory == id && n.Status == (int)EnumCataloging.Status.StorageApproved);
                if (profiles.IsNotEmpty())
                    model.Nodes.AddRange(profiles.Select(n => new VMNodeProfileCategory
                    {
                        ID = n.ID.ToString(),
                        Name = n.Title,
                        Type = (int)EnumProfileCategory.NodeType.Profile
                    }));
            }

            else if (type == (int)EnumProfileCategory.NodeType.Profile)
            {
                var docs = await GetCatalogingDocs(new DocBorrowCondition()
                {
                    IDProfile = (int)id
                });
                if (docs.IsNotEmpty())
                    model.Nodes.AddRange(docs.Select(n => new VMNodeProfileCategory
                    {
                        ID = n.ID.ToString(),
                        Name =   n.dictCodeValue.GetValueOrDefault("Subject"),
                        Type = (int)EnumProfileCategory.NodeType.Doc
                    }));

            }
            return model;
        }
        #endregion

        #region Privates

        private async Task<Dictionary<int, string>> GetCategories(string codeType, long parent)
        {
            var cates = (await _dasRepo.Category.GetAllListAsync(n =>
                codeType.IsNotEmpty() && n.CodeType == codeType && n.ParentId.GetValueOrDefault(0) == parent &&
                n.Status == (int)EnumCategory.Status.Active));
            // && n.IDOrgan == _userPrincipalService.IDOrgan   ));

            if (cates.Any(n => n.ParentId > 0))
            {
                //Render tree
                var treeModels = Utils.RenderTree(cates.Select(n => new TreeModel<VMPosition>
                {
                    ID = n.ID,
                    Name = n.Name,
                    Parent = n.ParentId ?? 0,
                    ParentPath = n.ParentPath ?? "0",
                }).ToList(), null, "--");
                return treeModels.OrderBy(n=>n.Name).ToDictionary(n => (int)n.ID, n => n.Name);
            }
            return cates.ToDictionary(n => n.ID, n => n.Name);
        }


        public async Task<List<VMCategoryTypeField>> GetCategoryTypeFields(VMCategoryType vmCategoryType)
        {
            if (Utils.IsEmpty(vmCategoryType))
                vmCategoryType = new VMCategoryType();

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            if (vmCategoryType.IsConfig > 0)
            {
                //Su dung config >  lấy cấu hình
                return await (from typeField in _dasRepo.CategoryTypeField.GetAll()
                              where typeField.IDCategoryType == vmCategoryType.ID
                              && (typeField.IDOrgan == userData.IDOrgan || typeField.IDOrgan == 0)
                              orderby typeField.Priority
                              select _mapper.Map<VMCategoryTypeField>(typeField)).ToListAsync();
            }
            else
            {
                var cateTypes = await _dasRepo.CategoryType.GetAllListAsync(n => n.Status == (int)EnumCategoryType.Status.Active && userData.IDOrgan == n.IDOrgan);
                var fields = _defaultDataService.GetDefaultCategoryFields(cateTypes, vmCategoryType.Name, vmCategoryType.Code);
                return _mapper.Map<List<VMCategoryTypeField>>(fields);
            }
        }

        private async Task<IEnumerable<VMCatalogingDoc>> GetCatalogingDocs(DocBorrowCondition condition)
        {
            //get list file
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.CatalogingDoc.GetAll()
                       join p in _dasRepo.CatalogingProfile.GetAll() on d.IDCatalogingProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDCatalogingDoc
                       where  d.Status == (int)EnumDocCollect.Status.Complete
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       &&
            (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword) || p.Title.Contains(condition.Keyword))
                           && (d.IDCatalogingProfile == condition.IDProfile || condition.IDProfile == 0)
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           p.FileCode,
                           p.Title
                       } into g
                       orderby g.Key.IDCatalogingProfile descending, g.Key.ID descending
                       select new VMCatalogingDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDCatalogingProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           ProfileCode = g.Key.FileCode,
                           ProfileName = g.Key.Title
                       };


            //nopaging
            var vmDocs = await temp.ToListAsync();
            var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var catalogingDocField = await GetCatalogingDocFieldsByIDs(vmDocs.Select(x => x.ID));
            foreach (var doc in vmDocs)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                //Dictionary
                doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
            }
            return new PaginatedList<VMCatalogingDoc>(vmDocs, vmDocs.Count, 1, vmDocs.Count);

        }
        private async Task<List<VMDocType>> GetDocTypes(IEnumerable<int> IDDocTypes)
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                             && IDDocTypes.Contains(dc.ID)
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }
        private async Task<List<VMDocTypeField>> GetDocTypeFields(IEnumerable<int> idDoctypes)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                             && idDoctypes.Contains(dtf.IDDocType)
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }
        private async Task<List<VMCatalogingDocField>> GetCatalogingDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                             && IDDocs.Contains(df.IDCatalogingDoc)
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }
        #endregion

    }
}