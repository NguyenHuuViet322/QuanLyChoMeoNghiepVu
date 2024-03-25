using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ESD.Application.Interfaces
{
    public interface IResetPasswordService : IBaseMasterService<ResetPassword>
    {
        Task<ServiceResult> ResetPasswordRequest(int userID, string token);
        Task<ServiceResult> CreateResetPasswordToken(int userID, string token);
        Task<ServiceResult> ResetPasswordAction(VMResetPassword model, string token);
    }
}
