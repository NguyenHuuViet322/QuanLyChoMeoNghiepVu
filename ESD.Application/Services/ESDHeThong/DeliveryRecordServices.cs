using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Application.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Domain.Enums;
using ESD.Domain.Models.DASNotify;
using ESD.Application.Constants;
using ESD.Domain.Interfaces.DASNotify;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Infrastructure.Notifications;
using ESD.Utility.CacheUtils;
using Newtonsoft.Json;

namespace ESD.Application.Services
{
    public class DeliveryRecordServices : BaseMasterService, IDeliveryRecordServices
    {
        #region Construcor
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IHubNotificationHelper _hubNotificationHelper;
        private readonly ICacheManagementServices _cacheManagementServices;
        public DeliveryRecordServices(IDasRepositoryWrapper dasRepository
            , IDasNotifyRepositoryWrapper dasNotifyRepository
            , IMapper mapper
            , IUserPrincipalService iUserPrincipalService
            , ICacheManagementServices cacheManagementServices
            , IHubNotificationHelper hubNotificationHelper) : base(dasRepository, dasNotifyRepository)
        {
            _mapper = mapper;
            _userPrincipalService = iUserPrincipalService;
            _cacheManagementServices = cacheManagementServices;
            _hubNotificationHelper = hubNotificationHelper;
        }
        #endregion
        #region Create & Search
        public async Task<ServiceResult> Create(VMDeliveryRecord vmRecord)
        {
            //check exist unique field
            var listExist = await _dasRepo.DeliveryRecord.GetAll().Where(m => (m.Status != (int)EnumDeliveryRecord.Status.Inactive)).ToListAsync();
            if (IsExisted(listExist))
                if (IsExisted(listExist.Where(m => m.Code == vmRecord.Code)))
                    return new ServiceResultError("Mã số biên bản đã tồn tại!");

            //update data
            vmRecord.Status = (int)EnumDeliveryRecord.Status.Active;
            var record = _mapper.Map<DeliveryRecord>(vmRecord);
            await _dasRepo.DeliveryRecord.InsertAsync(record);
            await _dasRepo.SaveAync();
            if (record.ID == 0)
                return new ServiceResultError("Thêm mới biên bản không thành công");
            return new ServiceResultSuccess("Thêm mới biên bản thành công");
        }
        public async Task<ServiceResult> Create(DeliveryRecord model)
        {
            await _dasRepo.DeliveryRecord.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }

