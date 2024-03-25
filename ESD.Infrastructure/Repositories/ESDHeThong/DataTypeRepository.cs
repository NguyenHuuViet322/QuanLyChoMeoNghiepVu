using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DataTypeRepository : DasBaseRepository<DataType>, IDataTypeRepository
    {
        public DataTypeRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}