using ESD.Application.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IBaseMasterService<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> Gets();

        Task<TEntity> Get(object id);

        Task<ServiceResult> Create(TEntity model);

        Task<ServiceResult> Update(TEntity model);

        Task<ServiceResult> Delete(object id);
    }
}