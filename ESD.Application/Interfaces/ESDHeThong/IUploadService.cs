using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ESD.Application.Interfaces
{
    public interface IUploadServices
    {
        VMFile Upload(IFormFile file);
    }
}
