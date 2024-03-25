using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IAuthorizeService
    {
        Task<bool> CheckPermission(int CodeModule, int[] Permissions);
    }
}
