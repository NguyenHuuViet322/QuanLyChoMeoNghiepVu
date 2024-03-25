using ESD.Application.Models.ViewModels;
using ESD.Application.Models.ViewModels.DasKTNN;
using ESD.Domain.Models.DAS;
using ESD.Domain.Models.ESDNghiepVu;
using ESD.Domain.Models.DASNotify;
using Profile = AutoMapper.Profile;
using ESD.Application.Models.ViewModels.ESDNghiepVu;

namespace ESD.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VMUser, User>();
            CreateMap<User, VMUser>();
            CreateMap<VMCreateUser, User>();
            CreateMap<User, VMCreateUser>();
            CreateMap<VMEditUser, User>();
            CreateMap<User, VMEditUser>();
            CreateMap<VMCreateUser, VMEditUser>();
            CreateMap<VMEditUser, VMCreateUser>();

            CreateMap<VMAdminUser, User>();
            CreateMap<User, VMAdminUser>();
            CreateMap<VMCreateAdminUser, User>();
            CreateMap<User, VMCreateAdminUser>();
            CreateMap<VMEditAdminUser, User>();
            CreateMap<User, VMEditAdminUser>();

            CreateMap<VMUserProfile, User>();
            CreateMap<User, VMUserProfile>();
            CreateMap<GroupPermission, VMGroupPermision>();
            CreateMap<VMGroupPermision, GroupPermission>();
            CreateMap<VMRole, Role>();
            CreateMap<Role, VMRole>();
            CreateMap<VMCreateRole, Role>();
            CreateMap<Role, VMCreateRole>();
            CreateMap<VMEditRole, Role>();
            CreateMap<Role, VMEditRole>();
            CreateMap<Language, VMLanguage>();
            CreateMap<VMLanguage, Language>();
            CreateMap<Position, VMPosition>();
            CreateMap<VMPosition, Position>();
            //CreateMap<Storage, VMStorage>();
            //CreateMap<VMStorage, Storage>();
            CreateMap<VMExpiryDate, ExpiryDate>();
            CreateMap<VMSercureLevel, SercureLevel>();
            CreateMap<CategoryType, VMCategoryType>();
            CreateMap<VMCategoryType, CategoryType>();
            CreateMap<VMUpdateCategoryType, CategoryType>();
            CreateMap<CategoryType, VMUpdateCategoryType>();
            CreateMap<CategoryTypeField, VMUpdateCategoryTypeField>();
            CreateMap<VMCategoryTypeField, VMUpdateCategoryTypeField>();
            CreateMap<VMUpdateCategoryTypeField, CategoryTypeField>();
            CreateMap<VMUpdateCategoryTypeField, VMCategoryTypeField>();
            CreateMap<VMOrgan, Organ>();
            CreateMap<Organ, VMOrgan>();
            CreateMap<VMEditOrgan, Organ>();
            CreateMap<Organ, VMEditOrgan>();
            CreateMap<VMCreateOrgan, Organ>();
            CreateMap<Organ, VMCreateOrgan>();
            CreateMap<Category, VMCategory>();
            CreateMap<Category, VMCreateCategory>();
            CreateMap<CategoryField, VMCategoryField>();
            CreateMap<CategoryTypeField, VMCategoryTypeField>();
            CreateMap<VMAgency, Agency>();
            CreateMap<Agency, VMAgency>();
            CreateMap<VMEditAgency, Agency>();
            CreateMap<Agency, VMEditAgency>();
            CreateMap<VMCreateAgency, Agency>();
            CreateMap<Agency, VMCreateAgency>();
            CreateMap<DocType, VMDocType>();
            CreateMap<VMUpdateDocType, DocType>();
            CreateMap<VMDocTypeField, DocTypeField>();
            CreateMap<DocTypeField, VMDocTypeField>();
            CreateMap<VMProfileList, ProfileList>();
            CreateMap<VMTeam, Team>();
            CreateMap<Team, VMTeam>();
            CreateMap<VMEditTeam, Team>();
            CreateMap<Team, VMEditTeam>();
            CreateMap<VMCreateTeam, Team>();
            CreateMap<Team, VMCreateTeam>();
            CreateMap<Profile, VMProfile>();
            CreateMap<Profile, VMUpdateProfile>();
            CreateMap<VMProfile, Profile>();
            CreateMap<VMUpdateProfile, Profile>();
            CreateMap<VMProfileTemplate, ProfileTemplate>();
            CreateMap<ProfileTemplate, VMProfileTemplate>();
            CreateMap<VMEditProfileTemplate, ProfileTemplate>();
            CreateMap<ProfileTemplate, VMEditProfileTemplate>();
            CreateMap<VMCreateProfileTemplate, ProfileTemplate>();
            CreateMap<ProfileTemplate, VMCreateProfileTemplate>();
            CreateMap<VMModule, Module>();
            CreateMap<Module, VMModule>();
            CreateMap<VMPlan, Plan>();
            CreateMap<Plan, VMPlan>();
            CreateMap<VMEditPlan, Plan>();
            CreateMap<Plan, VMEditPlan>();
            CreateMap<VMCreatePlan, Plan>();
            CreateMap<Plan, VMCreatePlan>();

            CreateMap<PlanProfile, VMPlanProfile>();
            CreateMap<VMUpdatePlanProfile, PlanProfile>();
            CreateMap<PlanProfile, VMUpdatePlanProfile>();
            CreateMap<PlanAgency, VMPlanAgency>();
            CreateMap<StgFile, VMStgFile>();
            CreateMap<VMStgFile, StgFile>();
            CreateMap<Doc, VMDocCreate>();
            CreateMap<Doc, VMSearchProfileDoc>();
            CreateMap<CatalogingDoc, VMSearchProfileDoc>();
            CreateMap<CatalogingDocField, VMDocField>();
            CreateMap<DocField, VMDocField>();
            CreateMap<DocTypeField, VMDocTypeField>();
            CreateMap<CatalogingProfile, VMCatalogingProfile>();

            CreateMap<CatalogingProfile, VMPlanProfile>();
            CreateMap<VMUpdatePlanProfile, CatalogingProfile>();
            CreateMap<CatalogingProfile, VMUpdatePlanProfile>();
            CreateMap<VMApproveStorage, CatalogingProfile>();
            CreateMap<CatalogingProfile, VMApproveStorage>();
            CreateMap<CatalogingDocField, VMCatalogingDocField>();
            CreateMap<CatalogingDoc, VMCatalogingDocCreate>();
            CreateMap<CatalogingDoc, VMCatalogingDoc>();
            CreateMap<VMDeliveryRecord, DeliveryRecord>();
            CreateMap<DeliveryRecord, VMDeliveryRecord>();
            CreateMap<SystemConfig, VMSystemConfig>();
            CreateMap<VMSystemConfig, SystemConfig>();
            CreateMap<VMTemplate, Template>();
            CreateMap<Template, VMTemplate>();
            CreateMap<CatalogingBorrow, VMCatalogingBorrow>();
            CreateMap<VMUpdateCatalogingBorrow, CatalogingBorrow>();
            CreateMap<Reader, VMReader>();
            CreateMap<Reader, VMReaderRegister>();
            CreateMap<VMReader, Reader>().ForMember(x => x.Birthday, y => y.Ignore());
            CreateMap<OrganConfig, VMOrganConfig>();
            CreateMap<VMOrganConfig, OrganConfig>();
            CreateMap<VMTemplateParam, TemplateParam>();
            CreateMap<TemplateParam, VMTemplateParam>();
            CreateMap<UserBookmark, VMUserBookMark>();

            CreateMap<VMNotification, Notification>();
            CreateMap<Notification, VMNotification>();
            CreateMap<VMCreateDestructionProfile, DestructionProfile>().ForMember(x => x.CreatedAt, y => y.Ignore());
            CreateMap<DestructionProfile, VMDestructionProfile>();

            CreateMap<VMDoc, Doc>();
            CreateMap<Doc, VMDoc>();


            CreateMap<ChuyenMonKiThuat, VMChuyenMonKiThuat>();
            CreateMap<VMChuyenMonKiThuat, ChuyenMonKiThuat>();
            CreateMap<ChuyenMonKiThuat, VMUpdateChuyenMonKiThuat>();
            CreateMap<VMUpdateChuyenMonKiThuat, ChuyenMonKiThuat>();

            CreateMap<CoSoVatChat, VMCoSoVatChat>();
            CreateMap<VMCoSoVatChat, CoSoVatChat>();
            CreateMap<CoSoVatChat, VMUpdateCoSoVatChat>();
            CreateMap<VMUpdateCoSoVatChat, CoSoVatChat>();

            CreateMap<DongVatNghiepVu, VMDongVatNghiepVu>();
            CreateMap<VMDongVatNghiepVu, DongVatNghiepVu>();
            CreateMap<DongVatNghiepVu, VMUpdateDongVatNghiepVu>();
            CreateMap<VMUpdateDongVatNghiepVu, DongVatNghiepVu>();

            CreateMap<DonViNghiepVu, VMDonViNghiepVu>();
            CreateMap<VMDonViNghiepVu, DonViNghiepVu>();
            CreateMap<DonViNghiepVu, VMUpdateDonViNghiepVu>();
            CreateMap<VMUpdateDonViNghiepVu, DonViNghiepVu>();

            CreateMap<LoaiChoNghiepVu, VMLoaiChoNghiepVu>();
            CreateMap<VMLoaiChoNghiepVu, LoaiChoNghiepVu>();
            CreateMap<LoaiChoNghiepVu, VMUpdateLoaiChoNghiepVu>();
            CreateMap<VMUpdateLoaiChoNghiepVu, LoaiChoNghiepVu>();

            CreateMap<NghiepVuDongVat, VMNghiepVuDongVat>();
            CreateMap<VMNghiepVuDongVat, NghiepVuDongVat>();
            CreateMap<NghiepVuDongVat, VMUpdateNghiepVuDongVat>();
            CreateMap<VMUpdateNghiepVuDongVat, NghiepVuDongVat>();

            CreateMap<ThongTinCanBo, VMThongTinCanBo>();
            CreateMap<VMThongTinCanBo, ThongTinCanBo>();
            CreateMap<ThongTinCanBo, VMUpdateThongTinCanBo>();
            CreateMap<VMUpdateThongTinCanBo, ThongTinCanBo>();

            CreateMap<TCDinhLuongAnChoNV, VMTCDinhLuongAnChoNV>();
            CreateMap<VMTCDinhLuongAnChoNV, TCDinhLuongAnChoNV>();
            CreateMap<TCDinhLuongAnChoNV, VMUpdateTCDinhLuongAnChoNV>();
            CreateMap<VMUpdateTCDinhLuongAnChoNV, TCDinhLuongAnChoNV>();

            CreateMap<TCDMTrangBi_DonVi, VMTCDMTrangBi_DonVi>();
            CreateMap<VMTCDMTrangBi_DonVi, TCDMTrangBi_DonVi>();
            CreateMap<TCDMTrangBi_DonVi, VMUpdateTCDMTrangBi_DonVi>();
            CreateMap<VMUpdateTCDMTrangBi_DonVi, TCDMTrangBi_DonVi>();

            CreateMap<TCDMTrangBiCBCS_ChoNV, VMTCDMTrangBiCBCS_ChoNV>();
            CreateMap<VMTCDMTrangBiCBCS_ChoNV, TCDMTrangBiCBCS_ChoNV>();
            CreateMap<TCDMTrangBiCBCS_ChoNV, VMUpdateTCDMTrangBiCBCS_ChoNV>();
            CreateMap<VMUpdateTCDMTrangBiCBCS_ChoNV, TCDMTrangBiCBCS_ChoNV>();

            CreateMap<TCDMTrangBiChoNV, VMTCDMTrangBiChoNV>();
            CreateMap<VMTCDMTrangBiChoNV, TCDMTrangBiChoNV>();
            CreateMap<TCDMTrangBiChoNV, VMUpdateTCDMTrangBiChoNV>();
            CreateMap<VMUpdateTCDMTrangBiChoNV, TCDMTrangBiChoNV>();

            //RenderHere
        }
    }
}