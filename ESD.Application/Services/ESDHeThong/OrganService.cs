using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ESD.Domain.Enums;
using System.Runtime.CompilerServices;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using ESD.Application.Constants;
using Microsoft.Extensions.Caching.Distributed;
using ESD.Utility.CacheUtils;
using ESD.Utility;

namespace ESD.Application.Services
{
    public class OrganService : BaseMasterService, IOrganServices
    {
        private readonly IMapper _mapper;
        private readonly IDefaultDataService _defaultDataService;
        private readonly ICacheManagementServices _cacheManagementServices;

        public OrganService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , IDefaultDataService defaultDataService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _defaultDataService = defaultDataService;
            _cacheManagementServices = cacheManagementServices;
        }

        #region BaseRepo
        public async Task<IEnumerable<Organ>> Gets()
        {
            return await _dasRepo.Organ.GetAllListAsync();
        }
        public async Task<Organ> Get(object id)
        {
            return await _dasRepo.Organ.GetAsync(id);
        }

        public async Task<ServiceResult> Create(Organ model)
        {
            await _dasRepo.Organ.InsertAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Created Successfully");
        }
        public async Task<ServiceResult> Update(Organ model)
        {
            await _dasRepo.Organ.UpdateAsync(model);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Add category suceess!");
        }

        public async Task<ServiceResult> Delete(object id)
        {
            throw new NotImplementedException();
        }
        #endregion BaseRepo

        #region Create & Search
        public async Task<ServiceResult> CreateOrgan(VMCreateOrgan vmOrgan)
        {
            //check exist unique field
            List<Organ> listExist;
            listExist = await _dasRepo.Organ.GetAll().Where(m => (m.Code == vmOrgan.Code) || (m.Email == vmOrgan.Email)).ToListAsync();
            if (IsExisted(listExist))
            {
                if (IsExisted(listExist.Where(m => m.Code == vmOrgan.Code)))
                    return new ServiceResultError("Mã cơ quan đã tồn tại!");
                else
                    return new ServiceResultError("Email đã tồn tại!");
            }

            //update data
            UpdateData(vmOrgan);

            Organ organ = _mapper.Map<Organ>(vmOrgan);
            await _dasRepo.Organ.InsertAsync(organ);
            await _dasRepo.SaveAync();
            if (organ.ID == 0)
                return new ServiceResultError("Thêm mới cơ quan không thành công!");
            else
            {
                await InsertDefaultData(organ.ID);
            }

            return new ServiceResultSuccess("Thêm mới cơ quan thành công!");
        }


        public async Task<PaginatedList<VMOrgan>> SearchByConditionPagging(OrganCondition condition)
        {
            var temp = from x in _dasRepo.Organ.GetAll()
                       where (x.Status == (int)EnumOrgan.Status.Active
                       && (string.IsNullOrEmpty(condition.Keyword) || x.Name.Contains(condition.Keyword) || x.Code.Contains(condition.Keyword)))
                       join y in _dasRepo.Organ.GetAll() on x.ParentId equals y.ID into right
                       from r in right.DefaultIfEmpty()
                       orderby x.ID descending
                       select new VMOrgan
                       {
                           ID = x.ID,
                           Name = x.Name,
                           Code = x.Code,
                           Address = x.Address,
                           Description = x.Description,
                           Fax = x.Fax,
                           //IsArchive = x.IsArchive,
                           //ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0,
                           Phone = x.Phone,
                           //ParentName = r.Name
                       };

            var total = await temp.LongCountAsync();
            if (total == 0)
                return null;

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;

            var organs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            var vmOrgans = _mapper.Map<List<VMOrgan>>(organs);

            return new PaginatedList<VMOrgan>(vmOrgans.ToList(), (int)total, condition.PageIndex, condition.PageSize);
        }

