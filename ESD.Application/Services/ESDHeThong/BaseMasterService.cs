using ESD.Domain.Interfaces;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Infrastructure.Repositories.DASKTNN;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ESD.Application.Services
{
    public class BaseMasterService
    {
        protected IDasRepositoryWrapper _dasRepo;
        protected IDasNotifyRepositoryWrapper _dasNotifyRepo;
        protected IESDNghiepVuRepositoryWrapper _dasNghiepVuRepo;
        protected IDasDataDapperRepo _dasDapperRepo;
        protected readonly IDbConnection _dasDataDapperConn;
        protected readonly IDbConnection _dasDynamicDbDapperConn;
        protected readonly IDbConnection _apiManageDbDapperConn;

        public BaseMasterService(IDasRepositoryWrapper dasRepository)
        {
            _dasRepo = dasRepository;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasDataDapperRepo dasdataDapperRepo)
        {
            _dasRepo = dasRepository;
            _dasDataDapperConn = dasdataDapperRepo.idbConnection;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasDataDapperRepo dasdataDapperRepo, IDynamicDBDapperRepo dasdynaicDapperRepo)
        {
            _dasRepo = dasRepository;
            _dasDataDapperConn = dasdataDapperRepo.idbConnection;
            _dasDynamicDbDapperConn = dasdynaicDapperRepo.idbConnection;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IAPIManageDBDapperRepo apiMangageDBDapperRepo)
        {
            _dasRepo = dasRepository;
            _apiManageDbDapperConn = apiMangageDBDapperRepo.idbConnection;
        }

        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasNotifyRepositoryWrapper dasNotifyRepository)
        {
            _dasRepo = dasRepository;
            _dasNotifyRepo = dasNotifyRepository;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasNotifyRepositoryWrapper dasNotifyRepository, IDasDataDapperRepo dasdataDapperRepo, IDynamicDBDapperRepo dasdynaicDapperRepo)
        {
            _dasRepo = dasRepository;
            _dasNotifyRepo = dasNotifyRepository;
            _dasDataDapperConn = dasdataDapperRepo.idbConnection;
            _dasDynamicDbDapperConn = dasdynaicDapperRepo.idbConnection;
        }
        public BaseMasterService(IDasNotifyRepositoryWrapper dasNotifyRepository)
        {
            _dasNotifyRepo = dasNotifyRepository;
        }
        public BaseMasterService(IDasNotifyRepositoryWrapper dasNotifyRepository, IDasDataDapperRepo dasdataDapperRepo)
        {
            _dasNotifyRepo = dasNotifyRepository;
            _dasDataDapperConn = dasdataDapperRepo.idbConnection;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IESDNghiepVuRepositoryWrapper dasKTNNRepository)
        {
            _dasRepo = dasRepository;
            _dasNghiepVuRepo = dasKTNNRepository;
        }

        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasDataDapperRepo dasDapperRepo, IESDNghiepVuRepositoryWrapper dasKTNNRepository)
        {
            _dasRepo = dasRepository;
            _dasDataDapperConn = dasDapperRepo.idbConnection;
            _dasNghiepVuRepo = dasKTNNRepository;
        }
        public BaseMasterService(IDasRepositoryWrapper dasRepository, IDasDataDapperRepo dasdataDapperRepo, IESDNghiepVuRepositoryWrapper dasKTNNRepositoryWrapper, IDynamicDBDapperRepo dasdynaicDapperRepo)
        {
            _dasRepo = dasRepository;
            _dasNghiepVuRepo = dasKTNNRepositoryWrapper;
            _dasDataDapperConn = dasdataDapperRepo.idbConnection;
            _dasDynamicDbDapperConn = dasdynaicDapperRepo.idbConnection;
        }
    }
}