using System.Threading.Tasks;

namespace ESD.Domain.Interfaces.DAS
{
    public interface IDasRepositoryWrapper
    {
       // IDasBaseRepository<T> DasBase { get; }
        IUserRepository User { get; }
        ICategoryRepository Category { get; }
        ICategoryTypeRepository CategoryType { get; }
        IPermissionRepository Permission { get; }
        IGroupPermissionRepository GroupPermission { get; }
        IPermissionGroupPerRepository PermissionGroupPer { get; }
        IRoleRepository Role { get; }
        IRoleGroupPerRepository RoleGroupPer { get; }
        //IUserRoleRepository UserRole { get; }
        IUserGroupPerRepository UserGroupPer { get; }
        IAgencyRepository Agency { get; }
        IModuleRepository Module { get; }
        IModuleChildRepository ModuleChild { get; }
        IPositionRepository Position { get; }
        IOrganRepository Organ { get; }
        ISercureLevelRepository SercureLevel { get; }
        IExpiryDateRepository ExpiryDate { get; }
        ILanguageRepository Language { get; }
        IStorageRepository Storage { get; }
        IProfileTemplateRepository ProfileTemplate { get; }
        IProfileListRepository ProfileList { get; }
        ICodeBoxRepository CodeBox { get; }
        IProfileRepository Profile { get; }
        IDocFieldRepository DocField { get; }
        IDocTypeRepository DocType { get; }
        ICategoryTypeFieldRepository CategoryTypeField { get; }
        IDocTypeFieldRepository DocTypeField { get; }
        ICategoryFieldRepository CategoryField { get; }
        IDataTypeRepository DataType { get; }
        ITeamRepository Team { get; }
        //ITeamRoleRepository TeamRole { get; }
        ITeamGroupPerRepository TeamGroupPer { get; }
        IUserTeamRepository UserTeam { get; }
        IStgFileRepository StgFile { get; }
        IDownloadLinkRepository DownloadLink { get; }
        IEmailRepository Email { get; }
        IResetPasswordRepository ResetPassword { get; }
        IAccountRepository Account { get; }
        IPlanRepository Plan { get; }
        IPlanProfileRepository PlanProfile { get; }
        IPlanAgencyRepository PlanAgency { get; }
        IDocRepository Doc { get; }
        ICatalogingProfileRepository CatalogingProfile { get; }
        ICatalogingDocRepository CatalogingDoc { get; }
        ICatalogingDocFieldRepository CatalogingDocField { get; }
        IDeliveryRecordRepository DeliveryRecord { get; }
        ISystemConfigRepository SystemConfig { get; }
        ITemplateRepository Template { get; }
        ITemplateParamRepository TemplateParam { get; }
        IReaderRepository Reader { get; }
        IOrganConfigRepository OrganConfig { get; }
        ICatalogingBorrowRepository CatalogingBorrow { get; }
        ICatalogingBorrowDocRepository CatalogingBorrowDoc { get; }
        IReaderInOrganRepository ReaderInOrgan { get;  }
        IUserBookMarkRepository UserBookMark { get; }
        IDestructionProfileRepository DestructionProfile { get; }
        IProfileDestroyedRepository ProfileDestroyed { get; }
        ILogSystemCRUDRepository LogSystemCRUD { get; }
        ILogUserActionRepository LogUserAction { get; }
        Task SaveAync();
    }
}