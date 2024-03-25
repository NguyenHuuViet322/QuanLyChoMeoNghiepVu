using ESD.Application.Interfaces;
using ESD.Application.Interfaces.DasKTNN;
using ESD.Application.Interfaces.ESDNghiepVu;
using ESD.Application.Services.DasKTNN;
using ESD.Application.Services.ESDNghiepVu;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Interfaces.DASNotify;
using ESD.Domain.Interfaces.ESDNghiepVu;
using ESD.Infrastructure.ContextAccessors;
using ESD.Infrastructure.HttpClientAccessors.Implementations;
using ESD.Infrastructure.HttpClientAccessors.Interfaces;
using ESD.Infrastructure.Notifications;
using ESD.Infrastructure.Repositories.DAS;
using ESD.Infrastructure.Repositories.DASNotify;
using ESD.Infrastructure.Repositories.ESDNghiepVu;
using Microsoft.Extensions.DependencyInjection;

namespace ESD.Application.Services
{
    public static class DIServiceWrapper
    {
        public static void DependencyInjectionService(this IServiceCollection services)
        {
            services.AddSingleton<ILogHttpClient, LogHttpClient>();
            // HttpClientService
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddSingleton<IBaseHttpClientFactory, BaseHttpClientFactory>();
            services.AddScoped<IStgFileClientService, StgFileClientService>();
            services.AddScoped<IDefaultDataService, DefaultDataService>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAgencyServices, AgencyService>();
            //services.AddScoped<IArchiveManagementService, ArchiveManagementService>();
            services.AddScoped<IAuthorizeService, AuthorizeService>();
            services.AddScoped<ICatalogingProfileService, CatalogingProfileService>();
            services.AddScoped<ICategoryServices, CategoryService>();
            services.AddScoped<ICategoryTypeServices, CategoryTypeService>();
            services.AddScoped<IDefaultDataService, DefaultDataService>();
            //services.AddScoped<IDeliveryRecordServices, DeliveryRecordServices>();
            // services.AddScoped<IDocBorrowServices, DocBorrowService>();
            // services.AddScoped<IDocTypeServices, DocTypeService>();
            services.AddScoped<IExcelServices, ExcelService>();
            // services.AddScoped<IExpiryDateServices, ExpiryDateService>();
            services.AddScoped<IGroupPermissionService, GroupPermissionService>();
            services.AddScoped<IHomeServices, HomeService>();
            services.AddScoped<IIPAddressClientServices, IPAddressClientService>();
            services.AddScoped<ILanguageServices, LanguageService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IOrganConfigServices, OrganConfigService>();
            services.AddScoped<IOrganServices, OrganService>();
            services.AddScoped<IPermissionService, PermissionService>();
            //services.AddScoped<IPlanDocServices, PlanDocService>();
            //services.AddScoped<IPlanServices, PlanService>();
            services.AddScoped<IPositionServices, PositionService>();
            //services.AddScoped<IProfileListService, ProfileListService>();
            //services.AddScoped<IProfileService, ProfileService>();
            //services.AddScoped<IProfileTemplateServices, ProfileTemplateService>();
            services.AddScoped<IReaderServices, ReaderService>();
            //services.AddScoped<IReportArchiveServices, ReportArchiveService>();
            services.AddScoped<IResetPasswordService, ResetPasswordService>();
            services.AddScoped<IRoleServices, RoleService>();
            services.AddScoped<ISercureLevelServices, SercureLevelService>();
            services.AddScoped<IStgFileService, StgFileService>();
            // services.AddScoped<IStorageServices, StorageService>();
            services.AddScoped<ISystemConfigServices, SystemConfigService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ITemplateServices, TemplateServices>();
            services.AddScoped<IUserLogServices, UserLogService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IProfileCategoryServices, ProfileCategoryService>();
            services.AddScoped<IUserBookMarkServices, UserBookMarkService>();
            //services.AddScoped<IProfileDestructionServices, ProfileDestructionService>();
            services.AddScoped<ICacheManagementServices, CacheManagementService>();
            services.AddScoped<IDataApiServices, DataApiServices>();

            services.AddScoped<ISendNotificationServices, SendNotificationService>();
            services.AddScoped<INotificationConfigService, NotificationConfigService>();

            services.AddScoped<IChuyenMonKiThuatServices, ChuyenMonKiThuatService>();

            services.AddScoped<ICoSoVatChatServices, CoSoVatChatService>();

            services.AddScoped<IDongVatNghiepVuServices, DongVatNghiepVuService>();

            services.AddScoped<IDonViNghiepVuServices, DonViNghiepVuService>();

            services.AddScoped<ILoaiChoNghiepVuServices, LoaiChoNghiepVuService>();

            services.AddScoped<INghiepVuDongVatServices, NghiepVuDongVatService>();

            services.AddScoped<IThongTinCanBoServices, ThongTinCanBoService>();

            services.AddScoped<IUploadServices, UploadService>();

            services.AddScoped<ITCDinhLuongAnChoNVServices, TCDinhLuongAnChoNVService>();

            services.AddScoped<ITCDMTrangBi_DonViServices, TCDMTrangBi_DonViService>();

            services.AddScoped<ITCDMTrangBiCBCS_ChoNVServices, TCDMTrangBiCBCS_ChoNVService>();

            services.AddScoped<ITCDMTrangBiChoNVServices, TCDMTrangBiChoNVService>();

            //RenderHere
        }
    }
}