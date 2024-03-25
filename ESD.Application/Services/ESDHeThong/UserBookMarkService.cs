using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class UserBookMarkService : IUserBookMarkServices
    {
        #region Properties
        private readonly IDasRepositoryWrapper _dasRepo;
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        #endregion Properties

        #region Ctor
        public UserBookMarkService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService userPrincipalService)
        {
            _dasRepo = dasRepository;
            _mapper = mapper;
            _userPrincipalService = userPrincipalService;
        }
        #endregion Ctor
        public async Task<ServiceResult> AddBookMark(int idModule)
        {
            try
            {
                var bookmark = await _dasRepo.UserBookMark.FirstOrDefaultAsync(x => x.IDUser == _userPrincipalService.UserId);
                if (bookmark == null)
                {
                    UserBookmark entry = new UserBookmark
                    {
                        IDUser = _userPrincipalService.UserId,
                        BookMark = JsonConvert.SerializeObject(new List<int> { idModule })
                    };
                    await _dasRepo.UserBookMark.InsertAsync(entry);
                    await _dasRepo.SaveAync();
                    if (entry.ID == 0)
                    {
                        return new ServiceResultError("Thêm lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
                    }
                    else
                    {
                        return new ServiceResultSuccess("Thêm lối tắt trang chủ thành công", (int)EnumCommon.Status.Active);
                    }
                }
                else
                {
                    var exsitList = JsonConvert.DeserializeObject<List<int>>(bookmark.BookMark);
                    if (exsitList.Contains(idModule))
                    {
                        return new ServiceResultError("Lối tắt trang chủ đã tồn tại", (int)EnumCommon.Status.Active);
                    }
                    else
                    {
                        exsitList.Add(idModule);
                        bookmark.BookMark = JsonConvert.SerializeObject(exsitList);
                        await _dasRepo.UserBookMark.UpdateAsync(bookmark);
                        await _dasRepo.SaveAync();
                        return new ServiceResultSuccess("Thêm lối tắt trang chủ thành công", (int)EnumCommon.Status.Active);
                    }
                }
            }
            catch (Exception ex)
            {

                return new ServiceResultError("Thêm lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
            }


        }

        public async Task<ServiceResult> ChangeBookMark(List<int> modules)
        {
            try
            {
                var bookmark = await _dasRepo.UserBookMark.FirstOrDefaultAsync(x => x.IDUser == _userPrincipalService.UserId);
                if (bookmark == null)
                {
                    UserBookmark entry = new UserBookmark
                    {
                        IDUser = _userPrincipalService.UserId,
                        BookMark = JsonConvert.SerializeObject(modules)
                    };
                    await _dasRepo.UserBookMark.InsertAsync(entry);
                    await _dasRepo.SaveAync();
                    if (entry.ID == 0)
                    {
                        return new ServiceResultError("Cập nhật lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
                    }
                    else
                    {
                        return new ServiceResultSuccess("Cập nhật lối tắt trang chủ thành công", (int)EnumCommon.Status.Active);
                    }
                }
                else
                {
                    bookmark.BookMark = JsonConvert.SerializeObject(modules);
                    await _dasRepo.UserBookMark.UpdateAsync(bookmark);
                    await _dasRepo.SaveAync();
                    return new ServiceResultSuccess("Cập nhật lối tắt trang chủ thành công", (int)EnumCommon.Status.Active);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Cập nhật lối tắt trang chủ không thành công", (int)EnumCommon.Status.Active);
            }

        }

        public async Task<VMUserBookMark> GetBookMark()
        {
            return _mapper.Map<VMUserBookMark>(await _dasRepo.UserBookMark.FirstOrDefaultAsync(x => x.IDUser == _userPrincipalService.UserId));
        }

        public async Task<ServiceResult> RemoveBookMark(int idModule)
        {
            try
            {
                var bookmark = await _dasRepo.UserBookMark.FirstOrDefaultAsync(x => x.IDUser == _userPrincipalService.UserId);
                if (bookmark == null)
                {
                    return new ServiceResultError("Xóa lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
                }
                else
                {
                    var exsitList = JsonConvert.DeserializeObject<List<int>>(bookmark.BookMark);
                    if (exsitList.Contains(idModule))
                    {
                        exsitList.Remove(idModule);
                        bookmark.BookMark = JsonConvert.SerializeObject(exsitList);
                        await _dasRepo.UserBookMark.UpdateAsync(bookmark);
                        await _dasRepo.SaveAync();
                        return new ServiceResultSuccess("Xóa lối tắt trang chủ thành công", (int)EnumCommon.Status.InActive);
                    }
                    else
                    {
                        return new ServiceResultError("Xóa lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
                    }
                }
            }
            catch (Exception ex)
            {

                return new ServiceResultError("Xóa lối tắt trang chủ không thành công", (int)EnumCommon.Status.InActive);
            }
        }
    }
}
