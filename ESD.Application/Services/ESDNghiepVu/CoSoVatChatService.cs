using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using ESD.Application.Interfaces;
using ESD.Application.Interfaces.ESDNghiepVu;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.Param.ESDNghiepVu;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.ESDNghiepVu;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Infrastructure.Repositories.ESDNghiepVu;
using ESD.Utility;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services.ESDNghiepVu
{
    public class CoSoVatChatService : BaseMasterService, ICoSoVatChatServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IModuleService _module;
        private readonly IDefaultDataService _defaultDataService;
        private readonly IHostApplicationLifetime _host;
        private ICacheManagementServices _cacheManagementServices;
        private IWebHostEnvironment _env;

        #endregion

        #region Ctor
        public CoSoVatChatService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager logger
            , IModuleService module
            , IDefaultDataService defaultDataService,
            IHostApplicationLifetime host
            , ICacheManagementServices cacheManagementServices, IESDNghiepVuRepositoryWrapper dasNghiepVuRepo,
            IWebHostEnvironment env) : base(dasRepository, dasNghiepVuRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _module = module;
            _cacheManagementServices = cacheManagementServices;
            _defaultDataService = defaultDataService;
            _host = host;
            _env = env;
        }


        #endregion

        #region Gets  

        public async Task<IEnumerable<CoSoVatChat>> GetsList()
        {
            var temp = from ct in _dasNghiepVuRepo.CoSoVatChat.GetAll()
                       orderby ct.ID descending
                       select ct;
            return await temp.ToListAsync();
        }
        public async Task<CoSoVatChat> Get(long id)
        {
            return await _dasNghiepVuRepo.CoSoVatChat.FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<VMIndexCoSoVatChat> SearchByConditionPagging(CoSoVatChatCondition condition)
        {
            var model = new VMIndexCoSoVatChat
            {
                SearchParam = condition
            };
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var lstDoiTuongSuDung = _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll().Where(p => p.IDDonViNghiepVu == condition.IDDoiTuongSuDung);
            var lstCoSoVatChat = _dasNghiepVuRepo.CoSoVatChat.GetAll().Where(p => lstDoiTuongSuDung.Any(t => t.IDCoSoVatChat == p.ID));

            if (condition.IDDoiTuongSuDung == 0 || condition.IDDoiTuongSuDung == null) lstCoSoVatChat = _dasNghiepVuRepo.CoSoVatChat.GetAll();

            var temp = from tb in lstCoSoVatChat
                       where (condition.Keyword.IsEmpty() || tb.Ten.Contains(condition.Keyword) || tb.Code.Contains(condition.Keyword))
                       orderby tb.UpdatedDate ?? tb.CreateDate descending
                       select _mapper.Map<VMCoSoVatChat>(tb);

            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
            {
                condition.PageIndex = 1;
            }
            var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            
            model.CoSoVatChats = new PaginatedList<VMCoSoVatChat>(result, (int)total, condition.PageIndex, condition.PageSize);
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();
            model.CoSoVatChat_DonVis = await _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAllListAsync();

            return model;
        }

        #endregion

        #region Create

        public async Task<VMUpdateCoSoVatChat> Create()
        {
            var model = new VMUpdateCoSoVatChat()
            {
            };

            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();

            return model;
        }
        

        public async Task<ServiceResult> Save(VMUpdateCoSoVatChat data)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();
                var coSoVatChat = Utils.Bind<CoSoVatChat>(data.KeyValue());
                var coSoVatChat_DonVi = Utils.Bind<CoSoVatChat_DonVi>(data.KeyValue());
                
                await ValidateData(data);
                await _dasNghiepVuRepo.CoSoVatChat.InsertAsync(coSoVatChat);
                await _dasNghiepVuRepo.SaveAync();

                coSoVatChat_DonVi.IDCoSoVatChat = coSoVatChat.ID;
                await _dasNghiepVuRepo.CoSoVatChat_DonVi.InsertAsync(coSoVatChat_DonVi);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Thêm danh mục cơ sở vật chất thành công");

            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                Guid.NewGuid().ToString();
                _logger.LogError(ex);
                return new ServiceResultError("Có lỗi khi thêm mới danh mục cơ sở vật chất");
            }
        }

        #endregion

        #region Update
        public async Task<VMUpdateCoSoVatChat> Update(long? id)
        {
            var coSoVatChat = await Get(id ?? 0);
            if (coSoVatChat == null || coSoVatChat.ID == 0)
            {
                throw new LogicException("Danh mục cơ sở vật chất không còn tồn tại");
            }
            var model = _mapper.Map<CoSoVatChat,VMUpdateCoSoVatChat>(coSoVatChat);
            model.IDDonViNghiepVu = (int)(from link in _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll()
                                     where link.IDCoSoVatChat == id
                                     select link).ToList().FirstOrDefault().IDDonViNghiepVu;
            model.DonViNghiepVus = await _dasNghiepVuRepo.DonViNghiepVu.GetAllListAsync();

            return model;
        }

        public async Task<ServiceResult> Change(VMUpdateCoSoVatChat vmCoSoVatChat)
        {
            try
            {
                UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();


                var coSoVatChat = await _dasNghiepVuRepo.CoSoVatChat.GetAsync(vmCoSoVatChat.ID);
                var coSoVatChat_DonVi = (from link in _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll()
                                        where link.IDCoSoVatChat == coSoVatChat.ID
                                        select link).ToList().FirstOrDefault();
                
                if (coSoVatChat == null)
                    return new ServiceResultError("Danh mục cơ sở vật chất này hiện không tồn tại hoặc đã bị xóa");
                //   var oldParent = vmCoSoVatChat.ParentPath;


                await ValidateData(vmCoSoVatChat);
                coSoVatChat.Bind(vmCoSoVatChat.KeyValue());
                coSoVatChat_DonVi.IDDonViNghiepVu = vmCoSoVatChat.IDDonViNghiepVu;

                await _dasNghiepVuRepo.CoSoVatChat.UpdateAsync(coSoVatChat);
                await _dasNghiepVuRepo.CoSoVatChat_DonVi.UpdateAsync(coSoVatChat_DonVi);
                await _dasNghiepVuRepo.SaveAync();
                return new ServiceResultSuccess("Cập nhật danh mục cơ sở vật chất thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion


        #region Delete
        public async Task<ServiceResult> Delete(long id)
        {
            try
            {
                var CoSoVatChat_DonVi = (from link in _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll()
                                         where link.IDCoSoVatChat == id
                                         select link).ToList().FirstOrDefault();
                var CoSoVatChat = await _dasNghiepVuRepo.CoSoVatChat.GetAsync(id);
                if (CoSoVatChat == null)
                    return new ServiceResultError("Danh mục cơ sở vật chất này hiện không tồn tại hoặc đã bị xóa");

                await _dasNghiepVuRepo.CoSoVatChat_DonVi.DeleteAsync(CoSoVatChat_DonVi);
                await _dasNghiepVuRepo.SaveAync();

                await _dasNghiepVuRepo.CoSoVatChat.DeleteAsync(CoSoVatChat);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa danh mục cơ sở vật chất thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);

            }
        }
        public async Task<ServiceResult> Delete(IEnumerable<long> ids)
        {
            try
            {
                var CoSoVatChat_DonVi = (from link in _dasNghiepVuRepo.CoSoVatChat_DonVi.GetAll()
                                         select link).ToList();
                var CoSoVatChatDeletes = await _dasNghiepVuRepo.CoSoVatChat.GetAllListAsync(n => ids.Contains(n.ID));
                if (CoSoVatChatDeletes == null || CoSoVatChatDeletes.Count() == 0)
                    return new ServiceResultError("Danh mục cơ sở vật chất đã chọn hiện không tồn tại hoặc đã bị xóa");

                foreach (var item in CoSoVatChat_DonVi)
                {
                    if (ids.Contains(item.IDCoSoVatChat))
                        await _dasNghiepVuRepo.CoSoVatChat_DonVi.DeleteAsync(item);
                }
                await _dasNghiepVuRepo.SaveAync();
                await _dasNghiepVuRepo.CoSoVatChat.DeleteAsync(CoSoVatChatDeletes);
                await _dasNghiepVuRepo.SaveAync();

                return new ServiceResultSuccess("Xóa danh mục cơ sở vật chất thành công");
            }
            catch (LogicException ex)
            {
                return new ServiceResultError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion


        #region Validate
        private async Task ValidateData(VMUpdateCoSoVatChat vmCoSoVatChat)
        {

            //if (await _dasNghiepVuRepo.CoSoVatChat.IsNameExist(vmCoSoVatChat.Name, (int)EnumCommon.Status.InActive, vmCoSoVatChat.ID))
            //{
            //    throw new LogicException("Tên danh mục cơ sở vật chất đã tồn tại");
            //}

        }
        #endregion

        #region Funtions


        #endregion
    }
}