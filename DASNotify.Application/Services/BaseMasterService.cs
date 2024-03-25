using DASNotify.Domain.Interfaces.DASNotify;

namespace DASNotify.Application.Services
{
    public class BaseMasterService
    {
        protected IDasNotifyRepositoryWrapper _dasNotifyRepo;
        public BaseMasterService(IDasNotifyRepositoryWrapper dasNotifyRepository)
        {
            _dasNotifyRepo = dasNotifyRepository;
        }
    }
}