        public async Task<IEnumerable<VMOrgan>> GetListByCondition(OrganCondition condition)
        {
            var temp = from x in _dasRepo.Organ.GetAll()
                       where (x.Status == (int)EnumOrgan.Status.Active && (string.IsNullOrEmpty(condition.Keyword) || x.Name.Contains(condition.Keyword)))
                       join y in _dasRepo.Organ.GetAll() on x.ParentId equals y.ID into right
                       from r in right.DefaultIfEmpty()
                       orderby x.ID descending
                       select new VMOrgan
                       {
                           ID = x.ID,
                           Name = x.Name,
                           Code = x.Code,
                           Address = x.Address,
                           Description = x.Description,
                           Fax = x.Fax,
                           //IsArchive = x.IsArchive,
                           //ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0,
                           Phone = x.Phone,
                           //ParentName = r.Name
                       };
            return await temp.ToListAsync();
        }
        #endregion Create & Search

        #region Get
        public async Task<VMOrgan> GetDetail(int id)
        {
            var temp = from x in _dasRepo.Organ.GetAll()
                       where x.Status == (int)EnumOrgan.Status.Active && x.ID == id
                       join y in _dasRepo.Organ.GetAll() on x.ParentId equals y.ID into right
                       from r in right.DefaultIfEmpty()
                       select new VMOrgan
                       {
                           ID = x.ID,
                           Name = x.Name,
                           Code = x.Code,
                           Address = x.Address,
                           Description = x.Description,
                           Fax = x.Fax,
                           Email = x.Email,
                           //IsArchive = x.IsArchive,
                           //ParentId = x.ParentId.HasValue ? x.ParentId.Value : 0,
                           Phone = x.Phone,
                           //ParentName = r.Name
                       };
            var organ = await temp.FirstOrDefaultAsync();
            return organ;
        }

        public async Task<VMEditOrgan> GetOrgan(int id)
        {
            //get organ by id
            var organ = await _dasRepo.Organ.GetAsync(id);
            if (!IsExisted(organ))
                return null;
            var vmOrgan = _mapper.Map<VMEditOrgan>(organ);

            //get all list organ without this organ and child this organ
            var organs = await GetParentOrgan(id);
            if (!IsExisted(organs))
                return vmOrgan;
            vmOrgan.Parents = organs;

            return vmOrgan;
        }

        public async Task<IEnumerable<Organ>> GetParentOrgan(int id)
        {
            //get all list organ without this organ and child this organ
            var temp = from a in _dasRepo.Organ.GetAll()
                       where a.Status == (int)EnumOrgan.Status.Active && id > 0 ? a.ID != id && a.ParentId != id : true
                       select new Organ
                       {
                           ID = a.ID,
                           Name = a.Name
                       };

            var organs = await temp.ToListAsync();
            return organs;
        }

        public async Task<IEnumerable<Organ>> GetActive(bool isShowAll = false)
        {
            if (isShowAll)
                return await _dasRepo.Organ.GetAll().Where(a => a.Status == (int)EnumOrgan.Status.Active).OrderBy(a => a.Name).ToListAsync();

            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return await _dasRepo.Organ.GetAll().Where(a => a.Status == (int)EnumOrgan.Status.Active && a.ID == userData.IDOrgan).OrderBy(a => a.Name).ToListAsync();
        }
        #endregion Get

        #region Update    
        public async Task<ServiceResult> UpdateOrgan(VMEditOrgan vmOrgan)
        {
            var organ = await _dasRepo.Organ.GetAsync(vmOrgan.ID);
            if (!IsExisted(organ))
                return new ServiceResultError("Không tồn tại cơ quan này!");

            //check exist unique field
            List<Organ> listExistRole;
            listExistRole = await _dasRepo.Organ.GetAll().Where(m => (m.Code == vmOrgan.Code && m.Code != organ.Code)
            || (m.Email == vmOrgan.Email && m.Email != organ.Email)).ToListAsync();
            if (IsExisted(listExistRole))
            {
                if (IsExisted(listExistRole.Where(m => m.Code == vmOrgan.Code && m.Code != organ.Code)))
                    return new ServiceResultError("Mã cơ quan đã tồn tại!");
                else
                    return new ServiceResultError("Email đã tồn tại!");
            }


            //update data
            UpdateData(vmOrgan);

            _mapper.Map(vmOrgan, organ);
            await _dasRepo.Organ.UpdateAsync(organ);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Cập nhật cơ quan thành công!");
        }
        #endregion Update

