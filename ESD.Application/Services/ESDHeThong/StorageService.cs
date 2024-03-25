using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Utility;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;
using ESD.Application.Constants;

namespace ESD.Application.Services
{
    public class StorageService : BaseMasterService, IStorageServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;

        #endregion

        #region Ctor
        public StorageService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService iUserPrincipalService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _userPrincipalService = iUserPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion

        #region Gets
        public async Task<IEnumerable<Storage>> Gets()
        {
            return await _dasRepo.Storage.GetAllListAsync();
        }

        public async Task<Storage> Get(object id)
        {
            return await _dasRepo.Storage.GetAsync(id);
        }

        //public async Task<IEnumerable<VMSearchProfileAndDoc>> SearchProfileByConditionPagging(SearchProfileCondition condition)
        //{
        //    var temp = from cp in _dasRepo.CatalogingProfile.GetAll()
        //           where (condition.Keyword.IsEmpty() || cp.Title.Contains(condition.Keyword)) && cp.Status == (int)EnumCataloging.Status.StorageApproved
        //           select new VMSearchProfileAndDoc()
        //           {
        //               Title = cp.Title,
        //               ID = cp.ID,
        //               IDPlanProfile = cp.IDPlanProfile
        //           };

        //    return result;
        //}

        public async Task<VMIndexProfileAndDoc> SearchProfileAndDocByConditionPaging(SearchProfileCondition condition)
        {
            VMIndexProfileAndDoc result = new VMIndexProfileAndDoc
            {
                //get list Doc
                VMPlanDoc = await GetPlanProfileAndDocs(condition),
                Condition = condition
            };
            if (!IsExisted(result.VMPlanDoc))
                return result;

            var ids = result.VMPlanDoc.Select(m => m.IDProfile).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var idDocTypelist = result.VMPlanDoc.Select(x => x.IDDocType).Distinct();
            if (!IsExisted(idDocTypelist))
                return result;
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(result.VMPlanDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in result.VMPlanDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDCatalogingDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            return result;
        }

        public async Task<VMIndexDocCatalogingProfile> PlanDocDetailIndex(SearchProfileCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocs(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDCatalogingDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocCatalogingProfile
            {
                VMUpdateCatalogingProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                SearchProfileCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes
            };
        }

        public async Task<VMIndexDocCatalogingProfile> PlanDocDetailIndexNoPaging(SearchProfileCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocsNoPaging(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDCatalogingDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault() == null ? "" : doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value);
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter -= listUnvalid.Count();
            }
            return new VMIndexDocCatalogingProfile
            {
                VMUpdateCatalogingProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                SearchProfileCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes
            };
        }

        public async Task<VMSearchProfileDoc> GetDocCollect(int IDDoc)
        {
            var temp = from doc in _dasRepo.CatalogingDoc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMSearchProfileDoc>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMDocTypes = await GetDocTypes();
            model.VMCatalogingProfile = await GetCatalogingProfile(model.IDCatalogingProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMDocFields = await GetDocFieldsByID(IDDoc);
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));

            return model;

        }
        #endregion

        #region Create

        #endregion Create

        #region Update

        #endregion Update

        #region Delete

        #endregion Delete

        #region Private methods
        private async Task<Dictionary<int, int>> GetTotalDocInProfiles(int[] ids)
        {
            //var idAgency = _userPrincipalService.IDAgency;
            //var tempStatistic = (await (from pp in _dasRepo.CatalogingProfile.GetAll()
            //                            join d in _dasRepo.Doc.GetAll() on pp.IDPlanProfile equals d.IDProfile into l
            //                            from ppl in l.DefaultIfEmpty()
            //                            where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.IDPlanProfile)) 
            //                            && ppl.Status != (int)EnumCommon.Status.InActive)
            //                            && (idAgency == 0 || (idAgency > 0 && idAgency == pp.IDAgency))
            //                            && pp.IDAgency == idAgency
            //                            group pp by pp.ID into g
            //                            select new
            //                            {
            //                                IDProfile = g.Key,
            //                                TotalDoc = g.Count()
            //                            }).ToListAsync()).ToDictionary(n => n.IDProfile, n => n.TotalDoc);
            //return tempStatistic;

            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       join d in _dasRepo.CatalogingDoc.GetAll() on pp.ID equals d.IDCatalogingProfile
                       where (ids.IsEmpty() || (ids.IsNotEmpty() && ids.Contains(pp.ID)) && d.Status != (int)EnumCommon.Status.InActive)
                       group new { pp, d } by new { pp.ID } into g
                       select new
                       {
                           IDProfile = g.Key.ID,
                           TotalDoc = g.Sum(x => x.d.Status == (int)EnumDocCollect.Status.Complete ? 1 : 0)
                       };
            return (await temp.ToListAsync()).ToDictionary(k => k.IDProfile, v => v.TotalDoc);
        }