        public async Task<PaginatedList<VMDeliveryRecord>> SearchListConditionPagging(DeliveryRecordCondition condition)
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            DateTime conditionFromDate = DateTime.MinValue;
            DateTime conditionToDate = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(condition.FromDate))
                conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(condition.ToDate))
                conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var temp = from dr in _dasRepo.DeliveryRecord.GetAll()
                       join p in _dasRepo.Plan.GetAll() on dr.IDPlan equals p.ID into joinPlan
                       from jp in joinPlan.DefaultIfEmpty()
                       join a in _dasRepo.Agency.GetAll() on dr.IDAgency equals a.ID into joinAgency
                       from ja in joinAgency.DefaultIfEmpty()
                       join su in _dasRepo.User.GetAll() on dr.IDSendUser equals su.ID into joinSendUser
                       from jsu in joinSendUser.DefaultIfEmpty()
                       join ru in _dasRepo.User.GetAll() on dr.IDReceiveUser equals ru.ID into joinReceiveUser
                       from jru in joinReceiveUser.DefaultIfEmpty()
                       where dr.Status != (int)EnumDeliveryRecord.Status.Inactive
                       && (condition.Keyword.IsEmpty() || dr.Title.Contains(condition.Keyword) || dr.Code.Contains(condition.Keyword) || jp.Name.Contains(condition.Keyword))
                       && (condition.IDAgency == 0 || ja.ID == condition.IDAgency)
                       && ((userData.IDOrgan != 0 && ja.IDOrgan == userData.IDOrgan))
                       && ((((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty()))
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionFromDate) >= 0) && (DateTime.Compare(dr.RecordCreateDate, conditionToDate) <= 0))))
                       orderby dr.Status descending, dr.CreateDate descending
                       select new VMDeliveryRecord
                       {
                           ID = dr.ID,
                           Code = dr.Code,
                           Title = dr.Title,
                           RecordCreateDate = dr.RecordCreateDate,
                           RecordCreateDateStr = dr.RecordCreateDate.Day.ToString() + "/" + dr.RecordCreateDate.Month.ToString() + "/" + dr.RecordCreateDate.Year.ToString(),
                           IDPlan = jp.ID,
                           PlanName = jp.Name,
                           IDAgency = ja.ID,
                           AgencyName = ja.Name,
                           IDSendUser = jsu.ID,
                           AccountSendUser = jsu.AccountName,
                           NameSendUser = jsu.Name,
                           IDReceiveUser = jru.ID,
                           AccountReceiveUser = jru.AccountName,
                           NameReceiveUser = jru.Name,
                           Status = dr.Status,
                           DocumentName = dr.DocumentName,
                           DocumentTime = dr.DocumentTime,
                           DocumentReceiveStatus = dr.DocumentReceiveStatus,
                           lstDeliveryPlanProfile = dr.lstDeliveryPlanProfile
                       };
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            List<int> orderStatus = new List<int> {
                (int)EnumDeliveryRecord.Status.Active,
                (int)EnumDeliveryRecord.Status.Reject,
                (int)EnumDeliveryRecord.Status.WaitingReceive,
                (int)EnumDeliveryRecord.Status.Complete
            };
            var lstTemp = await temp.ToListAsync();
            foreach (var item in lstTemp)
            {
                item.TotalProfiles = GetTotalProfileInDeliveryRecord(item);
            }
            var temprs = lstTemp.OrderBy(x => orderStatus.IndexOf(x.Status));

            var result = temprs.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize);
            if (!IsExisted(result))
                return null;

            PaginatedList<VMDeliveryRecord> model = new PaginatedList<VMDeliveryRecord>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }
        public async Task<PaginatedList<VMDeliveryRecord>> SearchListReceiveConditionPagging(DeliveryRecordCondition condition)
        {
            if (_userPrincipalService == null)
                return null;

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            DateTime conditionFromDate = DateTime.MinValue;
            DateTime conditionToDate = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(condition.FromDate))
                conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(condition.ToDate))
                conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var temp = from dr in _dasRepo.DeliveryRecord.GetAll().Where(dr => ((userData.IDOrgan == 0 || dr.IDSendUser == _userPrincipalService.UserId) && (dr.Status == (int)EnumDeliveryRecord.Status.WaitingReceive || dr.Status == (int)EnumDeliveryRecord.Status.Complete)))
                       join p in _dasRepo.Plan.GetAll() on dr.IDPlan equals p.ID into joinPlan
                       from jp in joinPlan.DefaultIfEmpty()
                       join ru in _dasRepo.User.GetAll() on dr.IDSendUser equals ru.ID into joinSendUser
                       from jsu in joinSendUser.DefaultIfEmpty()
                       join agency in _dasRepo.Agency.GetAll() on jsu.IDAgency equals agency.ID
                       join organ in _dasRepo.Organ.GetAll() on agency.IDOrgan equals organ.ID
                       where dr.Status != (int)EnumDeliveryRecord.Status.Inactive
                       && (condition.Keyword.IsEmpty() || dr.Title.Contains(condition.Keyword) || dr.Code.Contains(condition.Keyword) || jp.Name.Contains(condition.Keyword))
                       && ((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(dr.RecordCreateDate, conditionFromDate) >= 0) && (DateTime.Compare(dr.RecordCreateDate, conditionToDate) <= 0)))
                       orderby dr.Status descending, dr.CreateDate
                       select new VMDeliveryRecord
                       {
                           ID = dr.ID,
                           Code = dr.Code,
                           Title = dr.Title,
                           RecordCreateDate = dr.RecordCreateDate,
                           RecordCreateDateStr = dr.RecordCreateDate.Day.ToString() + "/" + dr.RecordCreateDate.Month.ToString() + "/" + dr.RecordCreateDate.Year.ToString(),
                           IDPlan = jp.ID,
                           PlanName = jp.Name,
                           IDReceiveUser = jsu.ID,
                           AccountReceiveUser = jsu.AccountName,
                           NameReceiveUser = jsu.Name,
                           Status = dr.Status,
                           DocumentName = dr.DocumentName,
                           DocumentTime = dr.DocumentTime.ToString(),
                           DocumentReceiveStatus = dr.DocumentReceiveStatus,
                           AgencyName = agency.Name,
                           OrganName = organ.Name
                       };
            var total = await temp.LongCountAsync();
            if (total <= 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            List<int> orderStatus = new List<int> {
                (int)EnumDeliveryRecord.Status.Active,
                (int)EnumDeliveryRecord.Status.Reject,
                (int)EnumDeliveryRecord.Status.WaitingReceive,
                (int)EnumDeliveryRecord.Status.Complete
            };
            var lstTemp = await temp.ToListAsync();
            foreach (var item in lstTemp)
            {
                item.TotalProfiles = GetTotalProfileInDeliveryRecord(item);
            }
            var temprs = lstTemp.OrderBy(x => orderStatus.IndexOf(x.Status));

            var result = temprs.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize);
            if (!IsExisted(result))
                return null;

            PaginatedList<VMDeliveryRecord> model = new PaginatedList<VMDeliveryRecord>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            return model;
        }

        public async Task<DeliveryRecord> Get(object id)
        {
            return await _dasRepo.DeliveryRecord.GetAsync(id);
        }

        public async Task<IEnumerable<DeliveryRecord>> Gets()
        {
            throw new NotImplementedException();
        }

        public async Task<VMDeliveryRecord> GetDeliveryRecord(int id)
        {
            if (_userPrincipalService == null)
                return null;
            //Get model
            var temp = from dr in _dasRepo.DeliveryRecord.GetAll()
                       join p in _dasRepo.Plan.GetAll() on dr.IDPlan equals p.ID into joinPlan
                       from jp in joinPlan.DefaultIfEmpty()
                       join a in _dasRepo.Agency.GetAll() on dr.IDAgency equals a.ID into joinAgency
                       from ja in joinAgency.DefaultIfEmpty()
                       join o in _dasRepo.Organ.GetAll() on ja.IDOrgan equals o.ID into joinOrgan
                       from jo in joinOrgan.DefaultIfEmpty()
                       where dr.Status != (int)EnumDeliveryRecord.Status.Inactive
                       && jp.Status != (int)EnumProfilePlan.Status.InActive
                       && ja.Status != (int)EnumCommon.Status.InActive
                       && dr.ID == id && jo.Status != (int)EnumCommon.Status.InActive
                       select new VMDeliveryRecord
                       {
                           ID = dr.ID,
                           Code = dr.Code,
                           Title = dr.Title,
                           RecordCreateDate = dr.RecordCreateDate,
                           RecordCreateDateStr = dr.RecordCreateDate.Day.ToString() + "/" + dr.RecordCreateDate.Month.ToString() + "/" + dr.RecordCreateDate.Year.ToString(),
                           IDPlan = jp.ID,
                           PlanName = jp.Name,
                           IDAgency = ja.ID,
                           AgencyName = ja.Name,
                           IDSendUser = dr.IDSendUser,
                           IDReceiveUser = dr.IDReceiveUser,
                           Status = dr.Status,
                           DocumentName = dr.DocumentName,
                           DocumentTime = dr.DocumentTime.ToString(),
                           DocumentReceiveStatus = dr.DocumentReceiveStatus,
                           Department = jo.Name,
                           IDTemplate = dr.IDTemplate,
                           lstDeliveryPlanProfile = dr.lstDeliveryPlanProfile,
                       };
            var count = await temp.LongCountAsync();
            if (count == 0)
                return null;
            var lstItem = await temp.ToListAsync();
            foreach (var item in lstItem)
            {
                //Get người giao, người nhận
                var lstUser = await _dasRepo.User.GetAllListAsync(x => (x.ID == item.IDSendUser || x.ID == item.IDReceiveUser) && x.Status != (int)EnumCommon.Status.InActive);

                if (!IsExisted(lstUser))
                {
                    item.NameSendUser = "";
                    item.SenderPosition = "";
                    item.AccountSendUser = "";
                    item.NameReceiveUser = "";
                    item.ReceiverPosition = "";
                    item.AccountReceiveUser = "";
                }
                else
                {
                    //Get người giao
                    var sendUser = lstUser.Where(x => x.ID == item.IDSendUser);
                    if (!IsExisted(sendUser))
                    {
                        item.NameSendUser = "";
                        item.SenderPosition = "";
                        item.AccountSendUser = "";
                    }
                    else
                    {
                        item.NameSendUser = sendUser.FirstOrDefault().Name;
                        item.AccountSendUser = sendUser.FirstOrDefault().AccountName;
                        var position = await _dasRepo.Position.GetAllListAsync(x => x.ID == sendUser.FirstOrDefault().IDPosition && x.Status != (int)EnumCommon.Status.InActive);
                        if (!IsExisted(position))
                            item.SenderPosition = "";
                        else
                            item.SenderPosition = position.FirstOrDefault().Name;
                    }
                    //Get người nhận
                    var receiveUser = lstUser.Where(x => x.ID == item.IDReceiveUser);
                    if (!IsExisted(receiveUser))
                    {
                        item.NameReceiveUser = "";
                        item.ReceiverPosition = "";
                        item.AccountReceiveUser = "";
                    }
                    else
                    {
                        item.NameReceiveUser = receiveUser.FirstOrDefault().Name;
                        item.AccountReceiveUser = receiveUser.FirstOrDefault().AccountName;
                        var position = await _dasRepo.Position.GetAllListAsync(x => x.ID == receiveUser.FirstOrDefault().IDPosition && x.Status != (int)EnumCommon.Status.InActive);
                        if (!IsExisted(position))
                            item.ReceiverPosition = "";
                        else
                            item.ReceiverPosition = position.FirstOrDefault().Name;
                    }
                }

                //Get thông tin hồ sơ từ model
                if (!string.IsNullOrWhiteSpace(item.lstDeliveryPlanProfile))
                {
                    var lstIDPP = new List<int>();
                    if (!item.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                    {
                        lstIDPP.Add(int.Parse(item.lstDeliveryPlanProfile));
                    }
                    else
                    {
                        lstIDPP.AddRange(JsonConvert.DeserializeObject<List<int>>(item.lstDeliveryPlanProfile));
                    }
                    var pp = await _dasRepo.PlanProfile.GetAllListAsync(x => lstIDPP.Contains(x.ID) && x.Status != (int)EnumProfilePlan.Status.InActive);
                    item.TotalDocument = pp.Count();
                    int totalFileInDoc = 0;
                    foreach (var doc in pp)
                    {
                        var file = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == doc.ID && x.Status != (int)EnumDocCollect.Status.InActive);
                        if (IsExisted(file))
                            totalFileInDoc += file.Count();
                    }

                    item.TotalDocumentFile = totalFileInDoc;

                }
                //var document = await _dasRepo.PlanProfile.GetAllListAsync(x => x.IDPlan == item.IDPlan && x.IDAgency == item.IDAgency && x.Status != (int)EnumProfilePlan.Status.InActive);
                //if (!IsExisted(document))
                //{
                //    item.TotalDocument = 0;
                //    item.TotalDocumentFile = 0;
                //}
                //else
                //{
                //    item.TotalDocument = document.Count();
                //    int totalFileInDoc = 0;
                //    foreach (var doc in document)
                //    {
                //        var file = await _dasRepo.Doc.GetAllListAsync(x => x.IDProfile == doc.ID);
                //        if (IsExisted(file))
                //            totalFileInDoc += file.Count();
                //    }
                //
                //    item.TotalDocumentFile = totalFileInDoc;
                //}

                item.VMPlanProfiles = await GetPlanProfiles(item);
            }
            return lstItem[0];
        }
        public async Task<ServiceResult> GetAgencyByIdPlan(int id)
        {
            if (id == 0)
            {
                var agency = await _dasRepo.Agency.GetAllListAsync(x => x.Status != (int)EnumAgency.Status.InActive);
                return new ServiceResultSuccess(agency.ToList());
            }
            var planProfile = await _dasRepo.PlanProfile.GetAllListAsync(x => x.IDPlan == id && x.Status == (int)EnumProfilePlan.Status.Active);
            if (!IsExisted(planProfile))
                return new ServiceResultError();
            var lstIDAgency = planProfile.ToList().Select(x => x.IDAgency);
            if (!IsExisted(lstIDAgency))
                return new ServiceResultError();
            var user = await _cacheManagementServices.GetUserDataAndSetCache();
            var temp = await _dasRepo.Agency.GetAllListAsync(x => lstIDAgency.Contains(x.ID) && x.Status == (int)EnumAgency.Status.Active && x.IDOrgan == user.IDOrgan);
            if (!IsExisted(temp))
                return new ServiceResultError();
            else
                return new ServiceResultSuccess(temp.ToList());
        }
        public async Task<ServiceResult> GetPlanByIdAgency(int id)
        {
            var planProfile = await _dasRepo.PlanProfile.GetAllListAsync(x => x.IDAgency == id && x.Status == (int)EnumProfilePlan.Status.ArchiveApproved);
            if (!IsExisted(planProfile))
                return new ServiceResultError();

            var lstIDPlan = planProfile.ToList().Select(x => x.IDAgency);
            if (!IsExisted(lstIDPlan))
                return new ServiceResultError();
            var user = await _cacheManagementServices.GetUserDataAndSetCache();
            var temp = await _dasRepo.Plan.GetAllListAsync(x => lstIDPlan.Contains(x.ID) && x.Status == (int)EnumPlan.Status.Approved && x.IDOrgan == user.IDOrgan);
            if (!IsExisted(temp))
                return new ServiceResultError();
            else
                return new ServiceResultSuccess(temp.ToList());
        }
        public async Task<ServiceResult> GetUserByIdAgency(int id)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var user = await _dasRepo.User.GetAllListAsync(x => x.IDAgency == id && x.Status == (int)EnumCommon.Status.Active && userData.IDOrgan == x.IDOrgan);
            if (!IsExisted(user))
                return new ServiceResultError();
            else
                return new ServiceResultSuccess(user.ToList());
        }
        public async Task<VMDeliveryRecord> GetDeliveryRecordByIDPlanProfile(int id)
        {
            var pp = await _dasRepo.PlanProfile.GetAsync(id);
            if (pp == null || pp.Status == (int)EnumProfilePlan.Status.InActive)
                return null;
            var delivery = await _dasRepo.DeliveryRecord.GetAllListAsync(x => x.Status != (int)EnumDeliveryRecord.Status.Inactive && !string.IsNullOrWhiteSpace(x.lstDeliveryPlanProfile));
            if (delivery.IsEmpty())
                return null;
            var intIDDR = 0;
            foreach (var item in delivery)
            {
                if (!item.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                {
                    if (item.lstDeliveryPlanProfile.Equals(pp.ID.ToString())) ;
                    intIDDR = item.ID;
                }
                else
                {
                    if (JsonConvert.DeserializeObject<List<int>>(item.lstDeliveryPlanProfile).Contains(pp.ID))
                        intIDDR = item.ID;
                }
            }
            if (intIDDR == 0)
                return null;
            var record = await _dasRepo.DeliveryRecord.GetAsync(intIDDR);
            return _mapper.Map<VMDeliveryRecord>(record); 
        }
        public async Task<ServiceResult> GetArchiveApprovedPlanProfileByModel(VMDeliveryRecord model)
        {
            if (model == null || model.IDPlan == 0 || model.IDAgency == 0)
                return new ServiceResultError();
            var planProfile = await _dasRepo.PlanProfile.GetAllListAsync(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved && x.IDAgency == model.IDAgency && x.IDPlan == model.IDPlan);
            var delivery = await _dasRepo.DeliveryRecord.GetAllListAsync(x => x.ID != model.ID && x.Status != (int)EnumDeliveryRecord.Status.Inactive && x.IDPlan == model.IDPlan && x.IDAgency == model.IDAgency && !string.IsNullOrWhiteSpace(x.lstDeliveryPlanProfile));

            //Kiểm tra các hồ sơ đã được lập biên bản
            if (!IsExisted(delivery))
            {
                var temp = _mapper.Map<IEnumerable<VMPlanProfile>>(planProfile);
                return new ServiceResultSuccess(temp);
            }

            //Lấy ID các hồ sơ đã được lập biên bản
            var lstIDPP = new List<int>();
            var lstID = delivery.Select(x => x.lstDeliveryPlanProfile);
            foreach (var item in lstID)
            {
                if (!item.Substring(0, 1).Equals("["))
                {
                    lstIDPP.Add(int.Parse(item));
                }
                else
                {
                    var lst = JsonConvert.DeserializeObject<List<int>>(item);
                    lstIDPP.AddRange(lst);
                }
            }

            var result = _mapper.Map<IEnumerable<VMPlanProfile>>(planProfile.Where(x => !lstIDPP.Contains(x.ID)));
            return new ServiceResultSuccess(result);
        }

        public async Task<IEnumerable<Agency>> GetAgencyReadyForDelivery()
        {
            //Get thông tin các hồ sơ đã được nộp lưu
            var pp = await _dasRepo.PlanProfile.GetAllListAsync(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved);
            if (!IsExisted(pp))
                return null;

            //Get thông tin các hồ sơ đã được lập biên bản nhưng chưa bàn giao
            var delivery = await _dasRepo.DeliveryRecord.GetAllListAsync(x => x.Status == (int)EnumDeliveryRecord.Status.Active && !string.IsNullOrWhiteSpace(x.lstDeliveryPlanProfile));
            var lstIDPPDelivery = new List<int>();

            foreach (var item in delivery)
            {
                if (!item.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                    lstIDPPDelivery.Add(int.Parse(item.lstDeliveryPlanProfile));
                else
                    lstIDPPDelivery.AddRange(JsonConvert.DeserializeObject<List<int>>(item.lstDeliveryPlanProfile));
            }
            var lstAgency = pp.Where(x => !lstIDPPDelivery.Contains(x.ID)).Select(x => x.IDAgency).ToList();
            var user = await _cacheManagementServices.GetUserDataAndSetCache();
            var agency = await _dasRepo.Agency.GetAllListAsync(x => lstAgency.Contains(x.ID) && x.Status == (int)EnumAgency.Status.Active && x.IDOrgan == user.IDOrgan);

            return agency;
        }
        public async Task<IEnumerable<Agency>> GetAgencyReadyForEditDelivery()
        {
            //Get thông tin các hồ sơ đã được nộp lưu
            var pp = await _dasRepo.PlanProfile.GetAllListAsync(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved);
            if (!IsExisted(pp))
                return null;

            var lstAgency = pp.Select(x => x.IDAgency).ToList();
            var user = await _cacheManagementServices.GetUserDataAndSetCache();
            var agency = await _dasRepo.Agency.GetAllListAsync(x => lstAgency.Contains(x.ID) && x.Status == (int)EnumAgency.Status.Active && x.IDOrgan == user.IDOrgan);

            return agency;
        }
        #endregion
        #region Delete
        public async Task<ServiceResult> Delete(object id)
        {
            var record = await _dasRepo.DeliveryRecord.GetAsync(id);
            if (record == null || record.Status == (int)EnumDeliveryRecord.Status.Inactive)
                return new ServiceResultError("Không tồn tại biên bản này");
            record.Status = (int)EnumDeliveryRecord.Status.Inactive;
            await _dasRepo.DeliveryRecord.UpdateAsync(record);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa biên bản thành công");
        }
        #endregion
        #region Update
        public async Task<ServiceResult> ChangeRecordStatus(int id, int status)
        {
            var record = await _dasRepo.DeliveryRecord.GetAsync(id);
            if (record == null || record.ID == 0)
                return new ServiceResultError("Biên bản không tồn tại");
            record.Status = status;
            await _dasRepo.DeliveryRecord.UpdateAsync(record);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Bàn giao biên bản thành công");
        }

        public async Task<ServiceResult> Update(DeliveryRecord model)
        {
            await _dasRepo.DeliveryRecord.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật biên bản bàn giao thành công");
        }

        public async Task<ServiceResult> Update(VMDeliveryRecord vmModel)
        {
            var model = _mapper.Map<DeliveryRecord>(vmModel);
            await _dasRepo.DeliveryRecord.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật biên bản bàn giao thành công");
        }
        #endregion
        #region Function

        public async Task<ServiceResult> SendDeliveryRecord(int id, byte[] attachment = null, string attachmentName = "")
        {
            var dr = await _dasRepo.DeliveryRecord.GetAsync(id);
            if (dr == null || dr.Status == (int)EnumDeliveryRecord.Status.Inactive)
                return new ServiceResultError("Biên bản không tồn tại");
            dr.Status = (int)EnumDeliveryRecord.Status.Complete;
            await _dasRepo.DeliveryRecord.UpdateAsync(dr);
            await _dasRepo.SaveAync();

            if (!string.IsNullOrWhiteSpace(dr.lstDeliveryPlanProfile))
            {
                var lstIDPlanProfile = new List<int>();
                if (!dr.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                {
                    lstIDPlanProfile.Add(int.Parse(dr.lstDeliveryPlanProfile));
                }
                else
                {
                    var lst = JsonConvert.DeserializeObject<List<int>>(dr.lstDeliveryPlanProfile);
                    lstIDPlanProfile.AddRange(lst);
                }

                var temp = await _dasRepo.PlanProfile.GetAllListAsync(x => lstIDPlanProfile.Contains(x.ID));
                var lstPP = temp.ToList();
                foreach (var item in lstPP)
                {
                    item.Status = (int)EnumProfilePlan.Status.DeliveryComplete;
                }

                await _dasRepo.PlanProfile.UpdateAsync(lstPP);
                await _dasRepo.SaveAync();
            }

            var model = await GetDeliveryRecord(id);
            //Gửi email thông báo
            var body = "Ông/bà " + model.NameReceiveUser + " - " + model.Department + " bàn giao " + model.Title + " cho " + model.PlanName;
            var title = "[" + model.Department + "]" + "[" + model.NameReceiveUser + "]" + model.Title;
            var emailType = "DeliveryRecord";
            await _dasRepo.Email.SendEmailWithUser(body, title, dr.IDSendUser, emailType, attachment, attachmentName);

            ////get users by permission
            //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            //if (dict.IsEmpty())
            //    return new ServiceResultSuccess("Gửi biên bản thành công");

            //List<int> userIds = new List<int>();
            //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
            //foreach (var item in dict)
            //{
            //    var per = dict.GetValueOrDefault(item.Key);
            //    if (per.IsEmpty())
            //        continue;
            //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.NBBBG && p.Type == (int)EnumPermission.Type.Approve).ToList();
            //    if (existeds.IsEmpty())
            //        continue;
            //    userIds.Add(item.Key);
            //}
            //if (userIds.IsEmpty())
            //    return new ServiceResultSuccess("Gửi biên bản thành công");

            //string content = string.Format("{0} gửi biên bản bàn giao qua mail {1}", _userPrincipalService.UserName, dr.Title);

            ////insert notify to db
            //List<Notification> listNoti = new List<Notification>();
            //foreach (var userId in userIds)
            //{
            //    listNoti.Add(new Notification
            //    {
            //        UserId = userId,
            //        Content = content,
            //        IsRead = false,
            //        CreatedDate = DateTime.Now
            //    });
            //}
            //if (listNoti.IsEmpty())
            //    return new ServiceResultSuccess("Gửi biên bản thành công");
            //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            //await _dasNotifyRepo.SaveAync();

            //foreach (var userId in userIds)
            //{
            //    //send notify to userIds
            //    await _hubNotificationHelper.PushToUser(userId);
            //}

            return new ServiceResultSuccess("Gửi biên bản thành công");
        }

        public async Task<ServiceResult> RejectDeliveryRecord(int id, string note)
        {
            var dr = await _dasRepo.DeliveryRecord.GetAsync(id);
            if (dr == null || dr.Status == (int)EnumDeliveryRecord.Status.Inactive)
                return new ServiceResultError("Biên bản không tồn tại");
            //if (dr.IDSendUser != _userPrincipalService.UserId)
            //    return new ServiceResultError("Bạn không có quyền thực hiện hành động này");
            dr.Status = (int)EnumDeliveryRecord.Status.Reject;
            dr.Reason = note;
            await _dasRepo.DeliveryRecord.UpdateAsync(dr);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Từ chối biên bản thành công");
        }

        public async Task<ServiceResult> ApproveDeliveryRecord(int id)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var dr = await _dasRepo.DeliveryRecord.GetAsync(id);
            if (dr == null || dr.Status == (int)EnumDeliveryRecord.Status.Inactive)
                return new ServiceResultError("Biên bản không tồn tại");
            //if (dr.IDSendUser != _userPrincipalService.UserId)
            //    return new ServiceResultError("Bạn không có quyền thực hiện hành động này");
            dr.Status = (int)EnumDeliveryRecord.Status.Complete;
            await _dasRepo.DeliveryRecord.UpdateAsync(dr);
            await _dasRepo.SaveAync();

            ////get users by permission
            //var dict = await _cache.GetCacheValueAsync<Dictionary<int, List<UserPermissionModel>>>(CacheConst.USER_PERMISSION);
            //if (dict.IsEmpty())
            //    return new ServiceResultSuccess("Nhận biên bản thành công");

            //List<int> userIds = new List<int>();
            //List<UserPermissionModel> existeds = new List<UserPermissionModel>();
            //foreach (var item in dict)
            //{
            //    var per = dict.GetValueOrDefault(item.Key);
            //    if (per.IsEmpty())
            //        continue;
            //    existeds = per.Where(p => p.CodeModule == (int)EnumModule.Code.QLBBBG && p.Type == (int)EnumPermission.Type.Approve).ToList();
            //    if (existeds.IsEmpty())
            //        continue;
            //    userIds.Add(item.Key);
            //}
            //if (userIds.IsEmpty())
            //    return new ServiceResultSuccess("Nhận biên bản thành công");

            //string organName = string.Empty;
            //var organ = await _dasRepo.Organ.GetAsync(userData.IDOrgan);
            //if (organ.IsNotEmpty())
            //    organName = organ.Name;
            //string content = string.Format("{0} {1} gửi yêu cầu phê duyệt hồ sơ lưu kho {1}", organName, _userPrincipalService.UserName, dr.Title);

            ////insert notify to db
            //List<Notification> listNoti = new List<Notification>();
            //foreach (var userId in userIds)
            //{
            //    listNoti.Add(new Notification
            //    {
            //        UserId = userId,
            //        Content = content,
            //        IsRead = false,
            //        CreatedDate = DateTime.Now
            //    });
            //}
            //if (listNoti.IsEmpty())
            //    return new ServiceResultSuccess("Nhận biên bản thành công");
            //await _dasNotifyRepo.Notification.InsertAsync(listNoti);
            //await _dasNotifyRepo.SaveAync();

            //foreach (var userId in userIds)
            //{
            //    //send notify to userIds
            //    await _hubNotificationHelper.PushToUser(userId);
            //}

            return new ServiceResultSuccess("Nhận biên bản thành công");
        }

        public async Task<ServiceResult> SendListRecord(int[] ids, Dictionary<int, byte[]> dictAttachment = null, Dictionary<int, string> dictAttachmentName = null)
        {
            var lstDR = await _dasRepo.DeliveryRecord.GetAllListAsync(n => ids.Contains(n.ID));

            if (!IsExisted(lstDR))
                return new ServiceResultError("Không tìm thấy biên bản phù hợp");

            if (_userPrincipalService == null || _userPrincipalService.UserId == 0)
                return new ServiceResultError("Bạn không có quyền thực hiện hành động này");

            var names = new List<string>();
            if (lstDR.Any(n => n.Status != (int)EnumDeliveryRecord.Status.Active))
            {
                foreach (var item in lstDR)
                {
                    if (item.Status != (int)EnumDeliveryRecord.Status.Active)
                        names.Add(item.Title);
                }
                if (names.Count > 0)
                    return new ServiceResultError($"Biên bản {string.Join(", ", names)} đã được bàn giao!");
            }
            else
            {
                //foreach (var item in lstDR)
                //{
                //    if (_userPrincipalService.UserId != item.IDReceiveUser)
                //        names.Add(item.Title);
                //}
                //if (names.Count > 0)
                //    return new ServiceResultError($"Bạn không có quyền gửi biên bản {string.Join(", ", names)} !");
            }

            foreach (var item in lstDR)
            {
                item.Status = (int)EnumDeliveryRecord.Status.Complete;
            }

            await _dasRepo.DeliveryRecord.UpdateAsync(lstDR);
            await _dasRepo.SaveAync();

            //Send Email with list user
            foreach (var item in lstDR)
            {
                var model = await GetDeliveryRecord(item.ID);
                var body = "Ông/bà " + model.NameReceiveUser + " - " + model.Department + " bàn giao " + model.Title + " cho " + model.PlanName;
                var title = "[" + model.Department + "]" + "[" + model.NameReceiveUser + "]" + model.Title;
                var emailType = "DeliveryRecord";
                byte[] attachment = null;
                var attachmentName = "";

                //Acctachment File
                if (dictAttachment != null)
                {
                    foreach (KeyValuePair<int, byte[]> key in dictAttachment)
                    {
                        if (key.Key == item.ID)
                        {
                            attachment = key.Value;
                        }
                    }
                }

                //Acctachment FileName
                if (dictAttachmentName != null)
                {
                    foreach (KeyValuePair<int, string> key in dictAttachmentName)
                    {
                        if (key.Key == item.ID)
                        {
                            attachmentName = key.Value;
                        }
                    }
                }

                await _dasRepo.Email.SendEmailWithUser(body, title, item.IDSendUser, emailType, attachment, attachmentName);
            }

            return new ServiceResultSuccess("Bàn giao biên bản thành công!");
        }

        public async Task<ServiceResult> DeleteListRecord(int[] ids)
        {
            var lstDR = await _dasRepo.DeliveryRecord.GetAllListAsync(n => ids.Contains(n.ID));

            if (!IsExisted(lstDR))
                return new ServiceResultError("Không tìm thấy biên bản phù hợp");

            if (_userPrincipalService == null || _userPrincipalService.UserId == 0)
                return new ServiceResultError("Bạn không có quyền thực hiện hành động này");

            var names = new List<string>();
            if (lstDR.Any(n => n.Status == (int)EnumDeliveryRecord.Status.WaitingReceive || n.Status == (int)EnumDeliveryRecord.Status.Complete))
            {
                foreach (var item in lstDR)
                {
                    if (item.Status == (int)EnumDeliveryRecord.Status.WaitingReceive || item.Status == (int)EnumDeliveryRecord.Status.Complete)
                        names.Add(item.Title);
                }
                if (names.Count > 0)
                    return new ServiceResultError($"Biên bản {string.Join(", ", names)} không được phép xóa!");
            }
            else
            {
                //foreach (var item in lstDR)
                //{
                //    if (_userPrincipalService.UserId != item.IDReceiveUser)
                //        names.Add(item.Title);
                //}
                //if (names.Count > 0)
                //    return new ServiceResultError($"Bạn không có quyền xóa biên bản {string.Join(", ", names)} !");
            }

            foreach (var item in lstDR)
            {
                item.Status = (int)EnumDeliveryRecord.Status.Inactive;
            }
            await _dasRepo.DeliveryRecord.UpdateAsync(lstDR);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Xóa biên bản thành công!");
        }
        #endregion
        #region Private Method
        private bool IsExisted(DeliveryRecord record)
        {
            if (record == null || record.ID == 0 || record.Status == (int)EnumDeliveryRecord.Status.Inactive)
                return false;
            return true;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }
        /// <summary>
        /// Get so hs theo ke hoach
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetTotalProfileInDeliveryRecord(VMDeliveryRecord model)
        {
            var dict = new Dictionary<int, int>();
            var count = 0;
            if (!string.IsNullOrWhiteSpace(model.lstDeliveryPlanProfile))
            {
                if (!model.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                {
                    count = 1;
                }
                else
                {
                    count = JsonConvert.DeserializeObject<List<int>>(model.lstDeliveryPlanProfile).Count;
                }
            }
            dict.Add(model.ID, count);
            return dict;
        }
        #endregion
        private async Task<PaginatedList<VMPlanProfile>> GetPlanProfiles(VMDeliveryRecord model)
        {
            var lstID = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.lstDeliveryPlanProfile))
            {
                if (!model.lstDeliveryPlanProfile.Substring(0, 1).Equals("["))
                {
                    lstID.Add(int.Parse(model.lstDeliveryPlanProfile));
                }
                else
                {
                    lstID = JsonConvert.DeserializeObject<List<int>>(model.lstDeliveryPlanProfile);
                }
            }
            if (lstID == null || lstID.Count == 0)
                return null;
            var temp = from pp in _dasRepo.PlanProfile.GetAll()
                       where pp.Status != (int)EnumProfilePlan.Status.InActive
                       && lstID.Contains(pp.ID)
                       orderby pp.ID descending
                       select _mapper.Map<VMPlanProfile>(pp);

            var profile = new List<VMPlanProfile>();

            var dictExpiryDates = await GetDictExpiryDate();
            profile = await temp.ToListAsync();
            foreach (var item in profile)
            {
                item.ProfileTime = Utils.DateToString(item.StartDate) + " - " + Utils.DateToString(item.EndDate);
                item.MaintenanceAndPageNumber = item.Maintenance + "/" + item.PageNumber;
                item.ExpiryDateName = dictExpiryDates.GetValueOrDefault(item.IDExpiryDate);
            }

            return new PaginatedList<VMPlanProfile>(profile, profile.Count(), 1, profile.Count());

            //if (model.PageIndex == -1)
            //{
            //    //Nopaging
            //    profile = await temp.ToListAsync();
            //    return new PaginatedList<VMPlanProfile>(profile, profile.Count, 1, profile.Count);
            //}
            //
            //var total = await temp.LongCountAsync();
            //int totalPage = (int)Math.Ceiling(total / (double)model.PageSize);
            //if (totalPage < model.PageIndex)
            //    model.PageIndex = 1;
            //
            //profile = await temp.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            //return new PaginatedList<VMPlanProfile>(profile, (int)total, model.PageIndex, model.PageSize);

        }
        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }
    }
}
