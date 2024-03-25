using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DeliveryRecordRepository : DasBaseRepository<DeliveryRecord>, IDeliveryRecordRepository
    {
        public DeliveryRecordRepository(ESDContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