        private async Task<List<VMDocType>> GetDocTypes()
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        private async Task<List<VMDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && df.IDCatalogingDoc == IDDoc
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocs(SearchProfileCondition condition)
        {
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
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && (condition.IDProfile <= 0 || d.IDCatalogingProfile == condition.IDProfile)
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var total = await temp.LongCountAsync();

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMPlanDoc>(docs, (int)total, condition.PageIndex, condition.PageSize);
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsNoPaging(SearchProfileCondition condition)
        {
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
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && (condition.IDProfile <= 0 || d.IDCatalogingProfile == condition.IDProfile)
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           d.Status,
                           d.CreatedBy,
                           d.CreateDate,
                           d.UpdatedDate,
                           d.UpdatedBy
                       } into g
                       orderby g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var total = await temp.LongCountAsync();
            var docs = await temp.ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, 0, condition.PageIndex, condition.PageSize);
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanProfileAndDocs(SearchProfileCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            //get list profile
            var tempProfile = from cp in _dasRepo.CatalogingProfile.GetAll()
                              where (condition.Keyword.IsEmpty() || cp.Title.Contains(condition.Keyword)
                              || cp.FileCode.Contains(condition.Keyword))
                              && cp.Status == (int)EnumCataloging.Status.StorageApproved
                              && cp.IDOrgan == userData.IDOrgan
                              //&& cp.IDAgency == 0   // only filter by IDOrgan
                              && (condition.IDStorage <= 0 || condition.IDStorage == cp.IDStorage)
                              && (condition.IDShelve <= 0 || condition.IDShelve == cp.IDShelve)
                              && (condition.IDBox <= 0 || condition.IDBox == cp.IDBox)
                              && (condition.IDProfileTemplate <= 0 || condition.IDProfileTemplate == cp.IDProfileTemplate)
                              orderby cp.ApprovedDate descending, cp.ID descending
                              select new VMPlanDoc()
                              {
                                  Title = cp.Title,
                                  IDProfile = cp.ID,
                                  FileCode = cp.FileCode,
                                  IDStorage = cp.IDStorage,
                                  IDShelve = cp.IDShelve,
                                  IDBox = cp.IDBox,
                                  IDProfileTemplate = cp.IDProfileTemplate
                              };
            var totalProfile = await tempProfile.LongCountAsync();

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
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       && p.IDOrgan == userData.IDOrgan && p.IDAgency == 0
                       && (condition.IDStorage <= 0 || condition.IDStorage == p.IDStorage)
                       && (condition.IDShelve <= 0 || condition.IDShelve == p.IDShelve)
                       && (condition.IDBox <= 0 || condition.IDBox == p.IDBox)
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           p.FileCode,
                           p.Title,
                           p.IDStorage,
                           p.IDShelve,
                           p.IDBox,
                           p.ApprovedDate,
                           p.IDProfileTemplate
                       } into g
                       orderby g.Key.ApprovedDate descending, g.Key.IDCatalogingProfile descending, g.Key.ID descending
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           FileCode = g.Key.FileCode,
                           Title = g.Key.Title,
                           IDStorage = g.Key.IDStorage,
                           IDShelve = g.Key.IDShelve,
                           IDBox = g.Key.IDBox,
                           IDProfileTemplate = g.Key.IDProfileTemplate
                       };
            var totalFile = string.IsNullOrEmpty(condition.Keyword) ? 0 : await temp.LongCountAsync();
            var total = totalProfile + totalFile;

            var totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var list = new List<VMPlanDoc>();
            var listToAdd = new List<VMPlanDoc>();
            if (totalProfile > condition.PageIndex * condition.PageSize)
            {
                list = await tempProfile.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            }
            else if (totalProfile < (condition.PageIndex - 1) * condition.PageSize)
            {
                if (!string.IsNullOrEmpty(condition.Keyword))
                {
                    int skip = (int)((condition.PageIndex - 1) * condition.PageSize - totalProfile);
                    listToAdd = await temp.Skip(skip).Take(condition.PageSize).ToListAsync();
                    list.AddRange(listToAdd);
                }
            }
            else
            {
                list = await tempProfile.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                if (!string.IsNullOrEmpty(condition.Keyword))
                {
                    //get rest of list
                    int take = (int)(condition.PageIndex * condition.PageSize - totalProfile);
                    listToAdd = await temp.Skip(0).Take(take).ToListAsync();
                    list.AddRange(listToAdd);
                }
            }

            if (!IsExisted(list))
                return null;

            var cateDict = new Dictionary<int, string>();
            var cates = await _dasRepo.Category.GetAllListAsync(c => c.Status == (int)EnumCommon.Status.Active
                            && (c.CodeType == "DM_Kho" || c.CodeType == "DM_Gia" || c.CodeType == "DM_HopSo"));
            if (IsExisted(cates))
            {
                cateDict = cates.ToDictionary(c => c.ID, c => c.Name);
                foreach (var item in list)
                {
                    item.StorageName = cateDict.GetValueOrDefault(item.IDStorage);
                    item.ShelveName = cateDict.GetValueOrDefault(item.IDShelve);
                    item.BoxName = cateDict.GetValueOrDefault(item.IDBox);
                }
            }

            var idProfileTemplates = list.GroupBy(m => m.IDProfileTemplate).Select(grp => grp.FirstOrDefault().IDProfileTemplate).ToList();

            var profileTemplateDict = new Dictionary<int, string>();
            var profileTemplate = await GetListProfileTemplate(idProfileTemplates);
            if (IsExisted(profileTemplate))
            {
                profileTemplateDict = profileTemplate.ToDictionary(c => c.ID, c => c.FondName);
                foreach (var item in list)
                {
                    item.ProfileTemplateName = profileTemplateDict.GetValueOrDefault(item.IDProfileTemplate);
                }
            }

            return new PaginatedList<VMPlanDoc>(list, (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<IEnumerable<ProfileTemplate>> GetListProfileTemplate(List<int> ids)
        {
            return await _dasRepo.ProfileTemplate.GetAllListAsync(c => c.Status == (int)EnumCommon.Status.Active && ( ids.IsEmpty() || ids.Contains(c.ID)));
        }

        private async Task<VMCatalogingProfile> GetCatalogingProfile(int idProfile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where (pp.Status == (int)EnumCataloging.Status.Active
                       || pp.Status == (int)EnumCataloging.Status.Approved
                       || pp.Status == (int)EnumCataloging.Status.StorageApproved)
                       && conditionStr && pp.ID == idProfile
                       select new VMCatalogingProfile
                       {
                           ID = pp.ID,
                           IDChannel = pp.IDChannel,
                           IDPlan = pp.IDPlan,
                           FileCode = pp.FileCode,
                           IDStorage = pp.IDStorage,
                           IDCodeBox = pp.IDCodeBox,
                           IDProfileList = pp.IDProfileList,
                           IDSecurityLevel = pp.IDSecurityLevel,
                           Identifier = pp.Identifier,
                           IDProfileTemplate = pp.IDProfileTemplate,
                           FileCatalog = pp.FileCatalog,
                           FileNotation = pp.FileNotation,
                           Title = pp.Title,
                           IDExpiryDate = pp.IDExpiryDate,
                           Rights = pp.Rights,
                           Language = pp.Language,
                           StartDate = pp.StartDate,
                           EndDate = pp.EndDate,
                           TotalDoc = pp.TotalDoc,
                           Description = pp.Description,
                           InforSign = pp.InforSign,
                           Keyword = pp.Keyword,
                           Maintenance = pp.Maintenance,
                           PageNumber = pp.PageNumber,
                           //Format = pp.Format,
                           Status = pp.Status,
                           IDAgency = pp.IDAgency,
                           CreateDate = pp.CreateDate,
                           ReasonToReject = pp.ReasonToReject
                       };

            return await temp.FirstOrDefaultAsync();
        }

        private async Task<VMUpdateCatalogingProfile> UpdatePlanProfile(int id)
        {
            var profile = await _dasRepo.CatalogingProfile.GetAsync(id) ?? new CatalogingProfile();
            var model = Utils.Bind<VMUpdateCatalogingProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            model.VMPlan = _mapper.Map<VMPlan>(await _dasRepo.Plan.GetAsync(profile.IDPlan) ?? new Plan());
            await GetUpdateModel(model);
            return model;
        }

        private async Task GetUpdateModel(VMUpdateCatalogingProfile model)
        {
            model.DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString());
            model.DictProfileTemplate = await GetDictProfileTemplate(model.IDProfileTemplate.GetValueOrDefault(0));
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictLangugage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString());
            model.DictExpiryDate = await GetDictExpiryDate();
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());
            model.DictSecurityLevel = (await _dasRepo.SercureLevel.GetAllListAsync(u => u.Status == (int)EnumSercureLevel.Status.Active)).ToDictionary(n => n.ID, n => n.Name);
            model.DictAgencies = await GetDictAgencies();
            if (string.IsNullOrEmpty(model.Language) || model.Language == "null")
                model.Language = string.Empty;
            else
            {
                if (model.Language.Contains("["))
                {
                    var language = JsonConvert.DeserializeObject<List<string>>(model.Language);
                    model.Language = string.Empty;
                    if (IsExisted(language))
                    {
                        foreach (var item in language)
                        {
                            model.Language += model.DictLangugage.GetValueOrDefault(int.Parse(item)) + ", ";
                        }
                        model.Language = model.Language[0..^2];
                    }
                }
                else
                    model.Language = model.DictLangugage.GetValueOrDefault(int.Parse(model.Language));
            }
            model.DictCates = new Dictionary<int, string>();
            var cates = await _dasRepo.Category.GetAllListAsync(c => c.Status == (int)EnumCommon.Status.Active
                            && (c.CodeType == "DM_Kho" || c.CodeType == "DM_Gia" || c.CodeType == "DM_HopSo"));

            if (IsExisted(cates))
            {
                model.DictCates = cates.ToDictionary(c => c.ID, c => c.Name);
            }
        }

        private async Task<Dictionary<int, string>> GetDictAgencies()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(n => n.Status == (int)EnumAgency.Status.Active
            && n.IDOrgan == userData.IDOrgan)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.IDOrgan == userData.IDOrgan && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active)).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a =>
            a.Status == (int)EnumOrgan.Status.Active
            //&& a.IDOrgan == _userPrincipalService.IDOrgan
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
            )).ToDictionary(n => n.ID, n => n.FondName);
        }

        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
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

        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        private async Task<List<VMDocField>> GetDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && IDDocs.Contains(df.IDCatalogingDoc)
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }
        #endregion Private methods

        #region Thow

        public Task<ServiceResult> Create(Storage model)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResult> Update(Storage model)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Portal
        public async Task<VMIndexProfileAndDoc> PortalSearch(SearchProfileCondition condition)
        {
            VMIndexProfileAndDoc result = new VMIndexProfileAndDoc
            {
                //get list Doc
                VMPlanDoc = await PortalGetPlanProfileAndDocs(condition)
            };
            if (!IsExisted(result.VMPlanDoc))
                return null;

            var ids = result.VMPlanDoc.Select(m => m.IDProfile).ToArray();
            result.TotalDocs = await GetTotalDocInProfiles(ids);
            var idDocTypelist = result.VMPlanDoc.Select(x => x.IDDocType).Distinct();
            if (!IsExisted(idDocTypelist))
                return result;
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(result.VMPlanDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in result.VMPlanDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDCatalogingDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            return result;
        }
        private async Task<PaginatedList<VMPlanDoc>> PortalGetPlanProfileAndDocs(SearchProfileCondition condition)
        {
            //get list profile
            var tempProfile = from cp in _dasRepo.CatalogingProfile.GetAll()
                              where (condition.Keyword.IsEmpty() || cp.Title.Contains(condition.Keyword)
                              || cp.FileCode.Contains(condition.Keyword))
                              && cp.Status == (int)EnumCataloging.Status.StorageApproved
                              //&& cp.IDOrgan == _userPrincipalService.IDOrgan
                              orderby cp.ID descending
                              select new VMPlanDoc()
                              {
                                  Title = cp.Title,
                                  IDProfile = cp.ID,
                                  FileCode = cp.FileCode
                              };
            var totalProfile = await tempProfile.LongCountAsync();

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
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       //&& p.IDOrgan == _userPrincipalService.IDOrgan
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
                       select new VMPlanDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           FileCode = g.Key.FileCode,
                           Title = g.Key.Title
                       };
            var totalFile = string.IsNullOrEmpty(condition.Keyword) ? 0 : await temp.LongCountAsync();
            var total = totalProfile + totalFile;

            var totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var list = new List<VMPlanDoc>();
            var listToAdd = new List<VMPlanDoc>();
            if (totalProfile > condition.PageIndex * condition.PageSize)
            {
                list = await tempProfile.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                return new PaginatedList<VMPlanDoc>(list, (int)total, condition.PageIndex, condition.PageSize);
            }
            else if (totalProfile < (condition.PageIndex - 1) * condition.PageSize)
            {
                if (!string.IsNullOrEmpty(condition.Keyword))
                {
                    int skip = (int)((condition.PageIndex - 1) * condition.PageSize - totalProfile);
                    listToAdd = await temp.Skip(skip).Take(condition.PageSize).ToListAsync();
                    list.AddRange(listToAdd);
                }

            }
            else
            {
                list = await tempProfile.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                if (!string.IsNullOrEmpty(condition.Keyword))
                {
                    //get rest of list
                    int take = (int)(condition.PageIndex * condition.PageSize - totalProfile);
                    listToAdd = await temp.Skip(0).Take(take).ToListAsync();
                    list.AddRange(listToAdd);
                }

            }
            //if (IsExisted(list))
            //    list = list.GroupBy(m => m.IDProfile).Select(x => x.First()).ToList();
            return new PaginatedList<VMPlanDoc>(list, (int)total, condition.PageIndex, condition.PageSize);
        }
        #endregion Portal

    }
}
