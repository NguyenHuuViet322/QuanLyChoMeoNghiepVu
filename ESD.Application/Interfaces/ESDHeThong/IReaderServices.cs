using System.Collections.Generic;
using ESD.Application.Models.ViewModels;
using System.Threading.Tasks;
using ESD.Domain.Models.DAS;
using ESD.Application.Models.CustomModels;
using ESD.Domain.Interfaces.DAS;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Interfaces
{
    public interface IReaderServices 
    {
        Task<ServiceResult> Authenticate(VMReaderLogin model);
        Task<ServiceResult> Register(VMReaderRegister model, int idOrgan = 0);
        Task<PaginatedList<VMReader>> SearchByConditionPagging(ReaderCondition condition, bool isExport = false);
        Task<VMReader> GetReader(int id);
        Task<ServiceResult> UpdateReader(VMReader model);
        Task<ServiceResult> UpdateProfileReader(VMReader model);
        Task<ServiceResult> DeleteReader(int id);
        Task<ServiceResult> DeleteReaders(IEnumerable<int> ids);
        Task<PaginatedList<VMReader>> SearchByConditionPaggingOrgan(ReaderCondition condition, bool isExport = false);
        Task<VMReader> GetReaderOrgan(int id);
        Task<ServiceResult> UpdateReaderOrgan(VMReader model);
        Task<ServiceResult> RegisterByOrgan(VMReaderRegister model);
        Task<bool> IsReaderInOrgan(int idOrgan, int idReader);
        Task<bool> AddReaderInOrgan(int idOrgan, int idReader);
        Task<ServiceResult> ChangePassword(VMAccount model);
       

    }
}
