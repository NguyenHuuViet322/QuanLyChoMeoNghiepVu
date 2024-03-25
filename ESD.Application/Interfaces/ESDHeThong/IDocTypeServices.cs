using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESD.Application.Interfaces
{
    public interface IDocTypeServices
    {
        Task<IEnumerable<DocType>> GetsActive();
        Task<IEnumerable<DataType>> GetDataTypes();
        Task<PaginatedList<VMDocType>> SearchByConditionPagging(DocTypeCondition DocTypeCondition);
        Task<VMDocType> Create();
        Task<ServiceResult> Create(VMUpdateDocType vmDocType);
        Task<VMDocType> Update(int? id);
        Task<ServiceResult> Update(VMUpdateDocType vmDocType);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Delete(IEnumerable<int> ids);
        Task<DocType> Get(object id);
        Task<IEnumerable<DocType>> Gets();
        Task<VMDocType> ChangeType(int id, int type);
        Task<VMDocTypeField> GetDocTypeField(object id);
    }
}