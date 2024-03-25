using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;

namespace DASNotify.Infrastructure.ContextAccessors
{
    public class UserPrincipalService : IUserPrincipalService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPrincipalService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        public int UserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return -1;
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (context == null)
                {
                    return -1;
                }

                int result;
                int.TryParse(context.Value, out result);
                return result;
            }
        }
        public string UserName
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return "";
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.Username);

                if (context == null)
                {
                    return SpecialSystemVariables.GuestName;
                }

                return context.Value;
            }
        }

        public string AccessToken
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return "";
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.AccessToken);

                if (context == null)
                {
                    return null;
                }

                return context.Value;
            }
        }

        public string Email
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return "";
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);

                if (context == null)
                {
                    return string.Empty;
                }

                return context.Value;
            }
        }
        public string FullName
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return "";
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.FullName);

                if (context == null)
                {
                    return SpecialSystemVariables.GuestName;
                }

                return context.Value;
            }
        }
        public bool IsSuperUser
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return false;
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.IsSupperUser);

                if (context == null)
                {
                    return false;
                }

                bool.TryParse(context.Value, out var result);
                return result;
            }

        }
        public string IpAddress
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return "";
                }

                IPAddress ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                if (ip != null)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        ip = Dns.GetHostEntry(ip).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }
                    return ip.ToString();
                }
                return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }
        }
        public int IDOrgan
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return 0;
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.IDOrgan);

                if (context == null)
                {
                    return 0;
                }

                return int.Parse(context.Value);
            }
        }

        public int IDAgency
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return 0;
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.IDAgency);

                if (context == null)
                {
                    return 0;
                }

                return int.Parse(context.Value);
            }
        }

        public bool IsLoginBySso
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return false;
                }

                var context = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.IsLoginBySso);

                if (context == null)
                {
                    return false;
                }

                return bool.Parse(context.Value);
            }
        }

        public bool AddUpdateClaim(string key, string value)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return false;
            }

            var identity = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;

            if (identity == null)
            {
                return false;
            }

            var existingClaims = identity.FindAll(key).ToList();
            if (existingClaims.Any())
            {
                foreach (var existingClaim in existingClaims)
                {
                    identity.RemoveClaim(existingClaim);
                }
                // add new claim
                identity.AddClaim(new Claim(key, value));
            }
            else
            {
                // add new claim
                identity.AddClaim(new Claim(key, value));
            }

            return true;
        }
    }

    public class CustomClaimTypes
    {
        public const string Username = "userName";
        public const string FullName = "fullName";
        public const string AccessToken = "access_token";
        public const string Roles = "roles";
        public const string Permissions = "permissions";
        public const string IsSupperUser = "issuperuser";
        public const string IDOrgan = "idOrgan";
        public const string IDAgency = "idAgency";
        public const string IsLoginBySso = "isLoginBySso";
    }

    public class SpecialSystemVariables
    {
        public const string GuestName = "Guest";
    }
}
