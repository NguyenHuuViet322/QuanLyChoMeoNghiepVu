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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OneAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ESD.Application.Services
{
    public class ProfileDestructionService : IProfileDestructionServices
    {
        #region Properties
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ILoggerManager _logger;
        private readonly ICacheManagementServices _cacheManagementServices;
        #endregion Properties

        #region Ctor
        public ProfileDestructionService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService userPrincipalService
            , ILoggerManager logger
            , ICacheManagementServices cacheManagementServices)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _userPrincipalService = userPrincipalService;
            _logger = logger;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion Ctor

        #region List & Search
        public async Task<VMIndexProfileWaitDestruction> SearchByConditionPagging(ProfileWaitDestructionCondition condition)
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var foreverValue = await GetForeverValue();
            //Lọc thời hạn bảo quản nhỏ hơn vĩnh viễn
            var iqExpiryDate = from ex in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active && x.Value < foreverValue)
                               select ex;
            IQueryable<VMPlanProfile> lastRS;
            //List quá hạn
            var iqrExpiry = from pp in _dasRepo.CatalogingProfile.GetAll()
                            join ex in iqExpiryDate on pp.IDExpiryDate equals ex.ID
                            where pp.Status == (int)EnumCataloging.Status.StorageApproved
                            && pp.IDOrgan == userData.IDOrgan
                            && pp.InUsing == (int)EnumCataloging.InUse.Using //đang sử dụng
                            select new VMPlanProfile
                            {
                                ID = pp.ID,
                                IDOrgan = pp.IDOrgan,
                                IDAgency = pp.IDAgency,
                                IDPlan = pp.IDPlan,
                                Type = pp.Type,
                                FileCode = pp.FileCode,
                                IDStorage = pp.IDStorage,
                                IDShelve = pp.IDShelve,
                                IDBox = pp.IDBox,
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
                                IDProfileCategory = pp.IDProfileCategory,
                                ExpiryDate = (pp.ApprovedDate ?? DateTime.Now).AddYears(ex.Value),
                                StatusDestruction = (int)EnumCataloging.StatusDestruction.Expiry
                            };
            var iqAfterParseDate = from rs in iqrExpiry
                                   where (condition.Keyword.IsEmpty() || rs.Title.Contains(condition.Keyword) || rs.FileNotation.Contains(condition.Keyword))
                                   &&(DateTime.Now > rs.ExpiryDate) ////Gỡ comment để lọc ngày hết hạn
                                   && (condition.IDExpiryDate == -1 || rs.IDExpiryDate == condition.IDExpiryDate)
                                   && (condition.IDStorage == -1 || rs.IDStorage == condition.IDStorage)
                                   && (condition.IDShelve == -1 || rs.IDShelve == condition.IDShelve)
                                   && (condition.IDBox == -1 || rs.IDBox == condition.IDBox)
                                   select rs;
            //list hết giá trị
            var tempOffvalue = from pp in _dasRepo.CatalogingProfile.GetAll()
                               where pp.Status == (int)EnumCataloging.Status.StorageApproved
                               && pp.IDOrgan == userData.IDOrgan
                               && pp.InUsing == (int)EnumCataloging.InUse.OffValue
                               && (condition.Keyword.IsEmpty() || pp.Title.Contains(condition.Keyword) || pp.FileNotation.Contains(condition.Keyword))
                               && (condition.IDExpiryDate == -1 || pp.IDExpiryDate == condition.IDExpiryDate)
                                   && (condition.IDStorage == -1 || pp.IDStorage == condition.IDStorage)
                                   && (condition.IDShelve == -1 || pp.IDShelve == condition.IDShelve)
                                   && (condition.IDBox == -1 || pp.IDBox == condition.IDBox)
                               select new VMPlanProfile
                               {
                                   ID = pp.ID,
                                   IDOrgan = pp.IDOrgan,
                                   IDAgency = pp.IDAgency,
                                   IDPlan = pp.IDPlan,
                                   Type = pp.Type,
                                   FileCode = pp.FileCode,
                                   IDStorage = pp.IDStorage,
                                   IDShelve = pp.IDShelve,
                                   IDBox = pp.IDBox,
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
                                   IDProfileCategory = pp.IDProfileCategory,
                                   ExpiryDate = DateTime.Now,
                                   StatusDestruction = (int)EnumCataloging.StatusDestruction.OffValue
                               };
            if (condition.IDStatus == (int)EnumCataloging.StatusDestruction.Expiry)
            {
                lastRS = iqAfterParseDate;
            }
            else if (condition.IDStatus == (int)EnumCataloging.StatusDestruction.OffValue)
            {
                lastRS = tempOffvalue;
            }
            else
            {
                lastRS = tempOffvalue.Concat(iqAfterParseDate);
            }


            //list Profile
            var total = await lastRS.LongCountAsync();
            if (total <= 0)
                return new VMIndexProfileWaitDestruction();

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var model = new VMIndexProfileWaitDestruction();
            var list = await lastRS.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMPlanProfiles = new PaginatedList<VMPlanProfile>(list, (int)total, condition.PageIndex, condition.PageSize);

            //Dict
            var expiryDates = await _dasRepo.ExpiryDate.GetAllListAsync(x => x.Status == (int)EnumExpiryDate.Status.Active);
            if (expiryDates != null && expiryDates.Count() > 0)
                model.DictExpiryDate = expiryDates.OrderBy(x=>x.Value).ToDictionary(n => n.ID, n => n.Name);
            model.DictProfileTemplate = await GetDictProfileTemplate(0);
            //model.DictAgency = await GetDictAgencies();
            //model.DictProfileType = Utils.EnumToDic<EnumProfile.Type>();
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictSheleve = await GetDictCategory(EnumCategoryType.Code.DM_Gia.ToString());
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());

            return model;
        }
        public async Task<VMIndexDestructionProfile> SearchByConditionPagging(DestructionProfileCondition condition, List<int> lstStatus, bool byApprove = false)
        {
            var model = new VMIndexDestructionProfile();
            var temp = from df in _dasRepo.DestructionProfile.GetAll()
                       where df.Status != (int)EnumDestruction.Status.InActive
                       && df.IDOragan == _userPrincipalService.IDOrgan
                       && lstStatus.Contains(df.Status)
                       && (condition.Keyword.IsEmpty() || df.Name.Contains(condition.Keyword))
                       && (condition.IDStatus == -1 || condition.IDStatus == df.Status)
                       && (!byApprove || df.ApprovedBy == _userPrincipalService.UserId)
                       select _mapper.Map<VMDestructionProfile>(df);

            //list Destruction
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return new VMIndexDestructionProfile
                { VMDestructionProfiles = new PaginatedList<VMDestructionProfile>() };

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var list = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMDestructionProfiles = new PaginatedList<VMDestructionProfile>(list, (int)total, condition.PageIndex, condition.PageSize);
            //Dict
            model.DictUser = await GetDictUserApproved();
            return model;
        }
        #endregion List & Search

        #region DestructionProfile
        #region ChangeExpiryDate
        public async Task<ServiceResult> ChangeExpiryDate(int idProfile, int idExpiryDate)
        {
            var expiryDate = await GetDictExpiryDate();
            if (!expiryDate.ContainsKey(idExpiryDate))
            {
                return new ServiceResultError("Gia hạn hồ sơ không thành công");
            }
            var exist = await _dasRepo.CatalogingProfile.FirstOrDefaultAsync(x=> x.ID ==idProfile && x.Status == (int)EnumCataloging.Status.StorageApproved && x.InUsing == (int)EnumCataloging.InUse.Using);
            if (exist == null)
            {
                return new ServiceResultError("Gia hạn hồ sơ không thành công");
            }
            exist.IDExpiryDate = idExpiryDate;
            await _dasRepo.CatalogingProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Gia hạn hồ sơ thành công");
        }
        #endregion ChangeExpiryDate

        #region AddProfileWaitDestruction
        public async Task<ServiceResult> AddProfileWaitDestructions(IEnumerable<int> ids)
        {
            var lstProfile = await _dasRepo.CatalogingProfile.GetAllListAsync(x => x.Status == (int)EnumCataloging.Status.StorageApproved && x.InUsing ==(int)EnumCataloging.InUse.Using && ids.Contains(x.ID));

            if (lstProfile == null || lstProfile.Count() <= 0)
            {
                return new ServiceResultError("Không thể thêm hồ sơ vào danh sách chờ tiêu hủy");
            }
            foreach (var item in lstProfile)
            {
                item.InUsing = (int)EnumCataloging.InUse.OffValue;
            }
            await _dasRepo.CatalogingProfile.UpdateAsync(lstProfile);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Thêm hồ sơ vào danh sách chờ tiêu hủy thành công");
        }
        public async Task<ServiceResult> RemoveProfileWaitDestructions(int id)
        {
            var exist = await _dasRepo.CatalogingProfile.FirstOrDefaultAsync(x=> x.Status == (int)EnumCataloging.Status.StorageApproved && x.InUsing == (int)EnumCataloging.InUse.OffValue && x.ID == id);
            if (exist == null)
            {
                return new ServiceResultError("Không thể bỏ hồ sơ ra khỏi danh sách chờ tiêu hủy");
            }
            exist.InUsing = (int)EnumCataloging.InUse.Using;
            await _dasRepo.CatalogingProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Bỏ hồ sơ ra khỏi danh sách chờ tiêu hủy thành công");
        }

        public async Task<VMIndexProfileWaitDestruction> SearchUsingByConditionPagging(ProfileWaitDestructionCondition condition)
        {
            if (_userPrincipalService == null || _userPrincipalService.IDAgency <= 0)
                return new VMIndexProfileWaitDestruction();
            var foreverValue = await GetForeverValue();
            //Lọc thời hạn bảo quản nhỏ hơn vĩnh viễn
            var iqExpiryDate = from ex in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active)
                               select ex;
            //List quá hạn
            var iqrExpiry = from pp in _dasRepo.CatalogingProfile.GetAll()
                            join ex in iqExpiryDate on pp.IDExpiryDate equals ex.ID
                            where pp.Status == (int)EnumCataloging.Status.StorageApproved
                            && pp.IDOrgan == _userPrincipalService.IDOrgan
                            && pp.InUsing == (int)EnumCataloging.InUse.Using //đang sử dụng
                            select new VMPlanProfile
                            {
                                ID = pp.ID,
                                IDOrgan = pp.IDOrgan,
                                IDAgency = pp.IDAgency,
                                IDPlan = pp.IDPlan,
                                Type = pp.Type,
                                FileCode = pp.FileCode,
                                IDStorage = pp.IDStorage,
                                IDShelve = pp.IDShelve,
                                IDBox = pp.IDBox,
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
                                IDProfileCategory = pp.IDProfileCategory,
                                ExpiryDate = (pp.ApprovedDate ?? DateTime.Now).AddYears(ex.Value),
                                isForever = ex.Value >= foreverValue
                            };
            var iqAfterParseDate = from rs in iqrExpiry
                                   where (condition.Keyword.IsEmpty() || rs.Title.Contains(condition.Keyword) || rs.FileNotation.Contains(condition.Keyword))
                                   &&(DateTime.Now < rs.ExpiryDate || rs.isForever) 
                                   && (condition.IDExpiryDate == -1 || rs.IDExpiryDate == condition.IDExpiryDate)
                                   && (condition.IDStorage == -1 || rs.IDStorage == condition.IDStorage)
                                   && (condition.IDShelve == -1 || rs.IDShelve == condition.IDShelve)
                                   && (condition.IDBox == -1 || rs.IDBox == condition.IDBox)
                                   select rs;
            //list Profile
            var total = await iqAfterParseDate.LongCountAsync();
            if (total <= 0)
                 return new VMIndexProfileWaitDestruction(); 

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var model = new VMIndexProfileWaitDestruction();
            var list = await iqAfterParseDate.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            model.VMPlanProfiles = new PaginatedList<VMPlanProfile>(list, (int)total, condition.PageIndex, condition.PageSize);

            //Dict
            var expiryDates = await _dasRepo.ExpiryDate.GetAllListAsync(x => x.Status == (int)EnumExpiryDate.Status.Active);
            if (expiryDates != null && expiryDates.Count() > 0)
                model.DictExpiryDate = expiryDates.ToDictionary(n => n.ID, n => n.Name);
            model.DictProfileTemplate = await GetDictProfileTemplate(0);
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictSheleve = await GetDictCategory(EnumCategoryType.Code.DM_Gia.ToString());
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());

            return model;
        }
        #endregion AddProfileWaitDestruction
        #region Create
        public async Task<ServiceResult> CreateNewDestructionProfile(VMCreateDestructionProfile model, bool isSend = false)
        {
            var listID = model.ListProfile ?? new List<int>();
            //Kiểm tra inUse: không thuộc hồ sơ tiêu hủy nào
            var lstprofile = await _dasRepo.CatalogingProfile.GetAllListAsync(x => listID.Contains(x.ID)
            && x.Status == (int)EnumCataloging.Status.StorageApproved
            && x.InUsing != (int)EnumCataloging.InUse.WaitDestructionExpiry
            && x.InUsing != (int)EnumCataloging.InUse.WaitDestructionUnUse);
            if (lstprofile == null || lstprofile.Count() == 0)
            {
                return new ServiceResultError("Chưa chọn hồ sơ chờ tiêu hủy hợp lệ");
            }

            var destructionProfile = _mapper.Map<DestructionProfile>(model);
            var dateCreate = Utils.GetDate(model.CreatedAt);
            if (dateCreate.HasValue)
            {
                destructionProfile.CreatedAt = dateCreate ?? DateTime.Now;
            }
            if (isSend)
            {
                destructionProfile.Status = (int)EnumDestruction.Status.WaitApprove;
            }

            destructionProfile.IDUser = _userPrincipalService.UserId;
            destructionProfile.IDOragan = _userPrincipalService.IDOrgan;
            await _dasRepo.DestructionProfile.InsertAsync(destructionProfile);
            await _dasRepo.SaveAync();
            if (destructionProfile.ID == 0)
            {
                return new ServiceResultError("Tạo quyết định tiêu hủy không thành công");
            }
            //updateProfile
            foreach (var item in lstprofile)
            {
                item.IDDestruction = destructionProfile.ID;
                item.InUsing = item.InUsing == (int)EnumCataloging.InUse.Using ? (int)EnumCataloging.InUse.WaitDestructionExpiry : (int)EnumCataloging.InUse.WaitDestructionUnUse;
            }

            await _dasRepo.CatalogingProfile.UpdateAsync(lstprofile);
            await _dasRepo.SaveAync();

            //UpdateDoc
            var lstDoc = await _dasRepo.CatalogingDoc.GetAllListAsync(x => x.Status != (int)EnumDocCollect.Status.InActive && lstprofile.Select(p => p.ID).Contains(x.IDCatalogingProfile));
            if (lstDoc != null)
            {
                foreach (var item in lstDoc)
                {
                    item.InUsing = item.InUsing == (int)EnumCataloging.InUse.Using ? (int)EnumCataloging.InUse.WaitDestructionExpiry : (int)EnumCataloging.InUse.WaitDestructionUnUse;
                }
            }
            await _dasRepo.CatalogingDoc.UpdateAsync(lstDoc);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Tạo quyết định tiêu hủy thành công");
        }
        public async Task<VMCreateDestructionProfile> CreateNewDestructionProfile(List<int> ids)
        {
            var model = new VMCreateDestructionProfile();
            model.DictUser = await GetDictUserApproved();
            model.DictExpiryDate = await GetDictExpiryDate();
            model.DictStorage = await GetDictCategory(EnumCategoryType.Code.DM_Kho.ToString());
            model.DictSheleve = await GetDictCategory(EnumCategoryType.Code.DM_Gia.ToString());
            model.DictBox = await GetDictCategory(EnumCategoryType.Code.DM_HopSo.ToString());
            model.VMPlanProfiles = await GetProfilesByList(ids);

            return model;
        }
        #endregion Create

        #region SendApproved
        public async Task<ServiceResult> SendApproved(int id)
        {
            var exist = await _dasRepo.DestructionProfile.FirstOrDefaultAsync(x => x.ID == id && (x.Status == (int)EnumDestruction.Status.Active || x.Status == (int)EnumDestruction.Status.Reject));
            if (exist == null)
            {
                return new ServiceResultError("Không thể gửi phê duyệt quyết định");
            }
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => x.Status == (int)EnumCataloging.Status.StorageApproved && x.IDDestruction == id);
            if (profiles == null || profiles.Count() <= 0)
            {
                return new ServiceResultError("Không thể gửi phê duyệt quyết định");
            }

            exist.Status = (int)EnumDestruction.Status.WaitApprove;
            await _dasRepo.DestructionProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Gửi phê duyệt quyết định thành công");
        }

        public async Task<ServiceResult> DeleteDestructionProfile(int id)
        {
            var exist = await _dasRepo.DestructionProfile.FirstOrDefaultAsync(x => x.ID == id && (x.Status == (int)EnumDestruction.Status.Active || x.Status == (int)EnumDestruction.Status.Reject));
            if (exist == null)
            {
                return new ServiceResultError("Không thể xóa quyết định");
            }
            exist.Status = (int)EnumDestruction.Status.InActive;
            //Profile
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => x.Status == (int)EnumCataloging.Status.StorageApproved && x.IDDestruction == id);
            if (profiles != null)
            {
                foreach (var item in profiles)
                {
                    item.IDDestruction = 0;
                    item.InUsing = item.InUsing == (int)EnumCataloging.InUse.WaitDestructionExpiry ? (int)EnumCataloging.InUse.Using : (int)EnumCataloging.InUse.Using;

                }
                await _dasRepo.CatalogingProfile.UpdateAsync(profiles);
                await _dasRepo.SaveAync();
            }
            //Doc
            var docs = await _dasRepo.CatalogingDoc.GetAllListAsync(x => x.Status != (int)EnumDocCollect.Status.InActive && profiles.Select(p => p.ID).Contains(x.IDCatalogingProfile));
            if (docs != null)
            {
                foreach (var item in docs)
                {
                    item.InUsing = item.InUsing == (int)EnumCataloging.InUse.WaitDestructionExpiry ? (int)EnumCataloging.InUse.Using : (int)EnumCataloging.InUse.Using;

                }
                await _dasRepo.CatalogingDoc.UpdateAsync(docs);
                await _dasRepo.SaveAync();
            }
            //Destruction
            await _dasRepo.DestructionProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Xóa quyết định tiêu hủy thành công");
        }
        #endregion SendApproved

        #region ApproveDestruction
        public async Task<ServiceResult> ApproveDestruction(int id)
        {
            var exist = await _dasRepo.DestructionProfile.FirstOrDefaultAsync(x => x.ID == id && (x.Status == (int)EnumDestruction.Status.WaitApprove )
            && x.ApprovedBy == _userPrincipalService.UserId);
            if (exist == null)
            {
                return new ServiceResultError("Không thể phê duyệt quyết định");
            }
            exist.ApprovedDate = DateTime.Now;
            exist.Status = (int)EnumDestruction.Status.Approved;

            //profile
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => x.Status == (int)EnumCataloging.Status.StorageApproved && x.IDDestruction == id);
            if (profiles == null || profiles.Count() <= 0)
            {
                return new ServiceResultError("Không thể gửi phê duyệt quyết định");
            }
            var lstDetroyed = new List<ProfileDestroyed>();
            foreach (var item in profiles)
            {
                //CreateProfileDetroyed
                var detroyed = new ProfileDestroyed
                {
                    IDDestruction = id,
                    IDCatalogingProfile = item.ID,
                    IDOrgan = item.IDOrgan,
                    Title = item.Title,
                    FileCode = item.FileCode,
                    IDExpiryDate = item.IDExpiryDate,
                    IDStorage = item.IDStorage,
                    IDShelve = item.IDShelve,
                    IDBox = item.IDBox,
                    InUsing = item.InUsing,
                    Status = (int)EnumProfileDetroyed.Status.Detroyed,
                };
                lstDetroyed.Add(detroyed);
                //Update status Cataloging
                item.Status = (int)EnumCataloging.Status.Destroyed;
            }

            if (lstDetroyed.Count > 0 )
            {
                await _dasRepo.ProfileDestroyed.InsertAsync(lstDetroyed);
                await _dasRepo.SaveAync();
            }

            await _dasRepo.CatalogingProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();

            var docs = await _dasRepo.CatalogingDoc.GetAllListAsync(x => x.Status != (int)EnumDocCollect.Status.InActive && profiles.Select(p => p.ID).Contains(x.IDCatalogingProfile));
            if (docs != null)
            {
                foreach (var item in docs)
                {
                    item.Status = (int)EnumDocCollect.Status.Destroyed;

                }
                await _dasRepo.CatalogingDoc.UpdateAsync(docs);
                await _dasRepo.SaveAync();
            }

            await _dasRepo.DestructionProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Đã tiêu hủy thành công");

        }

        public async Task<ServiceResult> RejectDestruction(int id, string reason = "")
        {
            var exist = await _dasRepo.DestructionProfile.FirstOrDefaultAsync(x => x.ID == id && x.Status == (int)EnumDestruction.Status.WaitApprove && x.ApprovedBy == _userPrincipalService.UserId);
            if (exist == null)
            {
                return new ServiceResultError("Không thể từ chối quyết định");
            }
            exist.Status = (int)EnumDestruction.Status.Reject;
            exist.ReasonToReject = reason;

            await _dasRepo.DestructionProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Đã từ chối quyết định thành công");
        }
        #endregion ApproveDestruction

        #region Restore Destruction
        public async Task<ServiceResult> RestoreDestruction(int id)
        {
            var exist = await _dasRepo.DestructionProfile.FirstOrDefaultAsync(x => x.ID == id && x.Status == (int)EnumDestruction.Status.Approved);
            if (exist == null)
            {
                return new ServiceResultError("Không thể khôi phục dữ liệu quả quyết định");
            }
            if (exist.ApprovedDate.Value.AddYears(1) < DateTime.Now)
            {
                return new ServiceResultError("Đã quá hạn khôi phục dữ liệu");
            }
            exist.Status = (int)EnumDestruction.Status.Restored;

            //ProfileDetroyed
            var detroyeds = await _dasRepo.ProfileDestroyed.GetAllListAsync(x => x.IDDestruction == id &&
            x.Status != (int)EnumProfileDetroyed.Status.Restored);
            if (detroyeds == null || detroyeds.Count() <= 0)
            {
                return new ServiceResultError("Không thể khôi phụ dữ liệu của quyết định");
            }
            foreach (var item in detroyeds)
            {
                item.Status = (int)EnumProfileDetroyed.Status.Restored;
            }
            await _dasRepo.ProfileDestroyed.UpdateAsync(detroyeds);
            await _dasRepo.SaveAync();
            //Profile 
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => x.Status == (int)EnumCataloging.Status.Destroyed 
            && detroyeds.Select(x=> x.IDCatalogingProfile).Contains(x.ID)
            && x.IDDestruction == id
            );
            if (profiles == null || profiles.Count() <= 0)
            {
                return new ServiceResultError("Không thể khôi phụ dữ liệu của quyết định");
            }
            foreach (var item in profiles)
            {
                item.InUsing = item.InUsing == (int)EnumCataloging.InUse.WaitDestructionUnUse ? (int)EnumCataloging.InUse.OffValue : (int)EnumCataloging.InUse.Using;
                item.Status = (int)EnumCataloging.Status.StorageApproved;
            }
            await _dasRepo.CatalogingProfile.UpdateAsync(profiles);
            await _dasRepo.SaveAync();
            //Doc
            var docs = await _dasRepo.CatalogingDoc.GetAllListAsync(x => x.Status == (int)EnumDocCollect.Status.Destroyed && profiles.Select(p => p.ID).Contains(x.IDCatalogingProfile));
            if (docs != null)
            {
                foreach (var item in docs)
                {
                    item.Status = (int)EnumDocCollect.Status.Complete;

                }
                await _dasRepo.CatalogingDoc.UpdateAsync(docs);
                await _dasRepo.SaveAync();
            }

            await _dasRepo.DestructionProfile.UpdateAsync(exist);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Đã khôi phục dữ liệu thành công");
        }
        #endregion Restore Destruction

        #endregion DestructionProfile

        #region ViewDetailProfile
        public async Task<VMIndexCatalogingDoc> CatalogingDocIndex(CatalogingDocCondition condition, bool isExport = false)
        {
            // GetProfile
            var profile = await GetCatalogingProfile(condition.IDProfile);
            if (profile == null)
            {
                return null;
            }
            //GetDoc
            var pagDoc = await GetCatalogingDocs(condition, isExport);
            var idDocTypelist = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var catalogingDocField = await GetCatalogingDocFieldsByIDs(pagDoc.Select(x => x.ID));
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                //Dictionary
                doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
            }
            return new VMIndexCatalogingDoc
            {
                VMCatalogingProfile = profile,
                VMCatalogingDocs = pagDoc,
                DictUsers = await GetDictUsers(),
                DictExpiryDate = await GetDictExpiryDate(),
                DictAgencies = await GetDictAgencies(),
                DictProfileTemplate = await GetDictProfileTemplate(profile.IDProfileTemplate),
                DictLanguage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString()),
                DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString()),
                CatalogingDocCondition = condition,
                VMDocTypeFields = docTypeFields,
                VMDocTypes = docTypes,
            };

        }
        public async Task<VMCatalogingDocCreate> GetDocCollect(int IDDoc)
        {
            var temp = from doc in _dasRepo.CatalogingDoc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMCatalogingDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMCatalogingProfile = await GetCatalogingProfile(model.IDCatalogingProfile);
            if (model.VMCatalogingProfile == null)
            {
                return null;
            }
            model.VMDocTypes = await GetDocTypes();
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));            
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMCatalogingDocFields = await GetDocFieldsByID(IDDoc);

            foreach (var item in model.VMCatalogingDocFields)
            {
                var field = model.VMDocTypeFields.FirstOrDefault(n => n.ID == item.IDDocTypeField);
                if (field == null)
                    continue;
                switch (field.Code)
                {
                    case "FileCode":
                    case "Identifier":
                    case "Organld":
                    case "FileCatalog":
                    case "FileNotation":
                    case "OrganName":
                        item.IsReadonly = true;
                        break;
                }
            }
            return model;
        }

        #region Private
        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }
        private async Task<List<VMCatalogingDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                       && df.IDCatalogingDoc == IDDoc
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }

        private async Task<List<VMDocType>> GetDocTypes()
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       where dc.IDOrgan == _userPrincipalService.IDOrgan
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }
        private async Task<VMCatalogingProfile> GetCatalogingProfile(int idProfile)
        {
            if (_userPrincipalService == null || _userPrincipalService.IDAgency == 0)
                return null;
            var foreverValue = await GetForeverValue();
            var iqExpiryDate = from ex in _dasRepo.ExpiryDate.GetAll().Where(x => x.Status == (int)EnumExpiryDate.Status.Active && x.Value < foreverValue)
                               select ex;
            var iqrExpiry = from pp in _dasRepo.CatalogingProfile.GetAll()
                            join ex in iqExpiryDate on pp.IDExpiryDate equals ex.ID
                            where pp.Status == (int)EnumCataloging.Status.StorageApproved
                            && pp.ID == idProfile
                            && pp.IDOrgan == _userPrincipalService.IDOrgan
                            && (pp.InUsing != (int)EnumCataloging.InUse.Using || (pp.ApprovedDate ?? DateTime.Now).AddYears(ex.Value) < DateTime.Now)
                            select _mapper.Map<VMCatalogingProfile>(pp);
            var rs = await iqrExpiry.FirstOrDefaultAsync();
            return rs;
        }
        private async Task<PaginatedList<VMCatalogingDoc>> GetCatalogingDocs(CatalogingDocCondition condition, bool isExport)
        {
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };
            string[] arrCodeType2 = { "Identifier" };
            string[] arrCodeType3 = { "Identifier" };
            var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == 1 && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == 2 && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == 3 && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.CatalogingDoc.GetAll()
                       join p in _dasRepo.CatalogingProfile.GetAll() on d.IDCatalogingProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDCatalogingDoc
                       where (d.Status == (int)EnumDocCollect.Status.Active || d.Status == (int)EnumDocCollect.Status.Complete)
                     && (d.IDCatalogingProfile == condition.IDProfile)
                    && (p.IDAgency == condition.IDAgency || condition.IDAgency == 0)
                       && (condition.ListStatusStr == null || condition.ListStatusStr.Count() == 0 || condition.ListStatusStr.Contains(d.Status.ToString()))
                       &&
                       (p.Status == (int)EnumCataloging.Status.Active
                       || p.Status == (int)EnumCataloging.Status.CollectComplete
                       || p.Status == (int)EnumCataloging.Status.Reject
                       || p.Status == (int)EnumCataloging.Status.WaitApprove
                       || p.Status == (int)EnumCataloging.Status.WaitApprove
                       || p.Status == (int)EnumCataloging.Status.StorageReject
                       || p.Status == (int)EnumCataloging.Status.StorageApproved)
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
                       orderby (g.Key.UpdatedDate ?? g.Key.CreateDate) descending
                       select new VMCatalogingDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDCatalogingProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           Status = g.Key.Status,
                           CreatedBy = g.Key.CreatedBy,
                           CreateDate = g.Key.CreateDate,
                           UpdatedDate = g.Key.UpdatedDate,
                           UpdatedBy = g.Key.UpdatedBy
                       };

            if (isExport)
            {
                var docs = await temp.ToListAsync();
                var vmDocs = _mapper.Map<List<VMCatalogingDoc>>(docs);
                return new PaginatedList<VMCatalogingDoc>(vmDocs, vmDocs.Count(), 1, vmDocs.Count());
            }
            else
            {
                var total = await temp.LongCountAsync();

                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                var vmDocs = _mapper.Map<List<VMCatalogingDoc>>(docs);
                return new PaginatedList<VMCatalogingDoc>(vmDocs, (int)total, condition.PageIndex, condition.PageSize);
            }
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
        #endregion Private
        #endregion ViewDetailProfile

        #region Cookie
        /// <summary>
        /// Đánh dấu hồ sơ thuộc quyết định tiêu hủy
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult> AddProfileToTicket(HttpRequest request, HttpResponse response, int IDprofile)
        {
            try
            {
                if (_userPrincipalService.UserId > 0)
                {
                    var profile = await _dasRepo.CatalogingProfile.FirstOrDefaultAsync(x => x.ID == IDprofile && x.Status == (int)EnumCataloging.Status.StorageApproved);
                    if (profile == null)
                        return new ServiceResultError("Hồ sơ không còn tồn tại");

                    var tkValue = AddToTicket(request, response, IDprofile);
                    return new ServiceResultSuccess($"Đã thêm hồ sơ {profile.Title} vào quyết định tiêu hủy", new
                    {
                        tkvalue = tkValue
                    });
                }
                else
                {
                    return new ServiceResultError("Chưa đăng nhập");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        public async Task<ServiceResult> AddProfileToTicket(HttpRequest request, HttpResponse response, List<int> ids)
        {
            try
            {
                if (_userPrincipalService.UserId > 0)
                {
                    var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(x => ids.Contains(x.ID) && x.Status == (int)EnumCataloging.Status.StorageApproved);
                    if (profiles == null || profiles.Count() == 0)
                        return new ServiceResultError("Hồ sơ không còn tồn tại");

                    var tkValue = AddToTickets(request, response, profiles.Select(x => x.ID).ToList());
                    return new ServiceResultSuccess($"Đã thêm {tkValue.Count} hồ sơ vào quyết định tiêu hủy", new
                    {
                        tkvalue = tkValue
                    });
                }
                else
                {
                    return new ServiceResultError("Chưa đăng nhập");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }

        public ServiceResult RemoveTicketItem(HttpRequest request, HttpResponse response, int IDprofile)
        {
            var tkValue = GetDestructionTicket(request);
            if (!tkValue.Contains(IDprofile))
            {
                return new ServiceResultError("Hồ sơ không thuộc quyết định tiêu hủy");
            }
            else
            {
                tkValue.Remove(IDprofile);
                UpdateTicket(response, tkValue);
                return new ServiceResultSuccess($"Đã xóa hồ sơ khỏi quyết định tiêu hủy", new
                {
                    tkvalue = tkValue
                });
            }
        }
        public ServiceResult RemoveAllTicket(HttpRequest request, HttpResponse response)
        {
            var ccName = CommonConst.DestructionProfile + _userPrincipalService.UserId;
            if (request.Cookies[ccName] != null)
            {
                response.Cookies.Delete(ccName);
            }
            return new ServiceResultSuccess("Bỏ chọn tất cả thành công");
        }
        public List<int> GetDestructionTicket(HttpRequest request)
        {
            var ccValue = request.Cookies[CommonConst.DestructionProfile + _userPrincipalService.UserId];
            return (Utils.Deserialize<List<int>>(ccValue) ?? new List<int>()).Select(n => n).Distinct().ToList();
        }
        private List<int> AddToTicket(HttpRequest request, HttpResponse response, int idprofile)
        {
            var ccName = CommonConst.DestructionProfile + _userPrincipalService.UserId;
            if (request.Cookies.ContainsKey(ccName))
            {
                var tkValue = GetDestructionTicket(request);
                if (!tkValue.Contains(idprofile))
                {
                    tkValue.Add(idprofile);
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(7);
                    response.Cookies.Append(ccName, Utils.Serialize(tkValue), option);
                }
                return tkValue;
            }
            else
            {
                //Create new 
                var newtk = new List<int> { idprofile };
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(7);
                response.Cookies.Append(ccName, Utils.Serialize(newtk), option);
                return newtk;
            }
        }
        private List<int> AddToTickets(HttpRequest request, HttpResponse response, List<int> ids)
        {
            var ccName = CommonConst.DestructionProfile + _userPrincipalService.UserId;
            if (request.Cookies.ContainsKey(ccName))
            {
                var tkValue = GetDestructionTicket(request);
                var insert = ids.Where(x => !tkValue.Contains(x)).ToList();
                if (insert.Count > 0)
                {
                    tkValue.AddRange(insert);
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(7);
                    response.Cookies.Append(ccName, Utils.Serialize(tkValue), option);
                }
                return tkValue;
            }
            else
            {
                //Create new 
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(7);
                response.Cookies.Append(ccName, Utils.Serialize(ids), option);
                return ids;
            }
        }
        private List<int> UpdateTicket(HttpResponse response, List<int> newValue)
        {
            var ccName = CommonConst.DestructionProfile + _userPrincipalService.UserId;
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(7);
            response.Cookies.Append(ccName, Utils.Serialize(newValue), option);
            return newValue;
        }
        #endregion Cookie
        #region Private
        private async Task<int> GetForeverValue()
        {
            int value = int.TryParse((await _dasRepo.SystemConfig.GetConfigByCode(SystemConfigConst.MAX_VALUE_EXPIRYDATE)).ToString(), out int max) ? max : 1000;
            return value;
        }
        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a => a.Status == (int)EnumOrgan.Status.Active
            && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
                       && a.IDOrgan == _userPrincipalService.IDOrgan)).ToDictionary(n => n.ID, n => n.FondName);
        }
        private async Task<Dictionary<int, string>> GetDictAgencies(int parentId = -1)
        {
            return (await _dasRepo.Agency.GetAllListAsync(n =>
            n.Status == (int)EnumAgency.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan
            && (parentId < 0 || n.ParentId == parentId)
            )).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }

        public async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(n => n.Status == (int)EnumExpiryDate.Status.Active)
            ).OrderBy(x=> x.Value).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictUserApproved()
        {
            return (await _dasRepo.User.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active
            && n.IDOrgan == _userPrincipalService.IDOrgan)).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictUsers()
        {
            return (await _dasRepo.User.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active
            && n.IDOrgan == _userPrincipalService.IDOrgan)).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            var cates = (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan));

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
        private async Task<PaginatedList<VMPlanProfile>> GetProfilesByList(List<int> ids)
        {
            var temp = from pp in _dasRepo.CatalogingProfile.GetAll()
                       where pp.Status == (int)EnumCataloging.Status.StorageApproved
                       && pp.InUsing != (int)EnumCataloging.InUse.WaitDestructionExpiry
                       && pp.InUsing != (int)EnumCataloging.InUse.WaitDestructionUnUse
                       && ids.Contains(pp.ID)
                       select new VMPlanProfile
                       {
                           ID = pp.ID,
                           IDOrgan = pp.IDOrgan,
                           IDAgency = pp.IDAgency,
                           IDPlan = pp.IDPlan,
                           Type = pp.Type,
                           FileCode = pp.FileCode,
                           IDStorage = pp.IDStorage,
                           IDShelve = pp.IDShelve,
                           IDBox = pp.IDBox,
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
                           IDProfileCategory = pp.IDProfileCategory,
                           //ExpiryDate = DateTime.Now,
                           StatusDestruction = pp.InUsing == (int)EnumCataloging.InUse.Using ? (int)EnumCataloging.StatusDestruction.Expiry : (int)EnumCataloging.StatusDestruction.OffValue
                       };
            //list Profile
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;


            var list = await temp.ToListAsync();
            return new PaginatedList<VMPlanProfile>(list, (int)total, -1, (int)total);

        }
    
        #endregion Private
    }
}
