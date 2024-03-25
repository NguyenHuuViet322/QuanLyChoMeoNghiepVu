using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using ESD.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using Newtonsoft.Json;
using ESD.Utility;

namespace ESD.Application.Services
{
    public class ResetPasswordService : BaseMasterService, IResetPasswordService
    {
        private readonly IUserService _userService;
        private readonly IDasRepositoryWrapper _dasRepo;
        public ResetPasswordService(IDasRepositoryWrapper dasRepository, IUserService userService) : base(dasRepository)
        {
            _dasRepo = dasRepository;
            _userService = userService;
        }
        public async Task<ServiceResult> Create(ResetPassword model)
        {
            await _dasRepo.ResetPassword.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add Token suceess!");
        }

        public async Task<ServiceResult> Delete(object id)
        {
            var rq = await _dasRepo.ResetPassword.GetAsync(id);
            await _dasRepo.ResetPassword.DeleteAsync(rq);
            await _dasRepo.SaveAync();
            if (rq == null)
                return new ServiceResultError("Request not exxist");

            return new ServiceResultSuccess();
        }

        public Task<ResetPassword> Get(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ResetPassword>> Gets()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> Update(ResetPassword model)
        {
            await _dasRepo.ResetPassword.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Update Token suceess!");
        }

        public async Task<ServiceResult> ResetPasswordRequest(int userID, string token)
        {
            try
            {
                var rq = await _dasRepo.ResetPassword.GetAllListAsync(u => u.UserID == userID);
                if (!IsExisted(rq))
                {
                    await CreateResetPasswordToken(userID, token);
                }
                else
                {
                    await Delete(rq.FirstOrDefault().ID);
                    await UpdateResetPasswordToken(userID, token);
                }

                return new ServiceResultSuccess("Yêu cầu lấy lại mật khẩu thành công. Vui lòng kiểm tra email để làm theo hướng dẫn.");
            }
            catch (Exception ex)
            {
                return new ServiceResultError("Lỗi không xác định. Vui lòng thử lại");
            }
        }
        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        public async Task<ServiceResult> CreateResetPasswordToken(int userID, string token)
        {
            ResetPassword model = new ResetPassword();
            model.UserID = userID;
            model.Token = token;
            model.ExpiredTime = DateTime.UtcNow.AddMinutes(5);
            model.IsUsed = false;
            try
            {
                return await Create(model);
            }
            catch (Exception)
            {
                return new ServiceResultError("Lỗi không xác định. Vui lòng thử lại");
            }

        }

        public async Task<ServiceResult> UpdateResetPasswordToken(int userID, string token)
        {
            ResetPassword model = new ResetPassword();
            model.UserID = userID;
            model.Token = token;
            model.ExpiredTime = DateTime.UtcNow.AddMinutes(5);
            model.IsUsed = false;
            try
            {
                return await Update(model);
            }
            catch (Exception)
            {
                return new ServiceResultError("Lỗi không xác định. Vui lòng thử lại");
            }
        }

        public async Task<ServiceResult> ResetPasswordAction(VMResetPassword model, string token)
        {
            var request = await _dasRepo.ResetPassword.GetAllListAsync(rq => rq.Token == token);
            if (!IsExisted(request))
            {
                return new ServiceResultError("Yêu cầu lấy lại mật khẩu thất bại");
            }

            if (request.FirstOrDefault().IsUsed)
            {
                return new ServiceResultError("Yêu cầu lấy lại mật khẩu không đúng");
            }

            if (DateTime.Compare(DateTime.UtcNow, request.FirstOrDefault().ExpiredTime.Value) > 0)
            {
                return new ServiceResultError("Thời gian lấy lại mật khẩu đã hết. Vui lòng yêu cầu lại.");
            }

            var user = await _dasRepo.User.GetAllListAsync(u => u.ID == request.FirstOrDefault().UserID);
            user.FirstOrDefault().Password = StringUltils.Md5Encryption(model.Password);
            await _dasRepo.User.UpdateAsync(user);
            request.FirstOrDefault().IsUsed = true;
            await _dasRepo.ResetPassword.UpdateAsync(request.FirstOrDefault());
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật mật khẩu thành công");
        }
    }
}
