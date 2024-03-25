using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult> Authenticate(VMLogin model);
        Task<bool> Register(VMRegister model);
        Task<ServiceResult> ChangePassword(VMAccount model);
        Task<int> GetUserIDByEmail(string Email);
        Task<ServiceResult> SendEmailWithEmailAddress(string body, string title, string email, string emailType);
        Task<VMUserProfile> GetCurrentUser(int id);

        Task<VMUserProfile> GetCurrentReader(int id);

        Task<ServiceResult> UpdateUserProfile(VMUserProfile vmUser);
        Task<ServiceResult> UploadLargeFile(List<IFormFile> files, string resumableIdentifier, int resumableChunkNumber, long resumableChunkSize, long resumableTotalSize, string resumableFilename, string resumableChunkHash);
        Task<List<User>> GetUserByModule(int codemodule,int idper);
    }
}
