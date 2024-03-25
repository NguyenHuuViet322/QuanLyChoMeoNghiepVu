using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Application.Models.MobileApiModel;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ESD.Application.Enums;
using ESD.Utility;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace DAS.MobileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class MobileApiController : BaseController
    {
        #region Properties

        private readonly IDataApiServices _dataApiServices;
        private readonly IConfiguration _configuration;

        #endregion

        #region Ctor

        public MobileApiController(IDataApiServices dataApiServices, IConfiguration configuration)
        {
            _dataApiServices = dataApiServices;
            _configuration = configuration;
        }

        #endregion

        #region Get Token

        [HttpGet]
        [AllowAnonymous]
        [Route("get-token")]
        public async Task<IActionResult> GetToken(string userName, string pwd)
        {
            //Test commit
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
            {
                return JSErrorResult("Tài khoản hoặc mật khẩu không được để trống");
            }
            if (pwd.Length > 255 || pwd.Length < 6)
            {
                return JSErrorResult("Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
            }
            var model = new VMMobileReaderLogin
            {
                UserName = userName,
                Password = pwd
            };
            var serviceResult = await _dataApiServices.Authenticate(model);
            if (serviceResult.Code != CommonConst.Success || serviceResult.Data == null) return JSErrorResult("Tai khoan/mat khau khong dung!");
            var user = (Reader)serviceResult.Data;
            // create claims token
            var claimsToken = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(CustomClaimTypes.Username, user.AccountName),
                new Claim(CustomClaimTypes.FullName, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                _configuration["Tokens:Issuer"],
                claimsToken,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);
            return new JsonResult(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        #endregion

        #region Account API

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] VMMobileReaderLogin model)
        {
            if (!ModelState.IsValid)
            {
                return JSErrorModelStateByLine();
            }
            if (model.Password.Length > 255 || model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                return JSErrorModelStateByLine();
            }

            if (model.UserName.Length > 50)
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập không được vượt quá 255 ký tự");
                return JSErrorModelStateByLine();
            }
            var serviceResult = await _dataApiServices.Authenticate(model);
            if (serviceResult.Code == CommonConst.Success && serviceResult.Data != null)
            {
                #region Bearer token
                var reader = (Reader)serviceResult.Data;
                // create claims token
                var claimsToken = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, reader.ID.ToString()),
                    new Claim(CustomClaimTypes.Username, reader.AccountName),
                    new Claim(CustomClaimTypes.FullName, reader.Name),
                    new Claim(ClaimTypes.Email, reader.Email)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenSecurity = new JwtSecurityToken(
                    _configuration["Tokens:Issuer"],
                    _configuration["Tokens:Audience"],
                    claimsToken,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials);
                #endregion
                // create claims
                var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);
                return CustJSonResult(new ServiceResultSuccess("Đăng nhập thành công", new
                {
                    Id = reader.ID,
                    UserName = reader.AccountName,
                    Name = reader.Name,
                    Token = token
                }));
            }
            return CustJSonResult(new ServiceResultError("Đăng nhập không thành công"));
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] VMMobileReaderRegister model)
        {
            if (!ModelState.IsValid)
            {
                return JSErrorModelStateByLine();
            }

            if (model.Password.IsNotEmpty())
            {
                if (model.Password.Length < 6 || model.Password.Length > 50)
                {
                    ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                    return JSErrorModelStateByLine();
                }
            }
            if (model.ConfirmPassword.IsNotEmpty())
            {
                if (model.ConfirmPassword.Length < 6 || model.ConfirmPassword.Length > 50)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                    return JSErrorModelStateByLine();
                }
            }

            if (model.Password.IsNotEmpty() && model.Password.IsNotEmpty())
            {
                if (!Equals(model.Password, model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Xác nhận mật khẩu không đúng !");
                    return JSErrorModelStateByLine();
                }
            }

            if (model.IdentityNumber != null)
            {
                if (model.IdentityNumber.Length != 9 && model.IdentityNumber.Length != 12)
                {
                    ModelState.AddModelError("IdentityNumber", "CMND/CCCD không đúng định dạng (9 hoặc 12 số)");
                    return JSErrorModelStateByLine();
                }
            }
            var rs = await _dataApiServices.Register(model);
            return CustJSonResult(rs);
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] VMMobileChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return JSErrorModelStateByLine();
            }
            if (model.OldPassword.IsNotEmpty())
            {
                if (model.OldPassword.Length < 6 || model.OldPassword.Length > 50)
                {
                    ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                    return JSErrorModelStateByLine();
                }
            }
            if (model.Password.IsNotEmpty())
            {
                if (model.Password.Length < 6 || model.Password.Length > 50)
                {
                    ModelState.AddModelError("Password", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                    return JSErrorModelStateByLine();
                }
            }
            if (model.ConfirmPassword.IsNotEmpty())
            {
                if (model.ConfirmPassword.Length < 6 || model.ConfirmPassword.Length > 50)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu có tối thiểu 6 ký tự và tối đa 255 ký tự");
                    return JSErrorModelStateByLine();
                }
            }

            if (model.Password.IsNotEmpty() && model.Password.IsNotEmpty())
            {
                if (!Equals(model.Password, model.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Nhập lại mật khẩu không chính xác !");
                    return JSErrorModelStateByLine();
                }
            }
            var rs = await _dataApiServices.ChangePassword(model);
            return CustJSonResult(rs);
        }

        
        #endregion

       
        #region #31474 API danh sách trạng thái phiếu mượn

        [HttpGet]
        [AllowAnonymous]
        [Route("gettest")]
        public IActionResult gettest()
        {
            try
            {
                var model = "";// _dataApiServices.GetListBorrowStatus();
                return CustJSonResult(new ServiceResultSuccess { Data = model });
            }
            catch (Exception ex)
            {
                return CustJSonResult(new ServiceResultError(ex.Message));
            }
        }
        #endregion
    }
}


