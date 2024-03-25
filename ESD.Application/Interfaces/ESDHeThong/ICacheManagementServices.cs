using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface ICacheManagementServices
    {
        Task<UserData> GetUserDataAndSetCache();
        Task<UserData> GetCurrentUserData();
    }
}
