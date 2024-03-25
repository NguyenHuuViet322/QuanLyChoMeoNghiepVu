using DASNotify.Application.Models.CustomModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DASNotify.Application.Interfaces
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