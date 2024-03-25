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
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneAPI;
using OneAPI.iOneSDK;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;

namespace ESD.Application.Services
{
    public class PlanDocService : BaseMasterService, IPlanDocServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IStgFileClientService _fileClientService;
        private readonly ICacheManagementServices _cacheManagementServices;
        protected string ioneSvAddress = ConfigUtils.GetKeyValue("IOneConfigs", "Address") ?? "localhost";

        private class LinqCondParam
        {
            public int IDField { get; set; }
            public string FieldName { get; set; }
            public string Value { get; set; }
        }
        #endregion

        #region Ctor
        public PlanDocService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager loggerManager
            , IUserPrincipalService userPrincipalService
            , IStgFileClientService stgFileClientService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _userPrincipalService = userPrincipalService;
            _fileClientService = stgFileClientService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion Ctor

        #region Get
        /// <summary>
        /// Lấy Index cho màn hình các thành phần hồ sơ
        /// Lấy Plan, hồ sơ cho Breadcrumb
        /// Lấy list Doc
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<VMIndexDocPlan> PlanDocDetailIndex(PlanDocCondition condition, bool isExport = false)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocs(condition, isExport);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
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
            //var profile = _mapper.Map<VMPlanProfile>(await _dasRepo.PlanProfile.GetAsync(condition.IDProfile) ?? new PlanProfile());
            var profile = (await GetPlanProfile(condition.IDProfile)) ?? new VMPlanProfile();
            return new VMIndexDocPlan
            {
                VMPlanProfile = profile,
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                DictAgencies = await GetDictAgencies(),
                DictProfileTemplate = await GetDictProfileTemplate(profile.IDProfileTemplate),
                DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString()),
                DictLanguage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString()),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
                DictUsers = await GetDictUsers()
            };
        }

        public async Task<VMIndexDocPlan> PlanDocDetailIndexNoPaging(PlanDocCondition condition)
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
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
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
            return new VMIndexDocPlan
            {
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMIndexDocPlan> PlanDocDetailIndexListApprovedNoPaging(PlanDocCondition condition)
        {
            //lấy list Doc
            var pagDoc = await GetPlanDocsListApprovedNoPaging(condition);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
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
            return new VMIndexDocPlan
            {
                VMUpdatePlanProfile = await UpdatePlanProfile(condition.IDProfile),
                vMPlanDocs = pagDoc,
                DictExpiryDate = await GetDictExpiryDate(),
                PlanDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };
        }

        public async Task<VMDocCreate> GetDocCollect(int IDDoc)
        {
            var temp = from doc in _dasRepo.Doc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMDocTypes = await GetDocTypes();
            model.VMPlanProfile = await GetPlanProfile(model.IDProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMDocFields = await GetDocFieldsByID(IDDoc);
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));

            return model;

        }
        #endregion Get

        #region Create 
        public async Task<VMDocCreate> CreateDocCollect(int IDProfile, int IDDocType = 0)
        {

            var model = new VMDocCreate();
            model.IDProfile = IDProfile;
            model.VMDocTypes = await GetDocTypes();
            model.VMPlanProfile = await GetPlanProfile(IDProfile);
            //Lấy doctype type là doc
            int idType1 = IDDocType;
            if (IDDocType == 0)
            {
                idType1 = model.VMDocTypes.Where(x => x.IsBase == true && x.Type == 1).FirstOrDefault().ID;
                model.IDDocType = idType1;
                model.VMDocTypeFields = await GetDocTypeFields(idType1);
            }
            else
            {
                model.IDDocType = IDDocType;
                model.VMDocTypeFields = await GetDocTypeFields(IDDocType);
            }


            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == idType1);
            model.VMDocFields = await GetDocFieldsDefault(model.VMDocTypeFields, model.VMPlanProfile, model.VMDocType);
            return model;
        }

        public async Task<ServiceResult> Create(Hashtable data, bool isComplete)
        {
            var doc = Utils.Bind<Doc>(data);
            if (isComplete)
            {
                doc.Status = (int)EnumDocCollect.Status.Complete;
            }
            else
            {
                doc.Status = (int)EnumDocCollect.Status.Active;
            }
            await _dasRepo.Doc.InsertAsync(doc);
            await _dasRepo.SaveAync();

            await UpdateProfileTotalDoc(doc.IDProfile);

            var docFields = new List<DocField>();
            var docTypeFields = await (from dtf in _dasRepo.DocTypeField.GetAll()
                                       where dtf.IDDocType == doc.IDDocType
                                       orderby dtf.Priority
                                       select _mapper.Map<VMDocTypeField>(dtf)).ToListAsync();
            if (docTypeFields.IsNotEmpty())
            {
                foreach (var field in docTypeFields)
                {
                    var docField = new DocField
                    {
                        IDDoc = doc.ID,
                        IDDocTypeField = field.ID,
                        Status = (int)EnumDocCollect.Status.Active,
                        CreatedBy = doc.CreatedBy,
                        CreateDate = DateTime.Now,
                    };
                    BindDocField(docField, field, data);
                    docFields.Add(docField);
                }
            }
            if (docFields.IsNotEmpty())
            {
                await _dasRepo.DocField.InsertAsync(docFields);
                await _dasRepo.SaveAync();
            }
            //update status Profile
            var profile = await _dasRepo.PlanProfile.GetAsync(doc.IDProfile);
            if (profile.Status != (int)EnumProfilePlan.Status.Reject)
            {
                if (isComplete)
                {
                    var catalogingdocs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == profile.ID && x.Status == (int)EnumDocCollect.Status.Active);
                    if ((catalogingdocs == null || catalogingdocs.Count() == 0) && profile.Status != (int)EnumProfilePlan.Status.Reject)
                    {
                        profile.Status = (int)EnumProfilePlan.Status.CollectComplete;
                        await _dasRepo.PlanProfile.UpdateAsync(profile);
                        await _dasRepo.SaveAync();
                    }
                }
                else
                {
                    profile.Status = (int)EnumProfilePlan.Status.Active;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                }

            }
            return new ServiceResultSuccess("Thêm mới tài liệu thành công thành công", doc.ID);
        }
        #endregion Create

        #region Update
        public async Task<ServiceResult> UpdateDocFile(VMDoc model)
        {
            var entity = await _dasRepo.Doc.GetAsync(model.ID);
            _mapper.Map(model, entity);
            await _dasRepo.Doc.UpdateAsync(entity);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess();
        }
        public async Task<ServiceResult> Update(Hashtable data, bool isComplete)
        {
            var doc = Utils.Bind<Doc>(data);
            if (isComplete)
            {
                doc.Status = (int)EnumDocCollect.Status.Complete;
            }
            else
            {
                doc.Status = (int)EnumDocCollect.Status.Active;
            }
            var check = await _dasRepo.Doc.AnyAsync(x => x.ID == doc.ID && x.Status == (int)EnumDocCollect.Status.InActive);
            if (check)
            {
                return new ServiceResultError("Thành phần hồ sơ không tồn tại hoặc đã xóa");
            }


            await _dasRepo.Doc.UpdateAsync(doc);
            await _dasRepo.SaveAync();

            var inserts = new List<DocField>();
            var updates = new List<DocField>();
            var deletes = new List<DocField>();

            var docTypeFields = await (from dtf in _dasRepo.DocTypeField.GetAll()
                                       where dtf.IDDocType == doc.IDDocType
                                       orderby dtf.Priority
                                       select _mapper.Map<VMDocTypeField>(dtf)).ToListAsync();
            var docFields = (await (_dasRepo.DocField.GetAllListAsync(x => x.IDDoc == doc.ID)));
            if (docFields.IsNotEmpty())
            {
                foreach (var field in docTypeFields)
                {
                    var docField = docFields.FirstOrDefault(n => n.IDDocTypeField == field.ID);
                    if (Utils.IsEmpty(docField))
                    {
                        docField = new DocField
                        {
                            IDDoc = doc.ID,
                            IDDocTypeField = field.ID,
                            Status = 1,
                            CreatedBy = doc.CreatedBy,
                            CreateDate = DateTime.Now,
                        };
                        inserts.Add(docField);
                    }
                    else
                    {
                        docField.UpdatedBy = doc.UpdatedBy;
                        docField.UpdatedDate = DateTime.Now;
                        updates.Add(docField);
                    }
                    BindDocField(docField, field, data);
                }
            }

            if (Utils.IsNotEmpty(docFields))
            {
                var updateIDs = updates.Select(x => x.ID).ToArray();
                deletes = docFields.Where(n => !updateIDs.Contains(n.ID)).ToList();
            }
            if (deletes.IsNotEmpty() || updates.IsNotEmpty() || inserts.IsNotEmpty())
            {
                if (deletes.IsNotEmpty())
                {
                    await _dasRepo.DocField.DeleteAsync(deletes);
                }
                if (updates.IsNotEmpty())
                {
                    await _dasRepo.DocField.UpdateAsync(updates);
                }
                if (inserts.IsNotEmpty())
                {
                    await _dasRepo.DocField.InsertAsync(inserts);
                }
                await _dasRepo.SaveAync();
            }
            //update Profile status
            if (isComplete)
            {
                var profile = await _dasRepo.PlanProfile.GetAsync(doc.IDProfile);
                var catalogingdocs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == profile.ID && x.Status == (int)EnumDocCollect.Status.Active);
                if ((catalogingdocs == null || catalogingdocs.Count() == 0) && profile.Status != (int)EnumProfilePlan.Status.Reject)
                {
                    profile.Status = (int)EnumProfilePlan.Status.CollectComplete;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                }
            }
            else
            {
                var profile = await _dasRepo.PlanProfile.GetAsync(doc.IDProfile);
                if (profile.Status != (int)EnumProfilePlan.Status.Reject)
                {
                    profile.Status = (int)EnumProfilePlan.Status.Active;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                }

            }
            return new ServiceResultSuccess("Cập nhật tài liệu thành công", doc.ID);
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteDoc(int id)
        {
            try
            {
                var docDelete = await _dasRepo.Doc.GetAsync(id);
                if (docDelete == null || docDelete.Status == (int)EnumDocCollect.Status.InActive)
                {
                    return new ServiceResultError("Tài liệu này hiện không tồn tại hoặc đã bị xóa");
                }
                var profile = await _dasRepo.PlanProfile.GetAsync(docDelete.IDProfile);
                if (profile == null || (profile.Status != (int)EnumProfilePlan.Status.CollectComplete && profile.Status != (int)EnumProfilePlan.Status.Active && profile.Status != (int)EnumProfilePlan.Status.Reject))
                {
                    return new ServiceResultError("Không thể xóa tài liệu trong hồ sơ này");
                }
                docDelete.Status = (int)EnumDocCollect.Status.InActive;
                await _dasRepo.Doc.UpdateAsync(docDelete);
                await _dasRepo.SaveAync();
                await UpdateProfileStatus(docDelete.IDProfile);
                var childs = await _dasRepo.DocField.GetAllListAsync(x => x.IDDoc == id);
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.DocField.UpdateAsync(childs);
                await _dasRepo.SaveAync();

                await UpdateProfileTotalDoc(docDelete.IDProfile);

                return new ServiceResultSuccess("Xóa tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteDocs(IEnumerable<int> ids)
        {
            try
            {
                var docDeletes = await _dasRepo.Doc.GetAllListAsync(x => ids.Contains(x.ID));
                if (docDeletes == null || docDeletes.Count() == 0)
                {
                    return new ServiceResultError("Tài liệu này hiện không tồn tại hoặc đã bị xóa");
                }
                var idProfiles = docDeletes.Select(x => x.IDProfile).ToList();

                var profiles = await _dasRepo.PlanProfile.GetAllListAsync(x => idProfiles.Contains(x.ID));
                var check = false;
                foreach (var item in profiles)
                {
                    if (item.Status != (int)EnumProfilePlan.Status.CollectComplete && item.Status != (int)EnumProfilePlan.Status.Active && item.Status != (int)EnumProfilePlan.Status.Reject)
                    {
                        check = true;
                        break;
                    }
                }
                if (check)
                {
                    return new ServiceResultError("Không thể xóa tài liệu trong hồ sơ này");
                }
                foreach (var doc in docDeletes)
                {
                    doc.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.Doc.UpdateAsync(docDeletes);
                await _dasRepo.SaveAync();
                //update status profile
                foreach (var item in idProfiles)
                {
                    await UpdateProfileStatus(item);
                }
                var childs = await _dasRepo.DocField.GetAllListAsync(x => ids.Contains(x.IDDoc));
                foreach (var child in childs)
                {
                    child.Status = (int)EnumDocCollect.Status.InActive;
                }

                await _dasRepo.DocField.UpdateAsync(childs);
                await _dasRepo.SaveAync();

                await UpdateProfileTotalDoc(docDeletes.FirstOrNewObj().IDProfile);

                return new ServiceResultSuccess("Xóa tài liệu thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion Delete

        #region File
        public async Task<ServiceResult> SaveFile(VMDoc model, string urlViewFile)
        {
            try
            {
                if (model.File != null)
                {
                    var stgFile = new VMStgFile
                    {
                        File = model.File,
                        FileName = model.File.FileName,
                        FileType = (int)EnumFile.Type.Doc,
                        IsTemp = false
                    };
                    var resultUpload = await _fileClientService.Upload(stgFile);
                    if (resultUpload.Code == null || resultUpload.Data == null || !resultUpload.Code.Equals(CommonConst.Success))
                    {
                        return new ServiceResultError("Tải file lên không thành công!");
                    }
                    else
                    {
                        var objUpload = Utils.Deserialize<VMStgFile>(resultUpload.Data.ToString());
                        var doc = await _dasRepo.Doc.GetAsync(model.ID);
                        if (Utils.IsNotEmpty(doc))
                        {

                            if (Utils.IsNotEmpty(objUpload))
                            {
                                doc.IDFile = objUpload.ID;
                                await _dasRepo.Doc.UpdateAsync(doc);
                                await _dasRepo.SaveAync();

                            }
                        }
                        return new ServiceResultSuccess("Tải file lên thành công",
                                  new
                                  {
                                      UrlFile = urlViewFile + FileUltils.EncryptPathFile(objUpload.PhysicalPath, _userPrincipalService.UserId),
                                      IDFile = objUpload.ID
                                  });
                    }
                }
                return new ServiceResultError("Vui lòng chọn file tài lên");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi trong quá trình tài lên");
            }
        }
        #endregion File

        #region OCR
        public async Task<ServiceResult> AutoOCR(long idFile)
        {
            try
            {
                var physicalPath = await _fileClientService.GetPhysicalPathById(idFile);
                if (Utils.IsNotEmpty(physicalPath))
                {
                    APIs.SetServerAddress(ioneSvAddress);
                    VBHCResult vbhc = APIs.ExtractInfoAPI.GetVBHCInfo(physicalPath.Data.ToString());
                    if (vbhc != null && vbhc.VBHCInfo != null)
                    {
                        var info = vbhc.VBHCInfo;
                        var docInfo = new
                        {
                            //KinhGui = info.KinhGui,
                            //NoiNhan = info.NoiNhan,
                            //DieuKhoan = info.DieuKhoan,
                            CodeNotation = info.KyHieu,//Ký hiệu của văn bản
                            OrganName = info.NoiBanHanh, //Noi ban hành
                            CodeNumber = info.SoKyHieu,//Số của văn bản
                                                       // SoQuyetDinh = info.SoQuyetDinh,
                            Subject = info.VeViec, //Trích yếu nội dung
                                                   //NoiDung = info.NoiDung,
                            TypeName = info.LoaiVB, //ten loai vb
                            IssuedDate = info.NgayKy, //Ngày vb
                                                      //NguoiKy = info.NguoiKy,
                        };
                        return new ServiceResultSuccess("Đã nhận dạng xong", docInfo);

                    }
                }
                return new ServiceResultError("Không tìm thấy file");

            }
            catch (Exception)
            {
                return new ServiceResultError("Lỗi nhận dạng, vui lòng thử lại");

            }
        }
        #endregion OCR

        #region Scan

        public async Task<ServiceResult> SaveScanFile(VMDoc vmDoc, string urlViewFile)
        {
            try
            {
                if (vmDoc.IDFile > 0)
                {
                    var stgFile = await _dasRepo.StgFile.GetAsync(vmDoc.IDFile);
                    if (stgFile == null)
                        return new ServiceResultError("Có lỗi trong quá trình upload file scan");

                    var doc = await _dasRepo.Doc.GetAsync(vmDoc.ID);

                    if (Utils.IsNotEmpty(doc))
                    {
                        //Đánh dáu file cũ là file temp
                        if (doc.IDFile > 0)
                            await _fileClientService.MarkFileTemp(doc.IDFile);

                        doc.IDFile = vmDoc.IDFile;
                        await _dasRepo.Doc.UpdateAsync(doc);
                        await _dasRepo.SaveAync();

                    }
                    else
                    {
                        //Tạo doc mới
                        //doc = new Doc
                        //{
                        //    IDFile = vmDoc.IDFile
                        //};
                        //await _dasRepo.Doc.InsertAsync(doc);
                        //await _dasRepo.SaveAync();
                    }
                    return new ServiceResultSuccess("Lưu file scan thành công", new
                    {
                        UrlFile = urlViewFile + FileUltils.EncryptPathFile(stgFile.PhysicalPath, stgFile.CreatedBy.GetValueOrDefault(0)),
                        vmDoc.IDFile,
                        //IDDoc = doc.ID
                    });
                }
                return new ServiceResultError("Chưa có file scan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi trong quá trình lưu file scan");
            }
        }


        #endregion Scan

        #region Private
        private async Task<VMUpdatePlanProfile> UpdatePlanProfile(int id)
        {
            var profile = await _dasRepo.PlanProfile.GetAsync(id) ?? new PlanProfile();
            var model = Utils.Bind<VMUpdatePlanProfile>(profile.KeyValue());
            model.StartDate = Utils.DateToString(profile.StartDate);
            model.EndDate = Utils.DateToString(profile.EndDate);
            model.VMPlan = _mapper.Map<VMPlan>(await _dasRepo.Plan.GetAsync(profile.IDPlan) ?? new Plan());
            await GetUpdateModel(model);
            return model;
        }

        private async Task GetUpdateModel(VMUpdatePlanProfile model)
        {
            model.DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString());
            model.DictProfileTemplate = await GetDictProfileTemplate(model.IDProfileTemplate.GetValueOrDefault(0));
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictLangugage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString());
            model.DictExpiryDate = await GetDictExpiryDate();
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());
            model.DictSecurityLevel = (await _dasRepo.SercureLevel.GetAllListAsync(u => u.Status == (int)EnumSercureLevel.Status.Active)).ToDictionary(n => n.ID, n => n.Name);
            model.DictAgencies = await GetDictAgencies();
            if (model.Language == null || model.Language == "null" || model.Language == string.Empty)
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
                        model.Language = model.Language.Substring(0, model.Language.Length - 2);
                    }
                }
                else
                    model.Language = model.DictLangugage.GetValueOrDefault(int.Parse(model.Language));
            }
        }

        private async Task<Dictionary<int, string>> GetDictAgencies()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(n => n.IDOrgan == userData.IDOrgan &&
            n.Status == (int)EnumAgency.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var cates = (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == userData.IDOrgan));

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
                return treeModels.ToDictionary(n => (int)n.ID, n => n.Name);
            }
            return cates.ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a =>
            a.Status == (int)EnumOrgan.Status.Active
            && a.IDOrgan == userData.IDOrgan
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
            )).ToDictionary(n => n.ID, n => n.FondName);
        }

        /// <summary>
        /// Lấy các tài liệu trong 1 hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocs(PlanDocCondition condition, bool isExport = false)
        {
            try
            {
                string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
                string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
                string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
                var tempFiled = from df in _dasRepo.DocField.GetAll()
                                join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                                join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                                where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                                || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                                || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                                select df;

                var temp = from d in _dasRepo.Doc.GetAll()
                           join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                           join v in tempFiled on d.ID equals v.IDDoc
                           where (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                           && (d.IDProfile == condition.IDProfile)
                           && (p.IDAgency == condition.IDAgency || condition.IDAgency == 0)
                           && (condition.IDStatus == -1 || condition.IDStatus == d.Status)
                           //&& (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                           &&
                           (p.Status == (int)EnumProfilePlan.Status.Active
                           || p.Status == (int)EnumProfilePlan.Status.CollectComplete
                           || p.Status == (int)EnumProfilePlan.Status.Reject
                           || p.Status == (int)EnumProfilePlan.Status.WaitApprove
                           || p.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved
                           || p.Status == (int)EnumProfilePlan.Status.ArchiveApproved)
                           && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                           group new { d, p, v } by new
                           {
                               d.ID,
                               d.IDChannel,
                               d.IDFile,
                               d.IDProfile,
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
                               IDProfile = g.Key.IDProfile,
                               IDDocType = g.Key.IDDocType,
                               Status = g.Key.Status,
                               CreatedBy = g.Key.CreatedBy,
                               CreateDate = g.Key.CreateDate,
                               UpdatedDate = g.Key.UpdatedDate,
                               UpdatedBy = g.Key.UpdatedBy
                           };
                if (isExport)
                {
                    var rs = await temp.ToListAsync();
                    return new PaginatedList<VMPlanDoc>(rs, rs.Count(), 1, rs.Count());
                }
                var total = await temp.LongCountAsync();
                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
                return new PaginatedList<VMPlanDoc>(vmDocs, (int)total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsNoPaging(PlanDocCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       let conditionStr = userData.HasOrganPermission ? p.IDOrgan == userData.IDOrgan : (p.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + p.IDAgency + "|"))
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && d.IDProfile == condition.IDProfile
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       && (p.Status == (int)EnumProfilePlan.Status.WaitApprove || p.Status == (int)EnumProfilePlan.Status.WaitArchiveApproved || p.Status == (int)EnumProfilePlan.Status.ArchiveReject)
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       where conditionStr
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
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
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var docs = await temp.ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, 0, condition.PageIndex, condition.PageSize);

        }

        private async Task<PaginatedList<VMPlanDoc>> GetPlanDocsListApprovedNoPaging(PlanDocCondition condition)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.DocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.Doc.GetAll()
                       join p in _dasRepo.PlanProfile.GetAll() on d.IDProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDDoc
                       let conditionStr = userData.HasOrganPermission ? p.IDOrgan == userData.IDOrgan : (p.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + p.IDAgency + "|"))
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && d.IDProfile == condition.IDProfile && conditionStr
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       && p.Status == (int)EnumProfilePlan.Status.ArchiveApproved && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword))
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDProfile,
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
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };
            var docs = await temp.ToListAsync();
            var vmDocs = _mapper.Map<List<VMPlanDoc>>(docs);
            return new PaginatedList<VMPlanDoc>(vmDocs, 0, condition.PageIndex, condition.PageSize);

        }

        /// <summary>
        /// Lấy Hồ sơ và kế hoạch của list thành phần trong hồ sơ
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>

        private async Task<VMPlanProfile> GetPlanProfile(int idProfile)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       join p in _dasRepo.Plan.GetAll() on pp.IDPlan equals p.ID
                       where (pp.Status != (int)EnumProfilePlan.Status.InActive)
                       && p.Status == (int)EnumPlan.Status.Approved && pp.ID == idProfile
                       join a in _dasRepo.Agency.GetAll() on pp.IDAgency equals a.ID
                       let conditionStr = userData.HasOrganPermission ? pp.IDOrgan == userData.IDOrgan : (pp.IDAgency == userData.IDAgency || ("|" + userData.ParentPath + "|").Contains("|" + pp.IDAgency + "|"))
                       where conditionStr
                       select new VMPlanProfile
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
                           Format = pp.Format,
                           Status = pp.Status,
                           IDAgency = pp.IDAgency,
                           CreateDate = pp.CreateDate,
                           PlanName = p.Name,
                           ReasonToReject = pp.ReasonToReject,
                           IDProfileCategory = pp.IDProfileCategory,
                           ApprovedBy = pp.ApprovedBy
                       };

            return await temp.FirstOrDefaultAsync();
        }

        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        /// <summary>
        /// Lấy list các DocType cho combobox
        /// </summary>
        /// <returns></returns>
        private async Task<List<VMDocType>> GetDocTypes()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       && dc.IDOrgan == userData.IDOrgan
                       orderby dc.ID
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        /// <summary>
        /// Lấy list các define khung biên mục
        /// </summary>
        /// <param name="idDoctype"></param>
        /// <returns></returns>
        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        /// <summary>
        /// Tạo các DocFiled theo list Type Fileds
        /// </summary>
        /// <param name="docTypeFields"></param>
        /// <returns></returns>
        private async Task<List<VMDocField>> GetDocFieldsDefault(List<VMDocTypeField> docTypeFields, VMPlanProfile vMPlanProfile, VMDocType vMDocType)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var organName = (await _dasRepo.Organ.GetAsync(userData.IDOrgan)).Name;
            var vmDocFields = new List<VMDocField>();
            if (docTypeFields.IsEmpty())
                return vmDocFields;
            bool isType1 = vMDocType.Type == (int)EnumDocType.Type.Doc;
            foreach (var field in docTypeFields)
            {
                int val = 0;
                string text;
                switch (field.InputType)
                {
                    case (int)EnumDocType.InputType.InpNumber:
                    case (int)EnumDocType.InputType.InpFloat:
                        text = val.ToString();
                        break;
                    case (int)EnumDocType.InputType.InpText:
                        text = string.Empty;
                        break;
                    case (int)EnumDocType.InputType.InpDate:
                        text = Utils.DateToString(DateTime.Now, field.Format ?? "dd-MM-yyyy");
                        break;
                    default:
                        text = val.ToString();
                        break;
                }
                //Giá trị mặc định văn bản theo hồ sơ
                if (isType1)
                {
                    switch (field.Code)
                    {
                        case "FileCode":
                            text = vMPlanProfile.FileCode ?? vMPlanProfile.FileCode;
                            break;
                        case "Identifier":
                            text = vMPlanProfile.Identifier ?? vMPlanProfile.Identifier;
                            break;
                        case "Organld":
                            text = vMPlanProfile.IDProfileTemplate.ToString();
                            break;
                        case "FileCatalog":
                            text = vMPlanProfile.FileCatalog.ToString();
                            break;
                        case "FileNotation":
                            text = vMPlanProfile.FileNotation ?? vMPlanProfile.FileNotation;
                            break;
                        case "OrganName":
                            text = organName;
                            break;
                        default:
                            break;
                    }
                }
                vmDocFields.Add(new VMDocField
                {
                    IDDoc = 0,
                    IDDocTypeField = field.ID,
                    Value = text
                });

            }
            return vmDocFields;

        }

        private void BindDocField(DocField docField, VMDocTypeField field, Hashtable data)
        {
            var fieldName = "Field" + field.ID;
            docField.Value = Utils.GetString(data, fieldName);
        }

        private async Task<List<VMDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.DocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && df.IDDoc == IDDoc
                       select _mapper.Map<VMDocField>(df);
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
        private async Task<List<VMDocField>> GetDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.DocField.GetAll()
                       where /*df.Status != (int)EnumCommon.Status.InActive*/
                       //&& 
                       IDDocs.Contains(df.IDDoc)
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }
        private async Task<List<VMDocType>> GetDocTypes(IEnumerable<int> IDDocTypes)
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       && IDDocTypes.Contains(dc.ID)
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Cập nhật sps văn bản trong hồ sơ
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        private async Task UpdateProfileTotalDoc(int idProfile)
        {
            var profile = await _dasRepo.PlanProfile.GetAsync(idProfile);
            if (profile != null)
            {
                profile.TotalDoc = await _dasRepo.Doc.CountAsync(n => n.Status != (int)EnumDocCollect.Status.InActive && n.IDProfile == idProfile);

                await _dasRepo.PlanProfile.UpdateAsync(profile);
                await _dasRepo.SaveAync();
            }
        }

        private async Task UpdateProfileStatus(int idProfile)
        {
            var profile = await _dasRepo.PlanProfile.GetAsync(idProfile);
            var catalogingdocs = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == profile.ID && x.Status != (int)EnumDocCollect.Status.InActive);
            if (profile.Status != (int)EnumProfilePlan.Status.Reject)
            {
                if (catalogingdocs == null || catalogingdocs.Count() == 0)
                {
                    profile.Status = (int)EnumProfilePlan.Status.Active;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                    return;
                }
                if (catalogingdocs.Count(x => x.Status == (int)EnumDocCollect.Status.Active) == 0)
                {
                    profile.Status = (int)EnumProfilePlan.Status.CollectComplete;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                    return;
                }
                else
                {
                    profile.Status = (int)EnumProfilePlan.Status.Active;
                    await _dasRepo.PlanProfile.UpdateAsync(profile);
                    await _dasRepo.SaveAync();
                    return;
                }
            }
        }

        private async Task<Dictionary<int, string>> GetDictUsers()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.User.GetAllListAsync(n =>
            n.Status == (int)EnumCommon.Status.Active && n.IDOrgan == userData.IDOrgan)).ToDictionary(n => n.ID, n => n.Name);
        }
        #endregion

        public async Task<ServiceResult> GetDocByIDFile(long id)
        {
            var doc = await _dasRepo.Doc.GetAllListAsync(x => x.IDFile == id && (x.Status == (int)EnumDocCollect.Status.Active || x.Status == (int)EnumDocCollect.Status.Complete));
            if (doc.IsEmpty())
                return new ServiceResultError("Không tồn tại văn bản");
            var docList = _mapper.Map<IEnumerable<VMDoc>>(doc).ToList();
            return new ServiceResultSuccess(docList[0]);
        }
    }
}
