using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.CustomModels;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class UserLogService : BaseMasterService, IUserLogServices
    {
        #region Properties
        private readonly ILogBySqlRepository _logBySql;
        private readonly IMapper _mapper;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IAuthorizeService _authorizeService;
        private readonly IDistributedCache _cache;
        private readonly ICacheManagementServices _cacheManagementServices;
        //private readonly IIPAddressClientServices _iPAddressClient;
        //private readonly IHttpContextAccessor _httpContext;
        #endregion Properties
        #region Ctor
        public UserLogService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IUserPrincipalService userPrincipalService
            , IAuthorizeService authorizeService
            , IDistributedCache cache
            , ICacheManagementServices cacheManagementServices, ILogBySqlRepository logBySql) : base(dasRepository)
        {
            _logBySql = logBySql;
            _mapper = mapper;
            _userPrincipalService = userPrincipalService;
            _authorizeService = authorizeService;
            _cache = cache;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion Ctor

        public async Task<ServiceResult> LogActionLogin(long userID, string userName)
        {
            string publicIP = _userPrincipalService.IpAddress;

            var rs = await _logBySql.InsertUserLog(new LogUserInfo(userID, userName, "Đăng nhập", publicIP));
            if (rs != -1)
            {
                return new ServiceResultSuccess("Success");
            }
            else
            {
                return new ServiceResultSuccess("Can't Log");
            }
        }

        public async Task<ServiceResult> LogActionLogout()
        {
            string publicIP = _userPrincipalService.IpAddress;
            var rs = await _logBySql.InsertUserLog(new LogUserInfo(_userPrincipalService.UserId, _userPrincipalService.UserName, "Đăng xuất", publicIP));
            if (rs != -1)
            {
                return new ServiceResultSuccess("Success");
            }
            else
            {
                return new ServiceResultSuccess("Can't Log");
            }
        }

        public async Task<PaginatedList<VMLogInfo>> GetCRUDLogByCondition(LogInfoCondition condition, bool isExport = false)
        {

            try
            {
                DateTime conditionFromDate = DateTime.MinValue;
                DateTime conditionToDate = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(condition.FromDate))
                    conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(condition.ToDate))
                    conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var temp = from u in _dasRepo.LogSystemCRUD.GetAll()
                           let cdAction = condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())
                           where (condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())) && (condition.Type == 1 ? condition.ActionCRUD.Equals(u.Action.ToString()) : "Lỗi" != u.Action.ToString()) &&
                           ((((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty()))
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && (DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0))))
                           orderby u.CreateDate descending
                           select new VMLogInfo
                           {
                               ID = u.ID,
                               Action = u.Action,
                               Entity = u.Entity,
                               TagName = u.TagName.ToString(),
                               CreatedDate = u.CreateDate.Value,
                               UserId = u.UserID,
                               Username = u.UserName,
                               OldValue = u.OldValue,
                               NewValue = u.NewValue,
                               ChangedValue = u.ChangedValue,
                           };
                var total = await temp.LongCountAsync();
                if (total == 0)
                    return null;

                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                if (isExport)
                    condition.PageSize = 1000000;
                var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();


                return new PaginatedList<VMLogInfo>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        public async Task<VMLogInfoStatistic> GetCRUDLogByConditionErrol(LogInfoCondition condition, bool isExport = false)
        {

            try
            {
                var model = new VMLogInfoStatistic();
                DateTime conditionFromDate = DateTime.MinValue;
                DateTime conditionToDate = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(condition.FromDate))
                    conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(condition.ToDate))
                    conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var temp = from u in _dasRepo.LogSystemCRUD.GetAll()
                           let cdAction = condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())
                           where (condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())) && (condition.Type == 1 ? condition.ActionCRUD.Equals(u.Action.ToString()) : "Lỗi" != u.Action.ToString()) &&
                           ((((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty()))
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && (DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0))))
                           orderby u.CreateDate descending
                           select new VMLogInfo
                           {
                               ID = u.ID,
                               Action = u.Action,
                               Entity = u.Entity,
                               TagName = u.TagName.ToString(),
                               CreatedDate = u.CreateDate.Value,
                               UserId = u.UserID,
                               Username = u.UserName,
                               OldValue = u.OldValue,
                               NewValue = u.NewValue,
                               ChangedValue = u.ChangedValue,
                           };
                var datas = await temp.ToListAsync();
                var result = datas
                .GroupBy(l => l.Username)
                    .Select(csLine => new VMLogInfo
                    {
                        Username = csLine.Key,
                        Total = csLine.Count(),
                    }).ToList<VMLogInfo>() ?? new List<VMLogInfo>();
                var chartData = new
                {
                    labels = result.Select(n => n.Username).ToArray(),
                    data = result.Select(n => n.Total).ToArray(),
                };
                var total = await temp.LongCountAsync();
                if (total == 0)
                    return null;

                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                if (isExport)
                    condition.PageSize = 1000000;
                var paggings = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

                model.ChartData = chartData;
                model.Tables= new PaginatedList<VMLogInfo>(paggings.ToList(), (int)total, condition.PageIndex, condition.PageSize);
                return model;
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        public async Task<VMLogInfo> GetChartCRUDLogByCondition(LogInfoCondition condition)
        {

            try
            {
                DateTime conditionFromDate = DateTime.MinValue;
                DateTime conditionToDate = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(condition.FromDate))
                    conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(condition.ToDate))
                    conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var temp = from u in _dasRepo.LogSystemCRUD.GetAll()
                           where
                           ((((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty()))
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && (DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0))))
                           orderby u.CreateDate descending
                           select new VMLogInfo
                           {
                               Total = 1,
                               Action = u.Action,
                           };
                var datas = await temp.ToListAsync();
                var result = datas
                .GroupBy(l => l.Action)
                //.SelectMany(cl => cl.Select(
                    //csLine => new VMLogInfo
                    //{
                    //    Action = cl.Key,
                    //    Total = cl.Count(),
                    //})).ToList<VMLogInfo>();
                //.SelectMany(cl => cl.Select(
                    .Select(csLine => new VMLogInfo
                    {
                        Action = csLine.Key,
                        Total = csLine.Count(),
                    }).ToList<VMLogInfo>() ?? new List<VMLogInfo>();
                var chartData = new
                {
                    labels = result.Select(n => n.Action).ToArray(),
                    data = result.Select(n => n.Total).ToArray(),
                };

                return new VMLogInfo { ChartData = chartData , vMLogInfos = result };
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        public async Task<PaginatedList<VMUserLogInfo>> GetUserLogByCondition(LogInfoCondition condition, bool isExport = false)
        {

            try
            {

                DateTime conditionFromDate = DateTime.MinValue;
                DateTime conditionToDate = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(condition.FromDate))
                    conditionFromDate = DateTime.ParseExact(condition.FromDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(condition.ToDate))
                    conditionToDate = DateTime.ParseExact(condition.ToDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                var temp = from u in _dasRepo.LogUserAction.GetAll()
                           let cdAction = condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())
                           where (condition.ActionCRUD.IsEmpty() || condition.ActionCRUD.Equals(u.Action.ToString())) &&
                           ((((condition.FromDate.IsEmpty() && condition.ToDate.IsEmpty()))
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && condition.ToDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0) && condition.FromDate.IsEmpty())
                       || ((DateTime.Compare(u.CreateDate.Value, conditionFromDate) >= 0) && (DateTime.Compare(u.CreateDate.Value, conditionToDate) <= 0))))
                           orderby u.CreateDate descending
                           select new VMUserLogInfo
                           {
                               ID = u.ID,
                               Action = u.Action,
                               CreatedDate = u.CreateDate.Value,
                               UserId = u.UserID,
                               Username = u.UserName,
                               IPAddress = u.IPAddress,

                           };
                var total = await temp.LongCountAsync();
                if (total == 0)
                    return null;

                int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                if (isExport)
                    condition.PageSize = 1000000;
                var result = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
                return new PaginatedList<VMUserLogInfo>(result.ToList(), (int)total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception ex)
            {

            }
            return null;

        }
        #region PrivateFunc
        //private string GetIpAddressOfClient()
        //{
        //    string ipAddress = string.Empty;
        //    IPAddress ip = _httpContext.HttpContext.Connection.RemoteIpAddress;
        //    if (ip != null)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        //        {
        //            ip = Dns.GetHostEntry(ip).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        //        }
        //        ipAddress = ip.ToString();
        //    }
        //    return ipAddress;
        //}

        private async Task<List<VMLogInfo>> ReadData(SqlDataReader dataReader)
        {
            var rs = new List<VMLogInfo>();
            if (dataReader.HasRows)
            {
                while (await dataReader.ReadAsync())
                {
                    rs.Add(new VMLogInfo
                    {

                        ID = dataReader["ID"].ToString(),
                        TagName = dataReader["TagName"].ToString(),
                        Entity = dataReader["Entity"].ToString(),
                        Action = dataReader["Action"].ToString(),
                        CreatedDate = (DateTime)dataReader["CreateDate"],
                        UserId = (int)dataReader["UserId"],
                        Username = dataReader["Username"].ToString(),
                        OldValue = dataReader["OldValue"].ToString(),
                        NewValue = dataReader["NewValue"].ToString(),
                        ChangedValue = dataReader["ChangedValue"].ToString(),
                    });
                }
                await dataReader.CloseAsync();
            }
            if (!dataReader.IsClosed)
            {
                await dataReader.CloseAsync();
            }
            return rs;
        }

        private async Task<List<VMUserLogInfo>> ReadDataUserLog(SqlDataReader dataReader)
        {
            var rs = new List<VMUserLogInfo>();
            if (dataReader.HasRows)
            {
                while (await dataReader.ReadAsync())
                {
                    rs.Add(new VMUserLogInfo
                    {

                        ID = dataReader["ID"].ToString(),
                        Action = dataReader["Action"].ToString(),
                        CreatedDate = (DateTime)dataReader["CreateDate"],
                        UserId = (int)dataReader["UserId"],
                        Username = dataReader["Username"].ToString(),
                        IPAddress = dataReader["IPAddress"].ToString(),

                    });
                }
                await dataReader.CloseAsync();
            }
            if (!dataReader.IsClosed)
            {
                await dataReader.CloseAsync();
            }
            return rs;
        }
        #endregion PrivateFunc
    }
}
