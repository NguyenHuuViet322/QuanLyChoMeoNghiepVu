using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.ViewModels;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
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

    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly IPermissionService _permissionService;
        private readonly IUserLogServices _userLogServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IConfiguration configuration, IAccountService accountService
            , IUserService userService, IUserPrincipalService userPrincipalService, IHttpContextAccessor httpContextAccessor
            , IResetPasswordService resetPasswordService, IPermissionService permissionService
            , IUserLogServices userLogServices)
        {
            _configuration = configuration;
            _accountService = accountService;
            _userService = userService;
            _userPrincipalService = userPrincipalService;
            _resetPasswordService = resetPasswordService;
            _permissionService = permissionService;
            _userLogServices = userLogServices;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Account
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin model)
        {
            SetCookieMobile(model.IsMobile.ToString());
            if (!ModelState.IsValid)
                return View();

            if (model.Password.Length > 255 || model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                return View();
            }

            var serviceResult = await _accountService.Authenticate(model);
            if (serviceResult.Code == CommonConst.Success && serviceResult.Data != null)
            {
                #region Bearer token

                var user = (ESD.Domain.Models.DAS.User)serviceResult.Data;
                // create claims token
                var claimsToken = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(CustomClaimTypes.Username, user.AccountName)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                    _configuration["Tokens:Issuer"],
                    claimsToken,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                #endregion

                // create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                    new Claim(CustomClaimTypes.Username, user.AccountName),
                    new Claim(CustomClaimTypes.AccessToken, new JwtSecurityTokenHandler().WriteToken(token)),
                    new Claim(CustomClaimTypes.FullName, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(CustomClaimTypes.IDOrgan, user.IDOrgan.ToString()),
                    new Claim(CustomClaimTypes.IsSupperUser, user.HasOrganPermission.ToString()),
                    new Claim(CustomClaimTypes.IDAgency, user.IDAgency.ToString())
                };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                //Push permission to Cache : chienvx
                //Get cache ()
                await _permissionService.AddCachePermission(user.ID);

                // sign-in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    });
                if (ConfigUtils.GetKeyValue("LogConfig", "LogUserActivity") == "true")
                {
                    await _userLogServices.LogActionLogin(user.ID, user.AccountName);
                }


                return Redirect("/DongVatNghiepVu");
            }

            ModelState.AddModelError("", serviceResult.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (ConfigUtils.GetKeyValue("LogConfig", "LogUserActivity") == "true" && _userPrincipalService.UserId != -1)
            {
                await _userLogServices.LogActionLogout();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(VMAccount model)
        {
            if (string.IsNullOrWhiteSpace(model.AccountName))
            {
                ModelState.AddModelError("AccountName", "Email không được để trống");
                return JSErrorModelStateByLine();
            }

            if (!StringUltils.IsValidEmail(model.AccountName))
            {
                ModelState.AddModelError("AccountName", "Email không đúng định dạng");
                return JSErrorModelStateByLine();
            }

            var userID = await _userService.GetUserIDByEmail(model.AccountName);
            if (userID == 0)
            {
                ModelState.AddModelError("AccountName", "Email không tồn tại trong hệ thống");
                return JSErrorModelStateByLine();
            }
            else
            {
                byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                string token = StringUltils.Md5Encryption(Convert.ToBase64String(time));
                string title = "Hướng dẫn reset mật khẩu tài khoản: " + model.AccountName;
                string domainUrl = ConfigUtils.GetKeyValue("Url", "DomainUrl");
                if (string.IsNullOrEmpty(domainUrl))
                {
                    domainUrl = HttpContext.Request.Host.Host;
                }
                string url = string.Concat(HttpContext.Request.Scheme, "://", domainUrl, "/Account/ResetPassword?token=", token);
                var body = await Utils.RenderViewAsync(this, "EmailResetPassword", model, true);
                body = body.Replace("{Email}", model.AccountName);
                body = body.Replace("{urlLink}", url);
                var sendResult = await _accountService.SendEmailWithEmailAddress(body, title, model.AccountName, "ForgotPassword");
                if (!sendResult.Code.Equals(CommonConst.Success))
                {
                    return CustJSonResult(sendResult);
                }
                else
                {
                    var request = await _resetPasswordService.ResetPasswordRequest(userID, token);
                    return CustJSonResult(request);
                }
            }
        }
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            ViewData["Token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAction(VMResetPassword model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            var result = await _resetPasswordService.ResetPasswordAction(model, model.Token);
            return CustJSonResult(result);
        }
        #endregion Account

        #region User
        [HttpGet]
        public IActionResult ChangePassword(VMAccount model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordAction(VMAccount model)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();

            model.userID = _userPrincipalService.UserId;
            model.Email = _userPrincipalService.Email;
            model.AccountName = _userPrincipalService.UserName;
            model.UserName = _userPrincipalService.FullName;
            model.Url = Path.Combine(_httpContextAccessor.HttpContext.Request.Host.Value, "Account", "ForgotPassword").Replace("\\", "/"); // URL quên mật khẩu trong mail
            var result = await _accountService.ChangePassword(model);

            if (result.Code.Equals(CommonConst.Success))
            {
                // logout
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return CustJSonResult(result);
            }

            ModelState.AddModelError("OldPassword", result.Message);
            return JSErrorModelStateByLine();
        }

        public IActionResult CreateChangePasswordPopup()
        {
            return PartialView("ChangePassword", new VMAccount());
        }

        public async Task<IActionResult> ViewProfile()
        {
            if (_userPrincipalService == null || _userPrincipalService.UserId == 0)
                return NotFound();
            var user = await _accountService.GetCurrentUser(_userPrincipalService.UserId);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(VMUserProfile vmUser)
        {
            if (!ModelState.IsValid)
                return JSErrorModelStateByLine();
            if (_userPrincipalService == null || _userPrincipalService.UserId == 0)
                return NotFound();
            vmUser.ID = _userPrincipalService.UserId;
            var rs = await _accountService.UpdateUserProfile(vmUser);
            return CustJSonResult(rs);
        }
        #endregion User

        public IActionResult TestUpload => View();

        [HttpPost]
        public async Task<IActionResult> TestUploadAction(string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash)
        {
            if (Request.Form.Files.Any())
            {
                var files = new List<IFormFile>();
                foreach (var formFile in Request.Form.Files)
                {
                    files.Add(formFile);
                }
                var result = await _accountService.UploadLargeFile(files, resumableIdentifier, resumableChunkNumber, resumableChunkSize, resumableTotalSize, resumableFilename, resumableChunkHash);
                return CustJSonResult(result);
            }

            return NotFound();
        }
        public void SetCookieMobile(string isMobile)
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("IsMobile", string.IsNullOrEmpty(isMobile) ? string.Empty : isMobile.ToLower(), cookieOptions);
        }
    }

}
