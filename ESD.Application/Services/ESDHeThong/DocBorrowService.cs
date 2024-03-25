using AutoMapper;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Application.Constants;
using ESD.Utility;
using ESD.Infrastructure.ContextAccessors;
using ESD.Application.Enums;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Collections;
using ESD.Infrastructure.Migrations;
using ExpiryDate = ESD.Domain.Models.DAS.ExpiryDate;

namespace ESD.Application.Services
{
    /// <summary>
    /// Service khai thác
    /// </summary>
    public class DocBorrowService : BaseMasterService, IDocBorrowServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IAccountService _accountService;

        #endregion

        #region Ctor
        public DocBorrowService(IDasRepositoryWrapper dasRepository, IMapper mapper, ILoggerManager logger, IUserPrincipalService userPrincipalService, IAccountService accountService) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
            _accountService = accountService;
        }
        #endregion

        #region Reader

        #region Search
        /// <summary>
        /// Tìm kiếm tài liệu & hồ sơ
        /// </summary>
        /// <param name="condition">Điều kiện</param>
        /// <param name="borrowType">Loại (nội bộ/bên ngoài)</param>
        /// <returns></returns>
        public async Task<VMIndexDocBorrow> SearchByCondition(DocBorrowCondition condition, EnumBorrow.BorrowType borrowType)
        {
            var result = new VMIndexDocBorrow();
            condition.BorrowType = (int)borrowType;
            result.VMCatalogingDocs = await GetCatalogingDocs(condition);
            result.Condition = condition;
            return result;
        }
        public async Task<VMIndexDocBorrow> BorrowSearchByCondition(DocBorrowCondition condition, EnumBorrow.BorrowType borrowType)
        {
            var model = new VMIndexDocBorrow();
            condition.BorrowType = (int)borrowType;
            model.VMCatalogingBorrows = await GetCatalogingBorrows(condition);
            model.DictStatus = Utils.EnumToDic<EnumBorrow.Status>();
            model.Condition = condition;
            return model;
        }

        #endregion Search

        #region Detail 
        public async Task<VMIndexDocBorrow> DetailProfile(int idProfile, EnumBorrow.BorrowType borrowType)
        {
            var model = new VMIndexDocBorrow();
            var profile = await _dasRepo.CatalogingProfile.GetAsync(idProfile);
            if (profile.Status == (int)EnumCataloging.Status.InActive)
                return new VMIndexDocBorrow();

            model.VMCatalogingProfile = _mapper.Map<VMCatalogingProfile>(profile);
            model.VMCatalogingProfile.OrganName = (await _dasRepo.Organ.GetAsync(profile.IDOrgan) ?? new Organ()).Name;
            model.VMCatalogingProfile.ExpiryDateName = (await _dasRepo.ExpiryDate.FirstOrDefaultAsync(n => n.ID == profile.IDExpiryDate && n.Status != (int)EnumExpiryDate.Status.InActive) ?? new ExpiryDate()).Name;
            model.VMCatalogingProfile.Creator = (await _dasRepo.User.FirstOrDefaultAsync(n => n.ID == profile.CreatedBy && n.Status != (int)EnumCommon.Status.InActive) ?? new User()).Name;

            var idLanguages = Utils.Deserialize<List<string>>(profile.Language).ToListInt();
            if (idLanguages.IsNotEmpty() && idLanguages.Any(n => n > 0))
            {
                var langugage = await _dasRepo.Category.GetAllListAsync(n => idLanguages.Contains(n.ID) && n.Status != (int)EnumCategory.Status.InActive);
                if (langugage.IsNotEmpty())
                    model.VMCatalogingProfile.LanguageNames = string.Join(", ", langugage.Select(n => n.Name));
            }

            model.VMCatalogingDocs = await GetCatalogingDocs(new DocBorrowCondition()
            {
                IDProfile = idProfile,
                PageIndex = -1,
                BorrowType = (int)borrowType
            });
            model.CatalogingBorrowDocs = await GetWaitingCatalogingBorrowByReader(idProfile, -1, (int)borrowType);

            if (borrowType == EnumBorrow.BorrowType.User)
            {
                model.DictExpiryDate = await GetDictExpiryDate();
                model.DictAgencies = await GetDictAgencies();
                model.DictProfileTemplate = await GetDictProfileTemplate(profile.IDProfileTemplate);
                model.DictLanguage = await GetDictCategory(EnumCategoryType.Code.DM_NgonNgu.ToString());
                model.DictProfileCategory = await GetDictCategory(EnumCategoryType.Code.DM_PhanLoaiHS.ToString());
            }
            return model;
        }

        public async Task<VMCatalogingDocCreate> DetailDoc(int idDoc, EnumBorrow.BorrowType borrowType)
        {
            var temp = from doc in _dasRepo.CatalogingDoc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                             && doc.ID == idDoc
                       select _mapper.Map<VMCatalogingDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync() ?? new VMCatalogingDocCreate();
            model.VMDocTypes = await GetDocTypes();
            model.VMCatalogingProfile = await GetCatalogingProfile(model.IDCatalogingProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(new int[] { model.IDDocType });
            model.VMCatalogingDocFields = await GetDocFieldsByID(idDoc);
            model.DocFieldValues = model.VMDocTypeFields.ToDictionary(k => k.Code, v => model.VMCatalogingDocFields.FirstOrDefault(x => x.IDDocTypeField == v.ID)?.Value ?? "");

            var idLanguage = Utils.GetInt(model.DocFieldValues.GetValueOrDefault("Language"));
            if (idLanguage > 0)
            {
                var lang = await GetCategoryActive(idLanguage);
                if (lang != null)
                    model.LanguageName = lang.Name;
            }
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile) ?? new StgFile());
            model.CatalogingBorrowDocs = await GetWaitingCatalogingBorrowByReader(model.IDCatalogingProfile, model.ID, (int)borrowType); //ds đang mượn
            return model;
        }

        /// <summary>
        /// Chi tiết phiếu 
        /// </summary>
        /// <param name="id">ID phiếu</param>
        /// <param name="idReader">Độc giả -1 lấy tất cả</param>
        /// <returns></returns>
        public async Task<VMCatalogingBorrow> DetailBorrow(int id, int idReader)
        {
            var borrow = await _dasRepo.CatalogingBorrow.GetAsync(id);
            //nếu là manager ko lấy phiếu huỷ
            if (borrow == null || borrow.Status == (int)EnumBorrow.Status.InActive || (idReader == -1 && borrow.Status == (int)EnumBorrow.Status.Cancel))
                return null;


            var model = _mapper.Map<VMCatalogingBorrow>(borrow);
            if (model.ReaderType == (int)EnumBorrow.BorrowType.User)
                model.ReaderName = (await _dasRepo.User.GetAsync(model.IDReader) ?? new User()).Name;
            else
                model.ReaderName = (await _dasRepo.Reader.GetAsync(model.IDReader) ?? new Reader()).Name;
            var cond = new DocBorrowCondition()
            {
                IDCatalogingBorrow = id
            };
            if (idReader != -1)
                cond.IDReader = idReader;
            var docs = await GetCatalogingBorrowDocs(cond);
            model.CatalogingBorrowDocs = docs;
            return model;

        }
        #endregion Detail

        #region ViewFile

        public async Task<string> ViewFile(int id, EnumBorrow.BorrowType borrowType)
        {
            if (borrowType == EnumBorrow.BorrowType.Reader)
            {
                var user = await _accountService.GetCurrentReader(_userPrincipalService.UserId);
                if (user == null)
                    return null;
            }


            var doc = await _dasRepo.CatalogingDoc.FirstOrDefaultAsync(n => n.ID == id && n.Status != (int)EnumCommon.Status.InActive);
            if (doc == null)
                return null;
            var catalogingBorroweds = await GetApproveCatalogingBorrowByReader(doc.IDCatalogingProfile, doc.ID); //ds đang mượn
            var isViewFile = catalogingBorroweds.IsNotEmpty() && catalogingBorroweds.Any(n => n.IDDoc == doc.ID);
            if (isViewFile)
            {
                var vmStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(doc.IDFile) ?? new StgFile());
                return FileUltils.EncryptPathFile(vmStgFile.PhysicalPath, _userPrincipalService.UserId);
            }
            return null;
        }

        #endregion ViewFile

        #region Borrow

        public async Task<ServiceResult> RequestBorrow(HttpRequest request, HttpResponse response, VMUpdateCatalogingBorrow catalogingBorrow, EnumBorrow.BorrowType borrowType)
        {
            var startDate = Utils.GetDate(catalogingBorrow.StrStartDate);
            var endDate = Utils.GetDate(catalogingBorrow.StrEndDate);
            var borrow = _mapper.Map<CatalogingBorrow>(catalogingBorrow);
            if (borrowType == EnumBorrow.BorrowType.User)
            {
                if (borrow.ReaderType == (int)EnumBorrow.BorrowType.User)
                {
                    borrow.IDReader = catalogingBorrow.IDUser;
                }
                else
                {
                    borrow.IDReader = catalogingBorrow.IDReader;
                }
            }
            else
            {
                borrow.IDReader = _userPrincipalService.UserId;
                borrow.ReaderType = (int)borrowType;
            }
            if (borrow.IDReader <= 0)
                return new ServiceResultError("Chưa chọn độc giả", nameof(VMUpdateCatalogingBorrow.IDReader));

            //var lastBorrow = (await _dasRepo.CatalogingBorrow.GetAll().LastOrDefaultAsync() ?? new CatalogingBorrow());
            borrow.Code = "P";
            //Todo: có quyền duyệt
            //if (isApprover)
            //{
            //    borrow.Status = (int)EnumBorrow.Status.Approved;
            //    borrow.ApproveBy = _userPrincipalService.UserId;
            //    borrow.ApproveDate = DateTime.Now;
            //}

            if (startDate.HasValue)
            {
                borrow.StartDate = startDate.Value;
                if (borrow.StartDate == DateTime.MinValue)
                    return new ServiceResultError("Mượn từ ngày không hợp lệ", nameof(VMUpdateCatalogingBorrow.StrStartDate));
            }
            if (endDate.HasValue)
            {
                borrow.EndDate = endDate.Value;
                if (borrow.EndDate == DateTime.MinValue)
                    return new ServiceResultError("Đến ngày không hợp lệ", nameof(VMUpdateCatalogingBorrow.StrEndDate));
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                if (borrow.StartDate > borrow.EndDate)
                    return new ServiceResultError("Mượn từ ngày không được phép lớn hơn Đến ngày", nameof(VMUpdateCatalogingBorrow.StrStartDate));
            }
            if (catalogingBorrow.CatalogingBorrowDocs.IsEmpty())
                return new ServiceResultError("Phiếu không có dữ liệu, vui lòng kiểm tra lại");

            var idTempDocs = catalogingBorrow.CatalogingBorrowDocs.Select(n => n.IDDoc).ToList();

            var borrowingDocs = await GetBorrowingDocs(idTempDocs);
            if (borrowingDocs.Any(n => !n.IsReturned) && borrow.IsOriginal)
                return new ServiceResultError("Có tài liệu đang được mượn bản cứng, vui lòng không mượn bản cứng");

            //if (borrowingDocs.IsNotEmpty())
            //    return new ServiceResultError("Có tài liệu đang trong thời gian mượn, vui lòng kiểm tra lại");


            var newBorrows = new List<CatalogingBorrow>();
            var idOrgans = catalogingBorrow.CatalogingBorrowDocs.Select(n => n.IDOrgan).Distinct();

            foreach (var idOrgan in idOrgans)
            {
                var newBorrow = Utils.CopyTo<CatalogingBorrow>(borrow);
                newBorrow.IDOrgan = idOrgan;
                newBorrows.Add(newBorrow);
            }

            await _dasRepo.CatalogingBorrow.InsertAsync(newBorrows);
            await _dasRepo.SaveAync();

            var newBorrowDocs = catalogingBorrow.CatalogingBorrowDocs.Select(n => new CatalogingBorrowDoc
            {
                IDCatalogingBorrow = newBorrows.FirstOrNewObj(x => x.IDOrgan == n.IDOrgan).ID,
                IDDoc = n.IDDoc,
                IDProfile = n.IDProfile,
            }).ToList();

            foreach (var newBorrow in newBorrows)
            {
                newBorrow.Code = newBorrow.Code + newBorrow.ID;
            }
            await _dasRepo.CatalogingBorrowDoc.InsertAsync(newBorrowDocs);
            await _dasRepo.CatalogingBorrow.UpdateAsync(newBorrows);
            await _dasRepo.SaveAync();

            //Update cookie
            var idDocs = newBorrowDocs.Select(n => n.IDDoc);
            var cartValue = GetBorrowCart(request, borrowType);
            cartValue = cartValue.Where(n => !idDocs.Contains(n)).ToList();

            UpdateBorrowCart(response, cartValue, borrowType);

            //Thêm reader vào ReaderOrgan
            if (borrow.ReaderType == (int)EnumBorrow.BorrowType.Reader)
                await AddReaderInOrgans(idOrgans, borrow.IDReader);

            return new ServiceResultSuccess("Gửi yêu cầu thành công");

        }

        public async Task<ServiceResult> BorrowProfile(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType)
        {
            try
            {
                if (_userPrincipalService.UserId > 0)
                {
                    var profile = await _dasRepo.CatalogingProfile.FirstOrDefaultAsync(n => n.ID == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (profile == null)
                        return new ServiceResultError("Hồ sơ không còn tồn tại");

                    var docBorrows = await GetWaitingCatalogingBorrowByReader(id, -1, (int)borrowType); //ds đang mượn
                    var docs = await _dasRepo.CatalogingDoc.GetAllListAsync(n =>
                        n.IDCatalogingProfile == id && n.Status != (int)EnumCommon.Status.InActive);


                    if (docBorrows.IsNotEmpty() && docs.IsNotEmpty())
                    {
                        var idDocBorrows = docBorrows.Select(n => n.IDDoc);
                        docs = docs.Where(n => !idDocBorrows.Contains(n.ID)); //skip doc waiting borrow
                    }

                    if (Utils.IsEmpty(docs) && docBorrows.IsNotEmpty())
                        return new ServiceResultError("Bạn đã đăng ký mượn tất cả tài liệu trong hồ sơ");

                    if (Utils.IsEmpty(docs))
                        return new ServiceResultError("Hồ sơ không có tài liệu để mượn");



                    //var newBorrows = docs.Select(doc => new CatalogingBorrow
                    //{
                    //    IDReader = _userPrincipalService.UserId,
                    //    IDDoc = doc.ID,
                    //    IDProfile = doc.IDCatalogingProfile,
                    //    IDDocType = doc.IDDocType,
                    //    IDFile = doc.IDFile,
                    //    Status = (int)EnumBorrow.Status.WaitApprove
                    //}).ToList();

                    //await _dasRepo.CatalogingBorrow.InsertAsync(newBorrows);
                    //await _dasRepo.SaveAync();
                    var cartValues = AddToBorrowCart(request, response, docs.Select(n => n.ID).ToList(), borrowType);
                    return new ServiceResultSuccess("Đã thêm vào danh sách chờ mượn", new
                    {
                        cartValue = cartValues
                    });
                }
                else
                {
                    return new ServiceResultError("Vui lòng đăng nhập để mượn hồ sơ, đăng nhập ngay?", new
                    {
                        isComfirm = true,
                        redirectUrl = "/Account/Login?RequestPath=" + request.Headers["Referer"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }

        }
        public async Task<ServiceResult> BorrowDoc(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType)
        {
            try
            {
                if (_userPrincipalService.UserId > 0)
                {
                    var doc = await _dasRepo.CatalogingDoc.FirstOrDefaultAsync(n => n.ID == id && n.Status != (int)EnumCommon.Status.InActive);
                    if (doc == null)
                        return new ServiceResultError("Tài liệu không còn tồn tại");

                    var docBorrows = await GetWaitingCatalogingBorrowByReader(doc.IDCatalogingProfile, doc.ID, (int)borrowType); //ds đang mượn
                    if (docBorrows.IsNotEmpty())
                    {
                        if (docBorrows.Any(n => !n.IsReturned))
                            return new ServiceResultError("Tài liệu này đang được mượn bản cứng, không được phép mượn");

                        return new ServiceResultError("Bạn đã đăng ký mượn tài liệu này, vui lòng chờ phê duyệt");
                    }

                    //var newBorrows = new CatalogingBorrow
                    //{
                    //    IDReader = _userPrincipalService.UserId,
                    //    IDDoc = doc.ID,
                    //    IDProfile = doc.IDCatalogingProfile,
                    //    IDDocType = doc.IDDocType,
                    //    IDFile = doc.IDFile,
                    //    Status = (int)EnumBorrow.Status.WaitApprove
                    //};

                    //await _dasRepo.CatalogingBorrow.InsertAsync(newBorrows);
                    //await _dasRepo.SaveAync();

                    var cartValues = AddToBorrowCart(request, response, doc.ID, borrowType);
                    return new ServiceResultSuccess("Đã thêm vào danh sách chờ mượn", new
                    {
                        cartValue = cartValues
                    });
                }
                else
                {

                    return new ServiceResultError("Vui lòng đăng nhập để mượn tài liệu, đăng nhập ngay?", new
                    {
                        isComfirm = true,
                        redirectUrl = "/Account/Login?RequestPath=" + request.Headers["Referer"].ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError(ex.Message);
            }
        }
        /// <summary>  
        /// Get the cookie  
        /// </summary>  
        /// <param name="key">Key </param>  
        /// <returns>string value</returns>  
        public List<int> GetBorrowCart(HttpRequest request, EnumBorrow.BorrowType borrowType)
        {
            var v = request.Cookies[CommonConst.BorrowCart + borrowType.ToString() + _userPrincipalService.UserId];
            return (Utils.Deserialize<List<int>>(v) ?? new List<int>()).Select(n => n).Distinct().ToList();
        }

        public async Task<VMIndexBorrowCart> BorrowCartSearchByCondition(HttpRequest request, HttpResponse response, BorrowCartCondition condition, EnumBorrow.BorrowType borrowType)
        {
            var model = new VMIndexBorrowCart();
            model.Condition = condition;

            if (borrowType == EnumBorrow.BorrowType.Reader)
            {
                var user = await _accountService.GetCurrentReader(_userPrincipalService.UserId);
                if (user == null)
                    return model;

                model.VMUpdateCatalogingBorrow = new VMUpdateCatalogingBorrow()
                {
                    IDReader = user.ID,
                    ReaderType = (int)borrowType,
                    Reader = user.Name,
                };
            }
            else
            {
                model.VMUpdateCatalogingBorrow = new VMUpdateCatalogingBorrow()
                {
                    IDReader = _userPrincipalService.UserId,
                    ReaderType = (int)borrowType,
                    DictReader = await GetDictReader(),
                    DictUser = await GetDictUser()
                };
            }

            var idDocs = GetBorrowCart(request, borrowType);
            if (idDocs.IsNotEmpty())
            {
                //get list file
                string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
                string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
                string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
                var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                                join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                                join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID

                                where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                                       || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                                       || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                                select df;

                var temp = from d in _dasRepo.CatalogingDoc.GetAll().Where(n => n.Status != (int)EnumCommon.Status.InActive)
                           join p in _dasRepo.CatalogingProfile.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on d.IDCatalogingProfile equals p.ID
                           join o in _dasRepo.Organ.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on p.IDOrgan equals o.ID
                           join v in tempFiled on d.ID equals v.IDCatalogingDoc
                           where (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword) || p.Title.Contains(condition.Keyword))
                               && (p.ID == condition.IDProfile || condition.IDProfile == 0)
                               && idDocs.Contains(d.ID)
                           //&& p.IsPublic
                           group new { cd = d, cp = p, v } by new
                           {
                               d.ID,
                               d.IDDocType,
                               IDProfile = d.IDCatalogingProfile,
                               ProfileName = p.Title,
                               OrganID = o.ID,
                               OrganName = o.Name,
                               d.CreateDate
                           } into g
                           orderby g.Key.CreateDate descending
                           select new VMCatalogingBorrowDoc
                           {
                               ID = g.Key.ID,
                               IDDocType = g.Key.IDDocType,
                               IDProfile = g.Key.IDProfile,
                               IDDoc = g.Key.ID,
                               ProfileName = g.Key.ProfileName,
                               OrganID = g.Key.OrganID,
                               OrganName = g.Key.OrganName,
                           };

                var vmDocs = await temp.ToListAsync();
                idDocs = vmDocs.Select(x => x.ID).ToList();
                var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
                var docTypes = await GetDocTypes(idDocTypelist);
                var docTypeFields = await GetDocTypeFields(idDocTypelist);
                var catalogingDocField = await GetCatalogingDocFieldsByIDs(idDocs);

                var users = await _dasRepo.User.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active);


                foreach (var doc in vmDocs)
                {
                    doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                    doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                    doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                    //Dictionary
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault()?.Value ?? "");
                    doc.ApproveName = users.FirstOrNewObj(n => n.ID == doc.ApproveBy).Name;
                }

                //UpdateBorrowCart(response, idDocs);
                model.VMCatalogingBorrowDocs = new PaginatedList<VMCatalogingBorrowDoc>(vmDocs);
            }
            return model;
        }

        public ServiceResult RemoveCartItem(HttpRequest request, HttpResponse response, int id, EnumBorrow.BorrowType borrowType)
        {
            var cartValues = GetBorrowCart(request, borrowType);
            cartValues = cartValues.Where(n => n != id).ToList();
            UpdateBorrowCart(response, cartValues, borrowType);
            return new ServiceResultSuccess("Đã xoá tài liệu khỏi giỏ hồ sơ/tài liệu", new
            {
                cartValue = cartValues
            });
        }


        #endregion Borrow

        #region Cancel

        public async Task<ServiceResult> CancelBorrow(int id)
        {
            var borrow = await _dasRepo.CatalogingBorrow.GetAsync(id);
            if (borrow == null)
                return new ServiceResultError("Phiếu mượn không còn tồn tại!");

            if (borrow.Status != (int)EnumBorrow.Status.WaitApprove)
            {
                return new ServiceResultError($"Bạn không được phép huỷ duyệt phiếu này");
            }
            borrow.Status = (int)EnumBorrow.Status.Cancel;
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrow);
            await _dasRepo.SaveAync();

            //  //Tạo CatalogingDoc
            return new ServiceResultSuccess("Huỷ phiếu mượn thành công!");
        }

        #endregion Cancel

        #region ReBorrow

        public async Task<ServiceResult> ReBorrowDoc(int id)
        {
            var borrow = await _dasRepo.CatalogingBorrow.GetAsync(id);
            if (borrow == null)
                return new ServiceResultError("Phiếu mượn không còn tồn tại!");

            if (borrow.Status != (int)EnumBorrow.Status.Cancel)
            {
                return new ServiceResultError($"Bạn không được phép gửi yêu cầu mượn phiếu này");
            }
            borrow.Status = (int)EnumBorrow.Status.WaitApprove;
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrow);
            await _dasRepo.SaveAync();

            //  //Tạo CatalogingDoc
            return new ServiceResultSuccess("Gửi yêu cầu mượn thành công!");
        }

        #endregion ReBorrow

        #endregion Reader


        #region Manager
        #region Search
        public async Task<VMIndexDocBorrow> ManagerBorrowSearchByCondition(DocBorrowCondition condition)
        {
            var model = new VMIndexDocBorrow();
            model.VMCatalogingBorrows = await GetManagerCatalogingBorrows(condition);
            model.DictStatus = Utils.EnumToDic<EnumBorrow.Status>().Where(n => n.Key != (int)EnumBorrow.Status.Cancel).ToDictionary(n => n.Key, n => n.Value);
            model.DictReader = await GetDictReader();
            model.DictUser = await GetDictUser();
            model.DictBorrowType = Utils.EnumToDic<EnumBorrow.BorrowType>();
            model.Condition = condition;
            return model;
        }
        #endregion Search
        #region Approve

        public async Task<VMCatalogingBorrow> GetBorrows(int[] ids)
        {
            var borrows = await _dasRepo.CatalogingBorrow.GetAllListAsync(n => ids.Contains(n.ID) && n.Status != (int)EnumBorrow.Status.InActive && n.Status != (int)EnumBorrow.Status.Cancel);
            if (borrows.IsEmpty())
            {
                return null;
            }

            if (borrows.Count() == 1)
            {
                var model = _mapper.Map<VMCatalogingBorrow>(borrows.First());
                model.IDs = new[] { model.ID };
                return model;
            }
            else
            {
                // > 1
                var model = new VMCatalogingBorrow();
                model.IDs = borrows.Select(n => n.ID).ToArray();
                return model;
            }
        }

        public async Task<ServiceResult> Approve(VMUpdateCatalogingBorrow vmBorrow)
        {
            var borrow = await _dasRepo.CatalogingBorrow.GetAsync(vmBorrow.ID);
            if (borrow == null || borrow.Status == (int)EnumBorrow.Status.InActive || borrow.Status == (int)EnumBorrow.Status.Cancel)
                return new ServiceResultError("Phiếu mượn không còn tồn tại");



            var startDate = Utils.GetDate(vmBorrow.StrStartDate);
            var endDate = Utils.GetDate(vmBorrow.StrEndDate);

            var errObj = new List<object>();
            if (!startDate.HasValue)
                errObj.Add(new
                {
                    Field = $"StartDate",
                    Mss = "Mượn từ ngày không được để trống"
                });
            if (!endDate.HasValue)
                errObj.Add(new
                {
                    Field = $"EndDate",
                    Mss = "Đến ngày không được để trống"
                });

            if (errObj.IsNotEmpty())
                return new ServiceResultError("Giá trị nhập vào không hợp lệ!", errObj);

            if (startDate.Value > endDate.Value)
                return new ServiceResultError("Mượn từ ngày không dược lớn hơn Đến ngày!", "StartDate");


            if (borrow.Status != (int)EnumBorrow.Status.WaitApprove)
            {
                return new ServiceResultError($"Phiếu {borrow.Code} không được phép duỵệt, vui lòng kiểm tra lại");
            }


            //Check 
            var idDocs = await _dasRepo.CatalogingBorrowDoc.GetAll().Where(n => n.IDCatalogingBorrow == vmBorrow.ID && n.Status != (int)EnumBorrow.Status.InActive).Select(n => n.IDDoc).ToListAsync();

            var borrowingDocs = await GetBorrowingDocs(idDocs);
            if (borrowingDocs.Any(n => !n.IsReturned))
                return new ServiceResultError("Trong phiếu có tài liệu đang được mượn bản cứng, vui lòng kiểm tra lại");

            //if (borrowingDocs.IsNotEmpty())
            //    return new ServiceResultError("Trong phiếu có tài liệu đang trong thời gian mượn, vui lòng kiểm tra lại");


            borrow.Status = (int)EnumBorrow.Status.Approved;
            borrow.ApproveBy = _userPrincipalService.UserId;
            borrow.StartDate = startDate.Value;
            borrow.EndDate = endDate.Value;
            borrow.ApproveDate = DateTime.Now;
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrow);
            await _dasRepo.SaveAync();
            return new ServiceResultSuccess("Duyệt phiếu mượn thành công!");
        }
        public async Task<ServiceResult> Approves(VMUpdateCatalogingBorrow vmBorrow)
        {
            if (vmBorrow.IDs.IsEmpty())
            {
                return new ServiceResultError("Vui lòng chọn phiếu cần duyệt");
            }
            var borrows = await _dasRepo.CatalogingBorrow.GetAllListAsync(n => vmBorrow.IDs.Contains(n.ID) && n.Status != (int)EnumBorrow.Status.Cancel && n.Status != (int)EnumBorrow.Status.InActive);
            if (borrows.IsEmpty())
                return new ServiceResultError("Phiếu mượn không còn tồn tại");


            var startDate = Utils.GetDate(vmBorrow.StrStartDate);
            var endDate = Utils.GetDate(vmBorrow.StrEndDate);

            var errObj = new List<object>();
            if (!startDate.HasValue)
                errObj.Add(new
                {
                    Field = $"StartDate",
                    Mss = "Mượn từ ngày không được để trống"
                });
            if (!endDate.HasValue)
                errObj.Add(new
                {
                    Field = $"EndDate",
                    Mss = "Đến ngày không được để trống"
                });

            if (errObj.IsNotEmpty())
                return new ServiceResultError("Giá trị nhập vào không hợp lệ!", errObj);

            if (startDate.Value > endDate.Value)
                return new ServiceResultError("Mượn từ ngày không dược lớn hơn Đến ngày!", "StartDate");

            if (borrows.Any(n => n.Status != (int)EnumBorrow.Status.WaitApprove))
            {
                var names = borrows.Where(n => n.Status != (int)EnumBorrow.Status.WaitApprove).Select(n => n.Code);
                return new ServiceResultError($"Phiếu {string.Join(", ", names)} không được phép duỵệt, vui lòng kiểm tra lại");
            }

            //Check tài liệu đang mượn hoặc bị mượn bản cứng
            var idDocs = await _dasRepo.CatalogingBorrowDoc.GetAll().Where(n => n.IDCatalogingBorrow == vmBorrow.ID && n.Status != (int)EnumBorrow.Status.InActive).Select(n => n.IDDoc).ToListAsync();
            var idReaders = borrows.Select(n => n.IDReader).Distinct().ToList();

            var borrowingDocs = await GetBorrowingDocs(idDocs);
            if (borrowingDocs.Any(n => !n.IsReturned))
                return new ServiceResultError("Có phiếu chứa tài liệu đang được mượn bản cứng, vui lòng kiểm tra lại");

            //if (borrowingDocs.IsNotEmpty())
            //    return new ServiceResultError("Có phiếu chứa tài liệu đang trong thời gian mượn, vui lòng kiểm tra lại");


            foreach (var borrow in borrows)
            {
                borrow.Status = (int)EnumBorrow.Status.Approved;
                borrow.ApproveBy = _userPrincipalService.UserId;
                borrow.StartDate = startDate.Value;
                borrow.EndDate = endDate.Value;
                borrow.ApproveDate = DateTime.Now;
            }
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrows);
            await _dasRepo.SaveAync();

            return new ServiceResultSuccess("Duyệt phiếu mượn thành công!");
        }
        public async Task<ServiceResult> Rejects(int[] ids, string note)
        {
            if (ids.IsEmpty())
                return new ServiceResultError("Vui lòng chọn phiếu duyệt!");

            var borrows = await _dasRepo.CatalogingBorrow.GetAllListAsync(n => ids.Contains(n.ID) && n.Status != (int)EnumBorrow.Status.InActive && n.Status != (int)EnumBorrow.Status.Cancel);
            if (borrows.IsEmpty())
                return new ServiceResultError("Phiếu mượn không còn tồn tại");

            if (borrows.Any(n => n.Status != (int)EnumBorrow.Status.WaitApprove))
            {
                return new ServiceResultError($"Có {borrows.Count(n => n.Status != (int)EnumBorrow.Status.WaitApprove)} phiếu không được phép từ chối duyệt, vui lòng kiểm tra lại");
            }
            foreach (var borrow in borrows)
            {
                borrow.Status = (int)EnumBorrow.Status.Reject;
                borrow.ApproveDate = DateTime.Now;
                borrow.ApproveBy = _userPrincipalService.UserId;
                borrow.ReasonToReject = note;
            }
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrows);
            await _dasRepo.SaveAync();

            //  //Tạo CatalogingDoc
            return new ServiceResultSuccess("Từ chối duyệt phiếu mượn thành công!");
        }

        public async Task<ServiceResult> Returns(int[] ids)
        {
            if (ids.IsEmpty())
                return new ServiceResultError("Vui lòng chọn phiếu!");

            var borrows = await _dasRepo.CatalogingBorrow.GetAllListAsync(n => ids.Contains(n.ID) && n.Status != (int)EnumBorrow.Status.InActive && n.Status != (int)EnumBorrow.Status.Cancel);
            if (borrows.IsEmpty())
                return new ServiceResultError("Phiếu mượn không còn tồn tại");

            var count = 0;
            if (borrows.Any(n => !n.IsOriginal))
            {
                count = borrows.Count(n => !n.IsOriginal);
                if (count > 1)
                    return new ServiceResultError($"Có {count} phiếu không mượn bản cứng");
                return new ServiceResultError("Phiếu không mượn bản cứng");

            }
            if (borrows.Any(n => n.IsReturned))
            {
                count = borrows.Count(n => n.IsReturned);
                if (count > 1)
                    return new ServiceResultError($"Có {count} phiếu đã trả bản cứng");
                return new ServiceResultError($"Phiếu đã trả bản cứng");
            }
            foreach (var borrow in borrows)
            {
                borrow.IsReturned = true;
                borrow.ReturnDate = DateTime.Now;
            }
            await _dasRepo.CatalogingBorrow.UpdateAsync(borrows);
            await _dasRepo.SaveAync();

            //  //Tạo CatalogingDoc
            return new ServiceResultSuccess("Trả bản cứng thành công!");
        }
        #endregion Approve
        #region DetailDoc

        public async Task<VMCatalogingDocCreate> GetDocCollect(int IDBorrow, int IDDoc)
        {
            var temp = from doc in _dasRepo.CatalogingDoc.GetAll()
                       where doc.Status != (int)EnumDocCollect.Status.InActive
                       && doc.ID == IDDoc
                       select _mapper.Map<VMCatalogingDocCreate>(doc);
            var model = await temp.FirstOrDefaultAsync();
            model.VMDocTypes = await GetDocTypes();
            model.VMStgFile = _mapper.Map<VMStgFile>(await _dasRepo.StgFile.GetAsync(model.IDFile));
            model.VMCatalogingProfile = await GetCatalogingProfile(model.IDCatalogingProfile);
            model.VMDocType = model.VMDocTypes.FirstOrNewObj(n => n.ID == model.IDDocType);
            model.VMDocTypeFields = await GetDocTypeFields(model.IDDocType);
            model.VMCatalogingDocFields = await GetDocFieldsByID(IDDoc);

            foreach (var item in model.VMCatalogingDocFields)
            {
                var field = model.VMDocTypeFields.FirstOrDefault(n => n.ID == item.IDDocTypeField);
                if (field == null)
                    continue;
                switch (field.Code)
                {
                    case "FileCode":
                    case "Identifier":
                    case "Organld":
                    case "FileCatalog":
                    case "FileNotation":
                    case "OrganName":
                        item.IsReadonly = true;
                        break;
                }
            }
            return model;
        }

        #endregion DetailDoc

        #endregion Manager


        #region Advance Search


        public async Task<VMIndexAdvanceSearch> AdvanceSearch(AdvanceSearchCondition condition)
        {
            var model = new VMIndexAdvanceSearch();
            if (condition.ReaderType == (int)EnumBorrow.BorrowType.User)
            {
                model.VMDocTypes = await GetDocTypes(_userPrincipalService.IDOrgan);
                var firstDocType = model.VMDocTypes.FirstOrNewObj();
                model.SearchFields = await GetSearchFieldsByIDDocType(firstDocType.ID);
            }
            else
            {
                //Loai khung
                model.VMDocTypes = Utils.EnumToDic<EnumDocType.Type>().Select(n => new VMDocType
                {
                    ID = n.Key,
                    Name = n.Value
                }).ToList();
                var firsType = model.VMDocTypes.FirstOrNewObj();
                model.SearchFields = await GetSearchFieldsByType(firsType.ID);
            }
            model.VMCatalogingDocs = await GetCatalogingDocs(condition);
            model.Condition = condition ?? new AdvanceSearchCondition();
            return model;
        }

        /// <summary>
        /// Lấy ra các trường meta để tìm kiếm
        /// </summary>
        /// <param name="idDocType">ID khung bm</param>
        /// <returns></returns>
        public async Task<List<VMDocTypeField>> GetSearchFieldsByIDDocType(int idDocType)
        {
            var temp = from g in _dasRepo.DocTypeField.GetAll()
                       where g.IsBase && g.Status == (int)EnumDocType.Status.Active
                             && (g.InputType == (int)EnumDocType.InputType.InpText || g.InputType == (int)EnumDocType.InputType.InpTextArea
                                                                                     || g.InputType == (int)EnumDocType.InputType.InpNumber || g.InputType == (int)EnumDocType.InputType.InpFloat || g.InputType == (int)EnumDocType.InputType.InpMoney || g.InputType == (int)EnumDocType.InputType.InpDate
                             )
                             && g.IDDocType == idDocType
                       orderby g.Name
                       select new VMDocTypeField
                       {
                           ID = g.ID,
                           Name = g.Name,
                           Code = g.Code,
                           InputType = g.InputType,
                       };

            return await temp.ToListAsync();
        }

        /// <summary>
        /// Lấy ra các trường meta để tìm kiếm
        /// </summary>
        /// <param name="type">Loại khung bm</param>
        public async Task<List<VMDocTypeField>> GetSearchFieldsByType(int type)
        {
            //Get all > lấy theo
            var temp = from dt in _dasRepo.DocType.GetAll()
                       join dtf in _dasRepo.DocTypeField.GetAll() on dt.ID equals dtf.IDDocType
                       where dtf.IsBase && dt.IsBase && dt.Status == (int)EnumDocType.Status.Active &&
                             dtf.Status == (int)EnumDocType.Status.Active
                             && (dtf.InputType == (int)EnumDocType.InputType.InpText || dtf.InputType == (int)EnumDocType.InputType.InpTextArea
                                                                                     || dtf.InputType == (int)EnumDocType.InputType.InpNumber || dtf.InputType == (int)EnumDocType.InputType.InpFloat || dtf.InputType == (int)EnumDocType.InputType.InpMoney || dtf.InputType == (int)EnumDocType.InputType.InpDate
                             )
                             && dt.Type == type
                       group new { dt, dtf } by new { dtf.ID, dtf.Name, dtf.Code, dt.Type, dtf.InputType }
                into g

                       orderby g.Key.Name
                       select new VMDocTypeField
                       {
                           ID = g.Key.ID,
                           Name = g.Key.Name,
                           Code = g.Key.Code,
                           TypeOfDocType = g.Key.Type,
                           InputType = g.Key.InputType,
                       };

            var rs = await temp.ToListAsync();

            return rs.GroupBy(g => new { g.Name, g.Code }).Select(n => new VMDocTypeField
            {
                ID = n.First().ID,
                Name = n.Key.Name,
                Code = n.Key.Code,
                TypeOfDocType = n.First().TypeOfDocType,
                InputType = n.First().InputType,
            }).ToList();

        }
        #endregion Advance Search

        #region Private methods
        private async Task<Category> GetCategoryActive(int id)
        {
            return await _dasRepo.Category.FirstOrDefaultAsync(n => n.ID != id && n.Status == (int)EnumCategory.Status.Active);

        }

        private async Task<PaginatedList<VMCatalogingDoc>> GetCatalogingDocs(DocBorrowCondition condition)
        {
            //get list file
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID
                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from d in _dasRepo.CatalogingDoc.GetAll()
                       join p in _dasRepo.CatalogingProfile.GetAll() on d.IDCatalogingProfile equals p.ID
                       join v in tempFiled on d.ID equals v.IDCatalogingDoc
                       where d.Status == (int)EnumDocCollect.Status.Complete
                       && p.Status == (int)EnumCataloging.Status.StorageApproved
                       && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword) || p.Title.Contains(condition.Keyword))
                           && (d.IDCatalogingProfile == condition.IDProfile || condition.IDProfile == 0)

                       && (condition.BorrowType == (int)EnumBorrow.BorrowType.Reader || (condition.BorrowType == (int)EnumBorrow.BorrowType.User && p.IDOrgan == _userPrincipalService.IDOrgan)) //Theo cq khi mượn nội bộ
                       group new { d, p, v } by new
                       {
                           d.ID,
                           d.IDChannel,
                           d.IDFile,
                           d.IDCatalogingProfile,
                           d.IDDocType,
                           p.FileCode,
                           p.Title
                       } into g
                       orderby g.Key.IDCatalogingProfile descending, g.Key.ID descending
                       select new VMCatalogingDoc
                       {
                           ID = g.Key.ID,
                           IDChannel = g.Key.IDChannel,
                           IDFile = g.Key.IDFile,
                           IDCatalogingProfile = g.Key.IDCatalogingProfile,
                           IDDocType = g.Key.IDDocType,
                           ProfileCode = g.Key.FileCode,
                           ProfileName = g.Key.Title
                       };

            if (condition.PageIndex == -1)
            {
                //nopaging
                var vmDocs = await temp.ToListAsync();
                var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
                var docTypes = await GetDocTypes(idDocTypelist);
                var docTypeFields = await GetDocTypeFields(idDocTypelist);
                var catalogingDocField = await GetCatalogingDocFieldsByIDs(vmDocs.Select(x => x.ID));
                foreach (var doc in vmDocs)
                {
                    doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                    doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                    doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                    //Dictionary
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                return new PaginatedList<VMCatalogingDoc>(vmDocs, vmDocs.Count, 1, vmDocs.Count);
            }
            else
            {
                var totalFile = await temp.LongCountAsync();
                var totalPage = (int)Math.Ceiling(totalFile / (double)condition.PageSize);
                if (totalPage < condition.PageIndex)
                    condition.PageIndex = 1;
                var vmDocs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

                var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
                var docTypes = await GetDocTypes(idDocTypelist);
                var docTypeFields = await GetDocTypeFields(idDocTypelist);
                var catalogingDocField = await GetCatalogingDocFieldsByIDs(vmDocs.Select(x => x.ID));
                foreach (var doc in vmDocs)
                {
                    doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                    doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                    doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                    //Dictionary
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                return new PaginatedList<VMCatalogingDoc>(vmDocs, (int)totalFile, condition.PageIndex, condition.PageSize);
            }
        }
        private async Task<List<VMDocType>> GetDocTypes(IEnumerable<int> IDDocTypes)
        {
            var temp = from dc in _dasRepo.DocType.GetAll()
                       where dc.Status != (int)EnumCommon.Status.InActive
                       && IDDocTypes.Contains(dc.ID)
                       select _mapper.Map<VMDocType>(dc);

            return await temp.ToListAsync();
        }
        private async Task<List<VMDocTypeField>> GetDocTypeFields(IEnumerable<int> idDoctypes)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                       && idDoctypes.Contains(dtf.IDDocType)
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }
        private async Task<List<VMCatalogingDocField>> GetCatalogingDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                             && IDDocs.Contains(df.IDCatalogingDoc)
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }
        private async Task<List<VMDocType>> GetDocTypes(int idOrgan = -1)
        {
            var temp = from dt in _dasRepo.DocType.GetAll()
                       where dt.Status != (int)EnumCommon.Status.InActive && (idOrgan == -1 || idOrgan == dt.IDOrgan)
                       select _mapper.Map<VMDocType>(dt);

            return await temp.ToListAsync();
        }
        private async Task<VMCatalogingProfile> GetCatalogingProfile(int idProfile)
        {
            var pp = (await _dasRepo.CatalogingProfile.GetAsync(idProfile)) ?? new CatalogingProfile();
            var plan = (await _dasRepo.Plan.GetAsync(pp.IDPlan)) ?? new Plan();
            var rs = _mapper.Map<VMCatalogingProfile>(pp);
            rs.PlanName = plan.Name;
            rs.OrganName = (await _dasRepo.Organ.GetAsync(pp.IDOrgan) ?? new Organ()).Name;
            return rs;
        }
        private async Task<List<VMCatalogingDocField>> GetDocFieldsByID(int IDDoc)
        {
            var temp = from df in _dasRepo.CatalogingDocField.GetAll()
                       where df.Status != (int)EnumCommon.Status.InActive
                             && df.IDCatalogingDoc == IDDoc
                       select _mapper.Map<VMCatalogingDocField>(df);
            return await temp.ToListAsync();
        }


        /// <summary>
        /// Lấy ds đang chờ duyệt theo Reader
        /// </summary>
        /// <param name="idProfile">-1 to skip</param>
        /// <param name="idDoc">-1 to skip</param>
        /// <returns></returns>
        private async Task<PaginatedList<VMCatalogingBorrowDoc>> GetWaitingCatalogingBorrowByReader(int idProfile, int idDoc, int type)
        {
            var idReader = _userPrincipalService.UserId;
            if (idReader <= 0)
                return new PaginatedList<VMCatalogingBorrowDoc>();

            var temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join brd in _dasRepo.CatalogingBorrowDoc.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on br.ID equals brd.IDCatalogingBorrow
                       join p in _dasRepo.CatalogingProfile.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on brd.IDProfile equals p.ID

                       where (br.Status == (int)EnumBorrow.Status.WaitApprove || (br.Status == (int)EnumBorrow.Status.Approved && br.StartDate.Date <= DateTime.Now.Date && DateTime.Now.Date <= br.EndDate.Date))
                             && ((br.IDReader == idReader && br.ReaderType == type) || idReader == -1)
                           && (brd.IDProfile == idProfile || idProfile == -1)
                           && (brd.IDDoc == idDoc || idDoc == -1)

                       select new VMCatalogingBorrowDoc
                       {
                           ID = brd.ID,
                           IDProfile = brd.IDProfile,
                           IDReader = br.IDReader,
                           IDDoc = brd.IDDoc,
                           IsReturned = br.IsReturned,
                           IDCatalogingBorrow = brd.IDCatalogingBorrow
                       };
            //br.Status == (int)EnumBorrow.Status.Approved && br.IsOriginal && !br.IsReturned phiêu mượn file gốc chưa trả
            var rs = await temp.ToListAsync();

            return new PaginatedList<VMCatalogingBorrowDoc>(rs, rs.Count, 1, rs.Count);
        }
        /// <summary>
        /// Lấy ds đã duyệt theo Reader
        /// </summary>
        /// <param name="idProfile">-1 to skip</param>
        /// <param name="idDoc">-1 to skip</param>
        /// <returns></returns>
        private async Task<IEnumerable<VMCatalogingBorrowDoc>> GetApproveCatalogingBorrowByReader(int idProfile, int idDoc)
        {
            var idReader = _userPrincipalService.UserId;
            if (idReader <= 0)
                return new List<VMCatalogingBorrowDoc>();

            var temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join brd in _dasRepo.CatalogingBorrowDoc.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on br.ID equals brd.IDCatalogingBorrow
                       join p in _dasRepo.CatalogingProfile.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on brd.IDProfile equals p.ID

                       where (br.StartDate.Date <= DateTime.Now.Date && br.EndDate.Date >= DateTime.Now.Date && br.Status == (int)EnumBorrow.Status.Approved) &&
                      (br.IDReader == idReader || idReader == -1) && (brd.IDProfile == idProfile || idProfile == -1) && (brd.IDDoc == idDoc || idDoc == -1)
                       select new VMCatalogingBorrowDoc
                       {
                           IDProfile = brd.IDProfile,
                           IDDoc = brd.IDDoc,
                           IDCatalogingBorrow = brd.IDCatalogingBorrow
                       };

            var rs = await temp.ToListAsync();
            return rs;
        }


        /// <summary>
        /// Lấy ds tài liệu theo phiếu
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMCatalogingBorrowDoc>> GetCatalogingBorrowDocs(DocBorrowCondition condition)
        {
            var fromCreateDate = Utils.GetDate(condition.FromCreateDate);
            var toCreateDate = Utils.GetDate(condition.ToCreateDate);

            //get list file
            string[] arrCodeType1 = { "DocCode", "TypeName", "Subject" };   // EnumDocType.Type.Doc
            string[] arrCodeType2 = { "Identifier" };   // EnumDocType.Type.Photo
            string[] arrCodeType3 = { "Identifier" };   // EnumDocType.Type.Video
            var tempFiled = from df in _dasRepo.CatalogingDocField.GetAll()
                            join dtf in _dasRepo.DocTypeField.GetAll() on df.IDDocTypeField equals dtf.ID
                            join dt in _dasRepo.DocType.GetAll() on dtf.IDDocType equals dt.ID

                            where ((dt.Type == (int)EnumDocType.Type.Doc && arrCodeType1.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Photo && arrCodeType2.Contains(dtf.Code))
                            || (dt.Type == (int)EnumDocType.Type.Video && arrCodeType3.Contains(dtf.Code)))
                            select df;

            var temp = from brd in _dasRepo.CatalogingBorrowDoc.GetAll()
                       join br in _dasRepo.CatalogingBorrow.GetAll() on brd.IDCatalogingBorrow equals br.ID
                       join p in _dasRepo.CatalogingProfile.GetAll() on brd.IDProfile equals p.ID
                       join d in _dasRepo.CatalogingDoc.GetAll() on brd.IDDoc equals d.ID
                       join v in tempFiled on d.ID equals v.IDCatalogingDoc
                       where
                           (!fromCreateDate.HasValue || (fromCreateDate.HasValue && fromCreateDate.Value <= brd.CreateDate.Value.Date)) && (!toCreateDate.HasValue || (toCreateDate.HasValue && toCreateDate.Value >= brd.CreateDate.Value.Date))

                      && (condition.IDCatalogingBorrow == br.ID || condition.IDCatalogingBorrow == 0)
                      && (condition.IDReader == br.IDReader || condition.IDReader == 0)
                        && (condition.Status == brd.Status || condition.Status.GetValueOrDefault(0) <= 0)
                        && (br.Status != (int)EnumBorrow.Status.InActive)
                           && (condition.Keyword.IsEmpty() || v.Value.Contains(condition.Keyword) || p.Title.Contains(condition.Keyword))
                                   && (brd.IDProfile == condition.IDProfile || condition.IDProfile == 0)

                       //&& p.IsPublic
                       group new { brd, p, v, d } by new
                       {
                           brd.ID,
                           brd.IDDoc,
                           d.IDDocType,
                           brd.CreateDate,
                           brd.IDProfile,
                           brd.Status,
                           p.FileCode,
                           p.Title
                       } into g
                       orderby g.Key.CreateDate descending
                       select new VMCatalogingBorrowDoc
                       {
                           ID = g.Key.ID,
                           IDDoc = g.Key.IDDoc,
                           CreateDate = g.Key.CreateDate,
                           Status = g.Key.Status,
                           IDProfile = g.Key.IDProfile,
                           IDDocType = g.Key.IDDocType,
                           ProfileCode = g.Key.FileCode,
                           ProfileName = g.Key.Title
                       };


            var totalFile = await temp.LongCountAsync();
            var totalPage = (int)Math.Ceiling(totalFile / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var vmDocs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var catalogingDocField = await GetCatalogingDocFieldsByIDs(vmDocs.Select(x => x.IDDoc));

            var users = await _dasRepo.User.GetAllListAsync(n => n.Status == (int)EnumCommon.Status.Active);
            foreach (var doc in vmDocs)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.IDDoc).ToList();
                //Dictionary
                doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault()?.Value ?? "");

                doc.ApproveName = users.FirstOrNewObj(n => n.ID == doc.ApproveBy).Name;
            }
            return new PaginatedList<VMCatalogingBorrowDoc>(vmDocs, (int)totalFile, condition.PageIndex, condition.PageSize);
        }

        /// <summary>
        /// Lấy danh sách phiếu mươn (độc giả)
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMCatalogingBorrow>> GetCatalogingBorrows(DocBorrowCondition condition)
        {
            var fromCreateDate = Utils.GetDate(condition.FromCreateDate);
            var toCreateDate = Utils.GetDate(condition.ToCreateDate);


            IQueryable<VMCatalogingBorrow> temp;
            if (condition.BorrowType == (int)EnumBorrow.BorrowType.Reader)

            {
                temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join o in _dasRepo.Organ.GetAll() on br.IDOrgan equals o.ID
                       join r in _dasRepo.Reader.GetAll() on br.IDReader equals r.ID
                       where (condition.IDReader == br.IDReader)
                             && (condition.Keyword.IsEmpty() || br.Code.Contains(condition.Keyword))
                             && (!fromCreateDate.HasValue || fromCreateDate.Value <= br.CreateDate.Value.Date) && (!toCreateDate.HasValue || toCreateDate.Value >= br.CreateDate.Value.Date)
                             && (condition.Status == br.Status || condition.Status.GetValueOrDefault(0) <= 0)
                             && (br.Status == (int)EnumBorrow.Status.Cancel || br.Status == (int)EnumBorrow.Status.WaitApprove || br.Status == (int)EnumBorrow.Status.Reject || br.Status == (int)EnumBorrow.Status.Approved)
                             && condition.BorrowType == br.ReaderType
                       orderby br.ID descending
                       select new VMCatalogingBorrow
                       {
                           ID = br.ID,
                           IDReader = br.IDReader,
                           Code = br.Code,
                           Status = br.Status,
                           OrganName = o.Name,
                           CreateDate = br.CreateDate,
                           StartDate = br.StartDate,
                           EndDate = br.EndDate,
                           ReaderName = r.Name,
                           ReasonToReject = br.ReasonToReject,
                           IsReturned = br.IsReturned,
                           IsOriginal = br.IsOriginal,
                           ReturnDate = br.ReturnDate
                       };
            }
            else
            {
                temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join o in _dasRepo.Organ.GetAll() on br.IDOrgan equals o.ID
                       join r in _dasRepo.User.GetAll() on br.IDReader equals r.ID
                       where (condition.IDReader == br.IDReader)
                             && (condition.Keyword.IsEmpty() || br.Code.Contains(condition.Keyword))
                             && (!fromCreateDate.HasValue || fromCreateDate.Value <= br.CreateDate.Value.Date) && (!toCreateDate.HasValue || toCreateDate.Value >= br.CreateDate.Value.Date)
                             && (condition.Status == br.Status || condition.Status.GetValueOrDefault(0) <= 0)
                             && (br.Status == (int)EnumBorrow.Status.Cancel || br.Status == (int)EnumBorrow.Status.WaitApprove || br.Status == (int)EnumBorrow.Status.Reject || br.Status == (int)EnumBorrow.Status.Approved)
                             && condition.BorrowType == br.ReaderType
                       orderby br.ID descending
                       select new VMCatalogingBorrow
                       {
                           ID = br.ID,
                           IDReader = br.IDReader,
                           Code = br.Code,
                           Status = br.Status,
                           OrganName = o.Name,
                           CreateDate = br.CreateDate,
                           StartDate = br.StartDate,
                           EndDate = br.EndDate,
                           ReaderName = r.Name,
                           ReasonToReject = br.ReasonToReject,
                           IsReturned = br.IsReturned,
                           IsOriginal = br.IsOriginal,
                           ReturnDate = br.ReturnDate
                       };

            }


            var totalFile = await temp.LongCountAsync();
            var totalPage = (int)Math.Ceiling(totalFile / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var vmBorrows = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).OrderByDescending(n => n.ID).ToListAsync();

            return new PaginatedList<VMCatalogingBorrow>(vmBorrows, (int)totalFile, condition.PageIndex, condition.PageSize);
        }

        /// <summary>
        /// Lấy ds phiếu mượn (người duyệt)
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private async Task<PaginatedList<VMCatalogingBorrow>> GetManagerCatalogingBorrows(DocBorrowCondition condition)
        {
            var fromCreateDate = Utils.GetDate(condition.FromCreateDate);
            var toCreateDate = Utils.GetDate(condition.ToCreateDate);

            IQueryable<VMCatalogingBorrow> temp;
            if (condition.BorrowType == (int)EnumBorrow.BorrowType.Reader)
            {
                temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join o in _dasRepo.Organ.GetAll() on br.IDOrgan equals o.ID
                       join r in _dasRepo.Reader.GetAll() on br.IDReader equals r.ID
                       join ro in _dasRepo.ReaderInOrgan.GetAll() on r.ID equals ro.IDReader

                       where br.IDOrgan == _userPrincipalService.IDOrgan
                        && (condition.Keyword.IsEmpty() || br.Code.Contains(condition.Keyword))
                        && (condition.IDReader == br.IDReader || condition.IDReader <= 0)
                        && (!fromCreateDate.HasValue || fromCreateDate.Value <= br.CreateDate.Value.Date) &&
                        (!toCreateDate.HasValue || toCreateDate.Value >= br.CreateDate.Value.Date)
                        && (condition.Status == br.Status || condition.Status.GetValueOrDefault(0) <= 0)
                        && (br.Status == (int)EnumBorrow.Status.WaitApprove ||
                            br.Status == (int)EnumBorrow.Status.Reject ||
                            br.Status == (int)EnumBorrow.Status.Approved)
                        && condition.BorrowType == br.ReaderType
                       group new { br, o, r }
                           by new
                           {
                               br.ID,
                               br.IDReader,
                               br.Code,
                               br.Status,
                               br.StartDate,
                               br.ReaderType,
                               br.IsReturned,
                               br.IsOriginal,
                               br.ReturnDate,
                               br.ReasonToReject,
                               br.EndDate,
                               br.CreateDate,
                               OrganName = o.Name,
                               ReaderName = r.Name
                           }
                  into g

                       orderby g.Key.ID descending
                       select new VMCatalogingBorrow
                       {
                           ID = g.Key.ID,
                           IDReader = g.Key.IDReader,
                           Code = g.Key.Code,
                           Status = g.Key.Status,
                           OrganName = g.Key.OrganName,
                           CreateDate = g.Key.CreateDate,
                           StartDate = g.Key.StartDate,
                           EndDate = g.Key.EndDate,
                           ReaderName = g.Key.ReaderName,
                           ReasonToReject = g.Key.ReasonToReject,
                           ReaderType = g.Key.ReaderType,
                           IsOriginal = g.Key.IsOriginal,
                       };
            }
            else
            {
                temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join o in _dasRepo.Organ.GetAll() on br.IDOrgan equals o.ID
                       join r in _dasRepo.User.GetAll() on br.IDReader equals r.ID

                       where br.IDOrgan == _userPrincipalService.IDOrgan
                        && (condition.Keyword.IsEmpty() || br.Code.Contains(condition.Keyword))
                        && (condition.IDReader == br.IDReader || condition.IDReader <= 0)
                        && (!fromCreateDate.HasValue || fromCreateDate.Value <= br.CreateDate.Value.Date) &&
                        (!toCreateDate.HasValue || toCreateDate.Value >= br.CreateDate.Value.Date)
                        && (condition.Status == br.Status || condition.Status.GetValueOrDefault(0) <= 0)
                        && (br.Status == (int)EnumBorrow.Status.WaitApprove ||
                            br.Status == (int)EnumBorrow.Status.Reject ||
                            br.Status == (int)EnumBorrow.Status.Approved)
                        && condition.BorrowType == br.ReaderType

                       group new { br, o, r }
                           by new
                           {
                               br.ID,
                               br.IDReader,
                               br.Code,
                               br.Status,
                               br.StartDate,
                               br.ReaderType,
                               br.IsReturned,
                               br.IsOriginal,
                               br.ReturnDate,
                               br.ReasonToReject,
                               br.EndDate,
                               br.CreateDate,
                               OrganName = o.Name,
                               ReaderName = r.Name
                           }
                       into g
                       orderby g.Key.ID descending
                       select new VMCatalogingBorrow
                       {
                           ID = g.Key.ID,
                           IDReader = g.Key.IDReader,
                           Code = g.Key.Code,
                           Status = g.Key.Status,
                           OrganName = g.Key.OrganName,
                           CreateDate = g.Key.CreateDate,
                           StartDate = g.Key.StartDate,
                           EndDate = g.Key.EndDate,
                           ReaderName = g.Key.ReaderName,
                           ReasonToReject = g.Key.ReasonToReject,
                           ReaderType = g.Key.ReaderType,
                           ReturnDate = g.Key.ReturnDate,
                           IsReturned = g.Key.IsReturned,
                           IsOriginal = g.Key.IsOriginal,
                       };
            }
            var totalFile = await temp.LongCountAsync();
            var totalPage = (int)Math.Ceiling(totalFile / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var vmBorrows = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new PaginatedList<VMCatalogingBorrow>(vmBorrows, (int)totalFile, condition.PageIndex, condition.PageSize);
        }


        private async Task<Dictionary<int, string>> GetDictReader()
        {
            var temp = from r in _dasRepo.Reader.GetAll().Where(n => n.Status == (int)EnumCommon.Status.Active)
                       join ro in _dasRepo.ReaderInOrgan.GetAll(n => n.Status == (int)EnumCommon.Status.Active) on r.ID equals
                           ro.IDReader
                       orderby r.Name
                       where ro.IDOrgan == _userPrincipalService.IDOrgan
                       select _mapper.Map<VMReader>(r);

            return (await temp.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictUser()
        {
            var temp = from u in _dasRepo.User.GetAll().Where(n => n.Status == (int)EnumCommon.Status.Active)
                       where u.IDOrgan == _userPrincipalService.IDOrgan
                       orderby u.Name
                       select _mapper.Map<VMUser>(u);
            return (await temp.ToListAsync()).ToDictionary(n => n.ID, n => n.Name);
        }

        private async Task<List<VMDocTypeField>> GetDocTypeFields(int idDoctype)
        {
            var temp = from dtf in _dasRepo.DocTypeField.GetAll()
                       where dtf.Status != (int)EnumCommon.Status.InActive
                             && dtf.IDDocType == idDoctype
                       orderby dtf.Priority
                       select _mapper.Map<VMDocTypeField>(dtf);
            return await temp.ToListAsync();
        }

        #region Cookie
        private List<int> AddToBorrowCart(HttpRequest request, HttpResponse response, int idItem, EnumBorrow.BorrowType borrowType)
        {
            var ccName = CommonConst.BorrowCart + borrowType.ToString() + _userPrincipalService.UserId;
            if (request.Cookies.ContainsKey(ccName))
            {
                var cartValue = GetBorrowCart(request, borrowType);
                if (!cartValue.Any(n => n == idItem))
                {
                    cartValue.Add(idItem);
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(7);
                    response.Cookies.Append(ccName, Utils.Serialize(cartValue), option);
                }
                return cartValue;
            }
            else
            {
                //Create new 
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(7);
                var cartValue = new List<int> { idItem };
                response.Cookies.Append(ccName, Utils.Serialize(cartValue), option);
                return cartValue;
            }
        }

        private List<int> AddToBorrowCart(HttpRequest request, HttpResponse response, List<int> idItems, EnumBorrow.BorrowType borrowType)
        {
            var ccName = CommonConst.BorrowCart + borrowType.ToString() + _userPrincipalService.UserId;
            if (request.Cookies.ContainsKey(ccName))
            {
                var cartValue = GetBorrowCart(request, borrowType);
                var newValue = idItems.Where(n => cartValue.IsEmpty() || !cartValue.Contains(n)).ToList();
                if (newValue.HasValue())
                {
                    cartValue.AddRange(newValue);
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(7);
                    response.Cookies.Append(ccName, Utils.Serialize(cartValue), option);
                }
                return cartValue;
            }
            else
            {
                //Create new 
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(7);
                response.Cookies.Append(ccName, Utils.Serialize(idItems), option);
                return idItems;
            }
        }

        private List<int> UpdateBorrowCart(HttpResponse response, List<int> idItems, EnumBorrow.BorrowType borrowType)
        {
            var ccName = CommonConst.BorrowCart + borrowType.ToString() + _userPrincipalService.UserId;
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(7);
            response.Cookies.Append(ccName, Utils.Serialize(idItems), option);
            return idItems;
        }
        private void RemoveBorrowCart(HttpResponse response, EnumBorrow.BorrowType borrowType)
        {
            var ccName = CommonConst.BorrowCart + borrowType.ToString() + _userPrincipalService.UserId;
            response.Cookies.Delete(ccName);
        }

        #endregion Cookie

        /// <summary>
        /// Thêm độc giả vào danh sách của cơ quan
        /// </summary>
        /// <param name="idOrgan"></param>
        /// <param name="idReader"></param>
        /// <returns></returns>
        private async Task<bool> AddReaderInOrgans(IEnumerable<int> idOrgans, int idReader)
        {
            if (idOrgans.IsEmpty() || idReader <= 0)
                return false;

            var readerInOrgans = await _dasRepo.ReaderInOrgan.GetAllListAsync(x => idOrgans.Contains(x.IDOrgan) && x.IDReader == idReader);
            var newReaderInOrgans = new List<ReaderInOrgan>();
            foreach (var idOrgan in idOrgans)
            {
                if (readerInOrgans != null && readerInOrgans.Any(n => n.IDOrgan == idOrgan))
                    continue;

                newReaderInOrgans.Add(new ReaderInOrgan
                {
                    Status = (int)EnumCommon.Status.Active,
                    IDOrgan = idOrgan,
                    IDReader = idReader
                });
            }

            if (newReaderInOrgans.IsNotEmpty())
            {
                await _dasRepo.ReaderInOrgan.InsertAsync(newReaderInOrgans);
                await _dasRepo.SaveAync();
            }
            return false;
        }
        private async Task<Dictionary<int, string>> GetDictCategory(string codeType)
        {
            var cates = (await _dasRepo.Category.GetAllListAsync(n => codeType.IsNotEmpty() && n.CodeType == codeType && n.Status == (int)EnumCategory.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan));

            if (cates.Any(n => n.ParentId > 0))
            {
                //Render tree
                var treeModels = Utils.RenderTree(cates.Select(n => new TreeModel<VMPosition>
                {
                    ID = n.ID,
                    Name = n.Name,
                    Parent = n.ParentId ?? 0,
                    ParentPath = n.ParentPath ?? "0",
                }).ToList(), null, "--");
                return treeModels.ToDictionary(n => (int)n.ID, n => n.Name);
            }
            return cates.ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictExpiryDate()
        {
            return (await _dasRepo.ExpiryDate.GetAllListAsync(u => u.Status == (int)EnumExpiryDate.Status.Active)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictAgencies()
        {
            return (await _dasRepo.Agency.GetAllListAsync(n => n.Status == (int)EnumAgency.Status.Active && n.IDOrgan == _userPrincipalService.IDOrgan)).OrderBy(n => n.Name).ToDictionary(n => n.ID, n => n.Name);
        }
        private async Task<Dictionary<int, string>> GetDictProfileTemplate(int idProfileTemplate)
        {
            return (await _dasRepo.ProfileTemplate.GetAllListAsync(a => a.Status == (int)EnumOrgan.Status.Active
                                                                        && ((int)EnumProfileTemplate.Type.Open == a.Type || idProfileTemplate == a.ID)
                                                                        && a.IDOrgan == _userPrincipalService.IDOrgan)).ToDictionary(n => n.ID, n => n.FondName);
        }

        /// <summary>
        /// Get tài liệu mình đang mượn & chưa trả bản cứng
        /// </summary>
        /// <param name="idDocs">Ds id doc</param>
        /// <returns></returns>
        private Task<List<VMCatalogingBorrowDoc>> GetBorrowingDocs(List<int> idDocs)
        {
            var temp = from br in _dasRepo.CatalogingBorrow.GetAll()
                       join brd in _dasRepo.CatalogingBorrowDoc.GetAll(n => n.Status != (int)EnumCommon.Status.InActive) on br.ID equals brd.IDCatalogingBorrow

                       where
                              //(br.Status == (int)EnumBorrow.Status.WaitApprove

                              //   || (br.Status == (int)EnumBorrow.Status.Approved
                              //       && br.StartDate.Date <= DateTime.Now.Date && DateTime.Now.Date <= br.EndDate.Date) ||
                              //   (br.Status == (int)EnumBorrow.Status.Approved && br.IsOriginal && !br.IsReturned)) &&
                              //  (idDocs.Contains(br.IDReader) ||

                              (br.Status == (int)EnumBorrow.Status.Approved && br.IsOriginal && !br.IsReturned)

                             && idDocs.Contains(brd.IDDoc)
                       select new VMCatalogingBorrowDoc()
                       {
                           IDDoc = brd.IDDoc,
                           IDReader = br.IDReader,
                           ID = br.ID,
                           IsReturned = br.IsReturned
                       };
            return temp.ToListAsync();
        }


        private async Task<PaginatedList<VMCatalogingDoc>> GetCatalogingDocs(AdvanceSearchCondition condition)
        {
            if (condition == null || condition.IsSearch == 0)
                return new PaginatedList<VMCatalogingDoc>();

            if (condition.IsAdvSearch == 0)
            {
                //Search thường
                var cond = Utils.Bind<DocBorrowCondition>(condition.KeyValue());
                return await GetCatalogingDocs(cond);
            }

            //CatalogingDoc d, CatalogingProfile p, CatalogingDocField df, DocTypeField dtf, DocType dt
            var defaultCondition = $" d.{nameof(CatalogingDoc.Status)} = {(int)EnumDocCollect.Status.Complete}  AND p.{nameof(CatalogingProfile.Status)} = {(int)EnumCataloging.Status.StorageApproved}  ";

            if (condition.ReaderType == (int)EnumBorrow.BorrowType.User)
                defaultCondition += $"AND p.{nameof(CatalogingProfile.IDOrgan)} = {_userPrincipalService.IDOrgan}";


            if (condition.IDDocType > 0)
                defaultCondition += $"AND d.{nameof(CatalogingDoc.IDDocType)} = {condition.IDDocType}";


            if (condition.TypeOfDocType > 0)
                defaultCondition += $"AND dt.{nameof(DocType.Type)} = {condition.TypeOfDocType}";


            var conditionString = string.Empty;
            var conditionCount = 0;
            //Get conditions
            var searchCond = new List<string>();

            if (condition.Conditions != null)
            {
                var fieldIds = condition.Conditions.Select(n => n.IDField);
                var searchFields = await _dasRepo.DocTypeField.GetAllListAsync(n => fieldIds.Contains(n.ID) && n.Status == (int)EnumCommon.Status.Active);

                foreach (var cond in condition.Conditions)
                {
                    var searchField = searchFields.FirstOrDefault(n => n.ID == cond.IDField);
                    if (searchField == null)
                        continue;

                    var rangeCond = new List<string>(); //Chứa các đk từ..đến
                                                        // var field = "Field" + searchField.ID;
                                                        //if (DATA.ContainsKey(field) || DATA.ContainsKey("From_" + field) || DATA.ContainsKey("To_" + field))
                    {
                        var val = cond.Keyword;
                        var valueSearch = $" LIKE N'%{val}%'";
                        if (searchField.InputType == (int)EnumCategoryType.InputType.InpDate)
                        {
                            //Các trường datetime/số thì phải gen ra Từ...Đến (Name input phải có Form_..., To_....)
                            var fromD = cond.FromValue;
                            var toD = cond.ToValue;
                            if (fromD.IsNotEmpty())
                                rangeCond.Add($"dtf.Code = '{searchField.Code}' AND  TRY_CONVERT(DATETIME, df.Value, 103) IS NOT NULL AND  TRY_CONVERT(DATETIME, df.Value, 103) >= CONVERT(DATETIME, '{fromD}', 103) ");
                            if (toD.IsNotEmpty())
                                rangeCond.Add($"dtf.Code = '{searchField.Code}' AND TRY_CONVERT(DATETIME, df.Value, 103) IS NOT NULL AND TRY_CONVERT(DATETIME, df.Value, 103) <=  CONVERT(DATETIME, '{toD}', 103) ");

                            if (rangeCond.IsNotEmpty())
                                searchCond.Add($"({string.Join(" AND ", rangeCond)})");
                        }

                        else if (searchField.InputType == (int)EnumCategoryType.InputType.InpFloat
                            || searchField.InputType == (int)EnumCategoryType.InputType.InpNumber
                            || searchField.InputType == (int)EnumCategoryType.InputType.InpMoney)
                        {
                            var fromN = cond.FromValue;
                            var toN = cond.ToValue;
                            if (searchField.InputType == (int)EnumCategoryType.InputType.InpMoney)
                            {
                                fromN = fromN.Replace(",", string.Empty);
                                toN = toN.Replace(",", string.Empty);
                            }

                            if (fromN.IsNumber())
                                rangeCond.Add($"dtf.Code = '{searchField.Code}' AND  TRY_CONVERT(decimal, df.Value) IS NOT NULL AND TRY_CONVERT(decimal, df.Value) >=  {fromN} ");
                            if (toN.IsNumber())
                                rangeCond.Add($"dtf.Code = '{searchField.Code}' AND TRY_CONVERT(decimal, df.Value) IS NOT NULL AND TRY_CONVERT(decimal, df.Value) <= {toN} ");

                            if (rangeCond.IsNotEmpty())
                                searchCond.Add($"({string.Join(" AND ", rangeCond)})");
                        }
                        else
                        {
                            if (val.IsNotEmpty())
                            {
                                searchCond.Add($"(dtf.Code = '{searchField.Code}' AND df.Value IS NOT NULL AND df.Value like N'%{val}%' )");
                            }
                        }
                    }
                }
                if (searchCond.IsNotEmpty())
                {
                    conditionString = string.Join(" OR ", searchCond);
                    //conditionCount = searchCond.Count;
                    conditionCount = condition.Conditions.Select(n => n.IDField).Distinct().Count();
                }
            }

            var totalFilter = new SqlParameter
            {
                ParameterName = "@TotalFilter",
                Direction = ParameterDirection.Output,
                Value = 0
            };
            object[] parameters =
             {
                 new SqlParameter(){  ParameterName = "@DefaultString",   SqlDbType = SqlDbType.NVarChar,   Value = defaultCondition },
                 new SqlParameter(){  ParameterName = "@ConditionString",   SqlDbType = SqlDbType.NVarChar,   Value = conditionString },
                 new SqlParameter("@CountOfFieldSearch", conditionCount),
                 new SqlParameter("@PageNumber", condition.PageIndex),
                 new SqlParameter("@PageSize" ,condition.PageSize),
                 totalFilter
             };

            var vmDocs = await _dasRepo.CatalogingDoc.ExecuteStoreProc("EXECUTE [dbo].[AdvanceSearchDoc] @DefaultString, @ConditionString, @CountOfFieldSearch, @PageNumber, @PageSize, @TotalFilter OUT", parameters).Select(n => new VMCatalogingDoc
            {
                ID = n.ID,
                IDDoc = n.IDDoc,
                IDCatalogingProfile = n.IDCatalogingProfile,
                IDDocType = n.IDDocType,
                Status = n.Status
            }).ToListAsync();

            var totalFile = Utils.IsNumber(totalFilter.Value?.ToString()) ? (int)totalFilter.Value : 0; //Lấy ra số bản ghi
            var idDocTypelist = vmDocs.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypelist);
            var docTypeFields = await GetDocTypeFields(idDocTypelist);
            var catalogingDocField = await GetCatalogingDocFieldsByIDs(vmDocs.Select(x => x.ID));

            var idProfiles = vmDocs.Select(n => n.IDCatalogingProfile).Distinct();
            var profiles = await _dasRepo.CatalogingProfile.GetAllListAsync(n => idProfiles.Contains(n.ID));
            foreach (var doc in vmDocs)
            {
                var profile = profiles.FirstOrNewObj(n => n.ID == doc.IDCatalogingProfile);
                doc.ProfileName = profile.Title;
                doc.ProfileCode = profile.FileCode;
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMCatalogingDocFields = catalogingDocField.Where(x => x.IDCatalogingDoc == doc.ID).ToList();
                //Dictionary
                doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMCatalogingDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
            }
            return new PaginatedList<VMCatalogingDoc>(vmDocs, (int)totalFile, condition.PageIndex, condition.PageSize);

        }



        #endregion Private methods
    }
}
