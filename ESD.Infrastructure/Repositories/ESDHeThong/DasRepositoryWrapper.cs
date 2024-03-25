using ESD.Domain.Interfaces.DAS;
using ESD.Infrastructure.Contexts;
using System.Threading.Tasks;

namespace ESD.Infrastructure.Repositories.DAS
{
    public class DasRepositoryWrapper : IDasRepositoryWrapper
    {
        #region ctor

        private readonly ESDContext _repoContext;

        public DasRepositoryWrapper(ESDContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        #endregion ctor

        #region Properties
        private IProfileDestroyedRepository _profileDestroyed;
        public IProfileDestroyedRepository ProfileDestroyed
        {
            get
            {
                if (_profileDestroyed == null)
                {
                    _profileDestroyed = new ProfileDestroyedRepository(_repoContext);
                }
                return _profileDestroyed;
            }
        }
        private IDestructionProfileRepository _destructionProfile;
        public IDestructionProfileRepository DestructionProfile
        {
            get
            {
                if (_destructionProfile == null)
                {
                    _destructionProfile = new DestructionProfileRepository(_repoContext);
                }
                return _destructionProfile;
            }
        }
        private IUserBookMarkRepository _userBookMark;
        public IUserBookMarkRepository UserBookMark
        {
            get
            {
                if (_userBookMark == null)
                {
                    _userBookMark = new UserBookMarkRepository(_repoContext);
                }
                return _userBookMark;
            }
        }
        private IReaderInOrganRepository _readerInOrgan;
        public IReaderInOrganRepository ReaderInOrgan
        {
            get
            {
                if (_readerInOrgan == null)
                {
                    _readerInOrgan = new ReaderInOrganRepository(_repoContext);
                }
                return _readerInOrgan;
            }
        }
        private IOrganConfigRepository _organConfig;
        public IOrganConfigRepository OrganConfig
        {
            get
            {
                if (_organConfig == null)
                {
                    _organConfig = new OrganConfigRepository(_repoContext);
                }
                return _organConfig;
            }
        }
        private IReaderRepository _reader;
        public IReaderRepository Reader
        {
            get
            {
                if (_reader == null)
                {
                    _reader = new ReaderRepository(_repoContext);
                }
                return _reader;
            }
        }
        private ISystemConfigRepository _systemConfig;
        public ISystemConfigRepository SystemConfig
        {
            get
            {
                if (_systemConfig == null)
                {
                    _systemConfig = new SystemConfigRepository(_repoContext);
                }
                return _systemConfig;
            }
        }

        private IUserRepository _user;

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }
        }
        private ILogSystemCRUDRepository _LogSystemCRUD;

        public ILogSystemCRUDRepository LogSystemCRUD
        {
            get
            {
                if (_LogSystemCRUD == null)
                {
                    _LogSystemCRUD = new LogSystemCRUDRepository(_repoContext);
                }
                return _LogSystemCRUD;
            }
        }
        private ILogUserActionRepository _LogUserAction;

