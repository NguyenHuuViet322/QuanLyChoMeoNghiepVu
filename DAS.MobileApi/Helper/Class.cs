using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ESD.Application.Constants;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.ESDNghiepVu;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace WebApi.Helpers
{
   
    public static class ExtensionMethods
    {
        //public static IEnumerable<SharedApp> WithoutPasswords(this IEnumerable<SharedApp> users)
        //{
        //    return users.Select(x => x.WithoutPassword());
        //}

        //public static User WithoutPassword(this User user)
        //{
        //    user.Password = null;
        //    return user;
        //}
    }
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {


        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            ServiceResult serviceResult =  null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                //serviceResult = await _sharedAppService.Authenticate(new ESD.Application.Models.MobileApiModel.VMMobileReaderLogin { UserName=username,Password=password});
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (serviceResult.Code != CommonConst.Success || serviceResult.Data == null)
                return AuthenticateResult.Fail("Invalid Username or Password");
            //var shareApp = (SharedApp)serviceResult.Data; ;
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, ""),
                new Claim(ClaimTypes.Name, "shareApp.Username"),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}