        #region Delete
        public async Task<ServiceResult> DeleteMultiOrgan(IEnumerable<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    await DeleteOrgan(id);
                }
            }
            catch (Exception)
            {
                return new ServiceResultError("Xóa cơ quan không thành công!");
            }

            return new ServiceResultSuccess("Xóa cơ quan thành công!");
        }


        public async Task<ServiceResult> DeleteOrgan(int id)
        {
            var organ = await _dasRepo.Organ.GetAsync(id);
            if (organ.Status != (int)EnumOrgan.Status.Active)
                return new ServiceResultError("Cơ quan này không tồn tại!");
            //update status this Organ
            organ.Status = (int)EnumOrgan.Status.InActive;
            await _dasRepo.Organ.UpdateAsync(organ);

            //get Agencys in Organ
            var Agencys = await _dasRepo.Agency.GetAllListAsync(m => m.Status == (int)EnumAgency.Status.Active && m.IDOrgan == id);
            if (IsExisted(Agencys))
            {
                //update Agencys set status = 0
                Agencys.Select(a => { a.Status = 0; return a; }).ToList();

                //update Agencys
                await _dasRepo.Agency.UpdateAsync(Agencys);
            }

            //get parent
            var organs = await _dasRepo.Organ.GetAll().Where(a => a.ParentId == id && a.Status == (int)EnumOrgan.Status.Active).ToListAsync();
            if (IsExisted(organs))
            {
                //update organs set parentid = 0
                organs.Select(a => { a.ParentId = 0; return a; }).ToList();

                //update Organs
                await _dasRepo.Organ.UpdateAsync(organs);
            }

            //get users in Organ
            var users = await _dasRepo.User.GetAllListAsync(m => m.Status == (int)EnumAgency.Status.Active && m.IDOrgan == id);
            if (IsExisted(users))
            {
                //update users set status = 0
                users.Select(a => { a.Status = 0; return a; }).ToList();

                //update users
                await _dasRepo.User.UpdateAsync(users);
            }

            //update fond(waiting)

            await _dasRepo.SaveAync();




            return new ServiceResultSuccess("Xóa cơ quan thành công!");
        }
        #endregion

        #region Commom
        public async Task<List<SelectListItem>> GetOrgans(string ids)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(ids))
                list = JsonConvert.DeserializeObject<List<string>>(ids);

            var organs = await _dasRepo.Organ.GetAllListAsync();

            var model = new List<SelectListItem>();
            if (organs == null || organs.Count() == 0)
                return model;
            model = organs.Select(s => new SelectListItem()
            {
                Value = s.ID.ToString(),
                Text = s.Name,
                Selected = list.Contains(s.ID.ToString())
            }).ToList();
            return model;
        }
        #endregion Common

        #region Private method
        private void UpdateData(VMCreateOrgan vmOrgan)
        {
            vmOrgan.Status = (int)EnumOrgan.Status.Active;
            //vmOrgan.ParentId = string.IsNullOrEmpty(vmOrgan.ParentIdStr) ? 0 : int.Parse(vmOrgan.ParentIdStr);
            //if (vmOrgan.IsArchiveStr == "true")
            //    vmOrgan.IsArchive = true;
            //else
            //    vmOrgan.IsArchive = false;
        }

        private void UpdateData(VMEditOrgan vmOrgan)
        {
            vmOrgan.Status = (int)EnumOrgan.Status.Active;
            //vmOrgan.ParentId = string.IsNullOrEmpty(vmOrgan.ParentIdStr) ? 0 : int.Parse(vmOrgan.ParentIdStr);
            //if (vmOrgan.IsArchiveStr == "true")
            //    vmOrgan.IsArchive = true;
            //else
            //    vmOrgan.IsArchive = false;
        }

        private bool IsExisted(Organ Organ)
        {
            if (Organ == null || Organ.ID == 0 || Organ.Status != (int)EnumOrgan.Status.Active)
                return false;
            return true;
        }

        private bool IsExisted<T>(IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0)
                return false;
            return true;
        }

        #endregion Private method

        #region Thêm dữ liệu mặc định sau khi tạo cơ quan
        private async Task InsertDefaultData(int idOrgan)
        {
            var cateTypes = new List<CategoryType>();
            var shelve = new CategoryType();
            var box = new CategoryType();
            #region Add CategoryType

            var language = await _dasRepo.CategoryType.InsertAsync(new CategoryType
            {
                IDOrgan = idOrgan,
                Code = EnumCategoryType.Code.DM_NgonNgu.ToString(),
                Name = "Ngôn ngữ",
                Status = (int)EnumCategoryType.Status.Active,
                ParentId = 0,
                IsConfig = true,
                ParentPath = "0"
            });
            var storage = await _dasRepo.CategoryType.InsertAsync(new CategoryType
            {
                IDOrgan = idOrgan,
                Code = EnumCategoryType.Code.DM_Kho.ToString(),
                Name = "Kho",
                Status = (int)EnumCategoryType.Status.Active,
                IsConfig = true,
                ParentId = 0,
                ParentPath = "0"
            });
            var profileCategory = await _dasRepo.CategoryType.InsertAsync(new CategoryType
            {
                IDOrgan = idOrgan,
                Code = EnumCategoryType.Code.DM_PhanLoaiHS.ToString(),
                Name = "Phân loại hồ sơ",
                Status = (int)EnumCategoryType.Status.Active,
                IsConfig = true,
                ParentId = 0,
                ParentPath = "0"
            });
            await _dasRepo.SaveAync();

            if (language.ID > 0)
            {
                cateTypes.Add(language);
            }
            if (storage.ID > 0)
            {
                cateTypes.Add(storage);
                //Thêm kho xong, thêm kệ
                shelve = await _dasRepo.CategoryType.InsertAsync(new CategoryType
                {
                    IDOrgan = idOrgan,
                    Code = EnumCategoryType.Code.DM_Gia.ToString(),
                    Name = "Giá/kệ",
                    Status = (int)EnumCategoryType.Status.Active,
                    IsConfig = true,
                    ParentId = storage.ID,
                    ParentPath = $"{storage.ParentPath}|{storage.ID}"
                });
                await _dasRepo.SaveAync();
                if (shelve.ID > 0)
                {
                    cateTypes.Add(shelve);
                    //Thêm giá xong, thêm hộp
                    box = await _dasRepo.CategoryType.InsertAsync(new CategoryType
                    {
                        IDOrgan = idOrgan,
                        Code = EnumCategoryType.Code.DM_HopSo.ToString(),
                        Name = "Hộp/cặp",
                        Status = (int)EnumCategoryType.Status.Active,
                        IsConfig = true,
                        ParentId = shelve.ID,
                        ParentPath = $"{shelve.ParentPath}|{shelve.ID}"
                    });
                    await _dasRepo.SaveAync();
                    if (box.ID > 0)
                    {
                        cateTypes.Add(box);
                    }
                }
            }

            //Add field
            if (storage.ID > 0)
            {
                await InsertCategoryTypeFields(cateTypes, idOrgan, storage);
            }
            if (language.ID > 0)
            {
                await InsertCategoryTypeFields(cateTypes, idOrgan, language);
            }
            if (profileCategory.ID > 0)
            {
                await InsertCategoryTypeFields(cateTypes, idOrgan, profileCategory);
            }
            if (shelve.ID > 0)
            {
                await InsertCategoryTypeFields(cateTypes, idOrgan, shelve);
            }
            if (box.ID > 0)
            {
                await InsertCategoryTypeFields(cateTypes, idOrgan, box);
            }
            await _dasRepo.SaveAync();

            #endregion Add DocType

            #region Add CategoryType
            var vbhc = await _dasRepo.DocType.InsertAsync(new DocType
            {
                IDOrgan = idOrgan,
                Code = "Doc",
                Name = "Văn bản hành chính",
                Status = (int)EnumDocType.Status.Active,
                Type = (int)EnumDocType.Type.Doc,
                IsBase = true
            });
            var photo = await _dasRepo.DocType.InsertAsync(new DocType
            {
                IDOrgan = idOrgan,
                Code = "Photo",
                Name = "Ảnh",
                Status = (int)EnumDocType.Status.Active,
                Type = (int)EnumDocType.Type.Photo,
                IsBase = true
            });
            var video = await _dasRepo.DocType.InsertAsync(new DocType
            {
                IDOrgan = idOrgan,
                Code = "Video",
                Name = "Video",
                Status = (int)EnumDocType.Status.Active,
                Type = (int)EnumDocType.Type.Video,
                IsBase = true
            });
            await _dasRepo.SaveAync();

            //Add fields
            if (vbhc.ID > 0)
            {
                await InsertDocTypeTypeFields(cateTypes, idOrgan, vbhc);
            }
            if (photo.ID > 0)
            {
                await InsertDocTypeTypeFields(cateTypes, idOrgan, photo);
            }
            if (video.ID > 0)
            {
                await InsertDocTypeTypeFields(cateTypes, idOrgan, video);
            }
            await _dasRepo.SaveAync();

            #endregion Add CategoryType


        }

        /// <summary>
        /// Add các trường cấu hình cho khung biên mục
        /// </summary>
        /// <param name="cateTypes"></param>
        /// <param name="idOrgan"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        public async Task InsertDocTypeTypeFields(List<CategoryType> cateTypes, int idOrgan, DocType docType)
        {
            var vmDocType = _mapper.Map<VMDocType>(docType);
            var storageFields = _defaultDataService.GetDefaultDocTypeFields(cateTypes, vmDocType, docType.Type);
            await _dasRepo.DocTypeField.InsertAsync(storageFields.Select(n => new DocTypeField
            {
                IDDocType = docType.ID,
                IDOrgan = idOrgan,

                Name = n.Name,
                Code = n.Code,
                InputType = n.InputType,
                Format = n.Format,
                IDCategoryTypeRelated = n.IDCategoryTypeRelated,
                IsRequire = n.IsRequire > 0,
                IsSearchGrid = n.IsSearchGrid > 0,
                IsShowGrid = n.IsShowGrid > 0,
                Maxlenght = n.Maxlenght,
                Minlenght = n.Minlenght,
                MaxValue = n.MaxValue,
                MinValue = n.MinValue,
                Priority = n.Priority.GetValueOrDefault(0),
                IsBase = true,
                Status = (int)EnumCommon.Status.Active,
            }));
        }

        /// <summary>
        /// Add các trường cấu hình cho danh mục động
        /// </summary>
        /// <param name="cateTypes"></param>
        /// <param name="idOrgan"></param>
        /// <param name="categoryType"></param>
        /// <returns></returns>
        public async Task InsertCategoryTypeFields(List<CategoryType> cateTypes, int idOrgan, CategoryType categoryType)
        {
            var fields = _defaultDataService.GetDefaultCategoryFields(cateTypes, categoryType.Name, categoryType.Code);
            await _dasRepo.CategoryTypeField.InsertAsync(fields.Select(n => new CategoryTypeField
            {
                IDCategoryType = categoryType.ID,
                IDOrgan = idOrgan,

                Name = n.Name,
                Code = n.Code,
                InputType = n.InputType,
                Format = n.Format,
                IDCategoryTypeRelated = n.IDCategoryTypeRelated,
                IsReadonly = n.IsReadonly > 0,
                IsRequire = n.IsRequire > 0,
                IsSearchGrid = n.IsSearchGrid > 0,
                IsShowGrid = n.IsShowGrid > 0,
                Maxlenght = n.Maxlenght,
                Minlenght = n.Minlenght,
                MaxValue = n.MaxValue,
                MinValue = n.MinValue,
                DefaultValueType = n.DefaultValueType,
                Priority = n.Priority.GetValueOrDefault(0),
                Status = (int)EnumCommon.Status.Active,
            }));
        }

        #endregion Thêm dữ liệu mặc định sau khi tạo cơ quan
    }
}