        public ILogUserActionRepository LogUserAction
        {
            get
            {
                if (_LogUserAction == null)
                {
                    _LogUserAction = new LogUserActionRepository(_repoContext);
                }
                return _LogUserAction;
            }
        }
        private ICategoryRepository _category;

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_repoContext);
                }
                return _category;
            }
        }

        private ICategoryTypeRepository _categoryType;

        public ICategoryTypeRepository CategoryType
        {
            get
            {
                if (_categoryType == null)
                {
                    _categoryType = new CategoryTypeRepository(_repoContext);
                }
                return _categoryType;
            }
        }

        private IPermissionRepository _permission;

        public IPermissionRepository Permission
        {
            get
            {
                if (_permission == null)
                {
                    _permission = new PermissionRepository(_repoContext);
                }
                return _permission;
            }
        }

        private IGroupPermissionRepository _groupPermission;

        public IGroupPermissionRepository GroupPermission
        {
            get
            {
                if (_groupPermission == null)
                {
                    _groupPermission = new GroupPermissionRepository(_repoContext);
                }
                return _groupPermission;
            }
        }

        private IPermissionGroupPerRepository _permissionGroupPer;

        public IPermissionGroupPerRepository PermissionGroupPer
        {
            get
            {
                if (_permissionGroupPer == null)
                {
                    _permissionGroupPer = new PermissionGroupPerRepository(_repoContext);
                }
                return _permissionGroupPer;
            }
        }

        private IRoleRepository _role;

        public IRoleRepository Role
        {
            get
            {
                if (_role == null)
                {
                    _role = new RoleRepository(_repoContext);
                }
                return _role;
            }
        }

        private IRoleGroupPerRepository _roleGroupPer;

        public IRoleGroupPerRepository RoleGroupPer
        {
            get
            {
                if (_roleGroupPer == null)
                {
                    _roleGroupPer = new RoleGroupPerRepository(_repoContext);
                }
                return _roleGroupPer;
            }
        }

        //private IUserRoleRepository _userRole;

        //public IUserRoleRepository UserRole
        //{
        //    get
        //    {
        //        if (_userRole == null)
        //        {
        //            _userRole = new UserRoleRepository(_repoContext);
        //        }
        //        return _userRole;
        //    }
        //}

        private IUserGroupPerRepository _userGroupPer;

        public IUserGroupPerRepository UserGroupPer
        {
            get
            {
                if (_userGroupPer == null)
                {
                    _userGroupPer = new UserGroupPerRepository(_repoContext);
                }
                return _userGroupPer;
            }
        }

        private IAgencyRepository _Agency;

        public IAgencyRepository Agency
        {
            get
            {
                if (_Agency == null)
                {
                    _Agency = new AgencyRepository(_repoContext);
                }
                return _Agency;
            }
        }

        private IModuleRepository _module;

        public IModuleRepository Module
        {
            get
            {
                if (_module == null)
                {
                    _module = new ModuleRepository(_repoContext);
                }
                return _module;
            }
        }

        private IModuleChildRepository _moduleChild;

        public IModuleChildRepository ModuleChild
        {
            get
            {
                if (_moduleChild == null)
                {
                    _moduleChild = new ModuleChildRepository(_repoContext);
                }
                return _moduleChild;
            }
        }

        private IPositionRepository _position;

        public IPositionRepository Position
        {
            get
            {
                if (_position == null)
                {
                    _position = new PositionRepository(_repoContext);
                }
                return _position;
            }
        }

        private IOrganRepository _Organ;

        public IOrganRepository Organ
        {
            get
            {
                if (_Organ == null)
                {
                    _Organ = new OrganRepository(_repoContext);
                }
                return _Organ;
            }
        }

        private ISercureLevelRepository _sercureLevel;
        public ISercureLevelRepository SercureLevel
        {
            get
            {
                if (_sercureLevel == null)
                {
                    _sercureLevel = new SercureLevelRepository(_repoContext);
                }
                return _sercureLevel;
            }
        }

        private IExpiryDateRepository _expiryDate;
        public IExpiryDateRepository ExpiryDate
        {
            get
            {
                if (_expiryDate == null)
                {
                    _expiryDate = new ExpiryDateRepository(_repoContext);
                }
                return _expiryDate;
            }
        }

        private ILanguageRepository _language;
        public ILanguageRepository Language
        {
            get
            {
                if (_language == null)
                {
                    _language = new LanguageRepository(_repoContext);
                }
                return _language;
            }
        }

        private IStorageRepository _storage;
        public IStorageRepository Storage
        {
            get
            {
                if (_storage == null)
                {
                    _storage = new StorageRepository(_repoContext);
                }
                return _storage;
            }
        }

        private IProfileTemplateRepository _profileTemplate;
        public IProfileTemplateRepository ProfileTemplate
        {
            get
            {
                if (_profileTemplate == null)
                {
                    _profileTemplate = new ProfileTemplateRepository(_repoContext);
                }
                return _profileTemplate;
            }
        }

        private IProfileListRepository _profileList;
        public IProfileListRepository ProfileList
        {
            get
            {
                if (_profileList == null)
                {
                    _profileList = new ProfileListRepository(_repoContext);
                }
                return _profileList;
            }
        }

        private ICodeBoxRepository _codeBox;
        public ICodeBoxRepository CodeBox
        {
            get
            {
                if (_codeBox == null)
                {
                    _codeBox = new CodeBoxRepository(_repoContext);
                }
                return _codeBox;
            }
        }

        private IProfileRepository _profile;
        public IProfileRepository Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = new ProfileRepository(_repoContext);
                }
                return _profile;
            }
        }

        private IDocFieldRepository _docField;
        public IDocFieldRepository DocField
        {
            get
            {
                if (_docField == null)
                {
                    _docField = new DocFieldRepository(_repoContext);
                }
                return _docField;
            }
        }

        private IDocTypeRepository _docType;
        public IDocTypeRepository DocType
        {
            get
            {
                if (_docType == null)
                {
                    _docType = new DocTypeRepository(_repoContext);
                }
                return _docType;
            }
        }

        private ICategoryTypeFieldRepository _categoryTypeField;
        public ICategoryTypeFieldRepository CategoryTypeField
        {
            get
            {
                if (_categoryTypeField == null)
                {
                    _categoryTypeField = new CategoryTypeFieldRepository(_repoContext);
                }
                return _categoryTypeField;
            }
        }

        private ICategoryFieldRepository _categoryField;
        public ICategoryFieldRepository CategoryField
        {
            get
            {
                if (_categoryField == null)
                {
                    _categoryField = new CategoryFieldRepository(_repoContext);
                }
                return _categoryField;
            }
        }

        private IDataTypeRepository _dataType;
        public IDataTypeRepository DataType
        {
            get
            {
                if (_dataType == null)
                {
                    _dataType = new DataTypeRepository(_repoContext);
                }
                return _dataType;
            }
        }

        private ITeamRepository _team;

        public ITeamRepository Team
        {
            get
            {
                if (_team == null)
                {
                    _team = new TeamRepository(_repoContext);
                }
                return _team;
            }
        }

        //private ITeamRoleRepository _teamRole;

        //public ITeamRoleRepository TeamRole
        //{
        //    get
        //    {
        //        if (_teamRole == null)
        //        {
        //            _teamRole = new TeamRoleRepository(_repoContext);
        //        }
        //        return _teamRole;
        //    }
        //}

        private ITeamGroupPerRepository _teamGroupPer;

        public ITeamGroupPerRepository TeamGroupPer
        {
            get
            {
                if (_teamGroupPer == null)
                {
                    _teamGroupPer = new TeamGroupPerRepository(_repoContext);
                }
                return _teamGroupPer;
            }
        }

        private IUserTeamRepository _userTeam;
        public IUserTeamRepository UserTeam
        {
            get
            {
                if (_userTeam == null)
                {
                    _userTeam = new UserTeamRepository(_repoContext);
                }
                return _userTeam;
            }
        }

        private IStgFileRepository _stgFile;
        public IStgFileRepository StgFile
        {
            get
            {
                if (_stgFile == null)
                {
                    _stgFile = new StgFileRepository(_repoContext);
                }
                return _stgFile;

            }
        }

        private IDownloadLinkRepository _downloadLink;

        public IDownloadLinkRepository DownloadLink
        {
            get
            {
                if (_downloadLink == null)
                {
                    _downloadLink = new DownloadLinkRepository(_repoContext);
                }
                return _downloadLink;

            }
        }

        private IDocTypeFieldRepository _docTypeField;
        public IDocTypeFieldRepository DocTypeField
        {
            get
            {
                if (_docTypeField == null)
                {
                    _docTypeField = new DocTypeFieldRepository(_repoContext);
                }
                return _docTypeField;
            }
        }
        private IEmailRepository _email;
        public IEmailRepository Email
        {
            get
            {
                if (_email == null)
                {
                    _email = new EmailRepository(_repoContext);
                }
                return _email;
            }
        }
        private IResetPasswordRepository _ResetPassword;
        public IResetPasswordRepository ResetPassword
        {
            get
            {
                if (_ResetPassword == null)
                {
                    _ResetPassword = new ResetPasswordRepository(_repoContext);
                }
                return _ResetPassword;
            }
        }
        private IAccountRepository _account;
        public IAccountRepository Account
        {
            get
            {
                if (_account == null)
                {
                    _account = new AccountRepository(_repoContext);
                }
                return _account;
            }
        }

        private IPlanRepository _Plan;
        public IPlanRepository Plan
        {
            get
            {
                if (_Plan == null)
                {
                    _Plan = new PlanRepository(_repoContext);
                }
                return _Plan;
            }
        }
        private IPlanProfileRepository _PlanProfile;
        public IPlanProfileRepository PlanProfile
        {
            get
            {
                if (_PlanProfile == null)
                {
                    _PlanProfile = new PlanProfileRepository(_repoContext);
                }
                return _PlanProfile;
            }
        }
        private IPlanAgencyRepository _PlanAgency;
        public IPlanAgencyRepository PlanAgency
        {
            get
            {
                if (_PlanAgency == null)
                {
                    _PlanAgency = new PlanAgencyRepository(_repoContext);
                }
                return _PlanAgency;
            }
        }

        private IDocRepository _Doc;
        public IDocRepository Doc
        {
            get
            {
                if (_Doc == null)
                {
                    _Doc = new DocRepository(_repoContext);
                }
                return _Doc;
            }
        }

        private ICatalogingProfileRepository _CatalogingProfile;
        public ICatalogingProfileRepository CatalogingProfile
        {
            get
            {
                if (_CatalogingProfile == null)
                {
                    _CatalogingProfile = new CatalogingProfileRepository(_repoContext);
                }
                return _CatalogingProfile;
            }
        }
        private ICatalogingDocRepository _CatalogingDoc;
        public ICatalogingDocRepository CatalogingDoc
        {
            get
            {
                if (_CatalogingDoc == null)
                {
                    _CatalogingDoc = new CatalogingDocRepository(_repoContext);
                }
                return _CatalogingDoc;
            }
        }
        private ICatalogingDocFieldRepository _CatalogingDocField;
        public ICatalogingDocFieldRepository CatalogingDocField
        {
            get
            {
                if (_CatalogingDocField == null)
                {
                    _CatalogingDocField = new CatalogingDocFieldRepository(_repoContext);
                }
                return _CatalogingDocField;
            }
        }
        private IDeliveryRecordRepository _DeliveryRecord;
        public IDeliveryRecordRepository DeliveryRecord
        {
            get
            {
                if (_DeliveryRecord == null)
                {
                    _DeliveryRecord = new DeliveryRecordRepository(_repoContext);
                }
                return _DeliveryRecord;
            }
        }

        private ITemplateRepository _Template;
        public ITemplateRepository Template
        {
            get
            {
                if (_Template == null)
                {
                    _Template = new TemplateRepository(_repoContext);
                }
                return _Template;
            }
        }

        private ITemplateParamRepository _templateParam;
        public ITemplateParamRepository TemplateParam
        {
            get
            {
                if (_templateParam == null)
                {
                    _templateParam = new TemplateParamRepository(_repoContext);
                }
                return _templateParam;
            }
        }

        private ICatalogingBorrowRepository _catalogingBorrow;
        public ICatalogingBorrowRepository CatalogingBorrow
        {
            get
            {
                if (_catalogingBorrow == null)
                {
                    _catalogingBorrow = new CatalogingBorrowRepository(_repoContext);
                }
                return _catalogingBorrow;
            }
        }
        private ICatalogingBorrowDocRepository _catalogingBorrowDoc;
        public ICatalogingBorrowDocRepository CatalogingBorrowDoc
        {
            get
            {
                if (_catalogingBorrowDoc == null)
                {
                    _catalogingBorrowDoc = new CatalogingBorrowDocRepository(_repoContext);
                }
                return _catalogingBorrowDoc;
            }
        }
        #endregion Properties

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}