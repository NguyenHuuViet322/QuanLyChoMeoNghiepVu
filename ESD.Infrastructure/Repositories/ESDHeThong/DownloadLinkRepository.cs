using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DownloadLinkRepository : DasBaseRepository<DownloadLink>, IDownloadLinkRepository
    {
        public DownloadLinkRepository(ESDContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
