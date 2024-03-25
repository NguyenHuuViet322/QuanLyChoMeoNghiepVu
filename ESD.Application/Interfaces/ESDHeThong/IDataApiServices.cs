using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.MobileApiModel;
using ESD.Application.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using ESD.Application.Enums;
using System.Collections;

namespace ESD.Application.Interfaces
{
    public interface IDataApiServices
    {
        Task<ServiceResult> Register(VMMobileReaderRegister model);
        Task<ServiceResult> Authenticate(VMMobileReaderLogin loginModel);
        Task<ServiceResult> ChangePassword(VMMobileChangePassword model);
    }
}
