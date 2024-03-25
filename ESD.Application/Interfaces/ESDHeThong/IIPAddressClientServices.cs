using ESD.Application.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IIPAddressClientServices
    {
        Task<ServiceResult> GetPublicIPAddress();
    }
}
