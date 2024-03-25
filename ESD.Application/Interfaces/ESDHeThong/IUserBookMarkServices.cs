using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using Microsoft.AspNetCore.Http;
namespace ESD.Application.Interfaces
{
    public interface IUserBookMarkServices
    {
        Task<VMUserBookMark> GetBookMark();
        Task<ServiceResult> ChangeBookMark( List<int> modules);
        Task<ServiceResult> AddBookMark(int idModule);
        Task<ServiceResult> RemoveBookMark(int idModule);
    }
}
