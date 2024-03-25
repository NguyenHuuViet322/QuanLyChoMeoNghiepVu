using DASNotify.Application.Models.CustomModels;
using DocumentFormat.OpenXml.EMMA;
using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CustomClass;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAS.Web.Controllers
{

    public class UploaderController : BaseController
    {
        private readonly IUploadServices _uploadService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UploaderController(IConfiguration configuration, IUploadServices uploadService
            , IUserPrincipalService userPrincipalService, IHttpContextAccessor httpContextAccessor
            , ILoggerManager logger)
        {
            _configuration = configuration;
            _uploadService = uploadService;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            try
            {
                var rs = _uploadService.Upload(file);
                return JSSuccessResult("Upload thành công", rs);
            }
            catch (LogicException ex)
            {
                return JSErrorResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.Serialize(ex));
                return JSErrorResult("Upload không thành công");
            }
        }
    }

}
