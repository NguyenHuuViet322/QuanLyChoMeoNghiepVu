using AutoMapper;
using ESD.Application.Constants;
using ESD.Application.Enums;
using ESD.Application.Interfaces;
using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Enums;
using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.ContextAccessors;
using ESD.Utility;
using ESD.Utility.CacheUtils;
using ESD.Utility.LogUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESD.Application.Services
{
    public class ReportArchiveService : BaseMasterService, IReportArchiveServices
    {
        #region Properties
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly ICacheManagementServices _cacheManagementServices;
        #endregion Properties

        #region Ctor
        public ReportArchiveService(IDasRepositoryWrapper dasRepository
            , IMapper mapper
            , ILoggerManager loggerManager
            , IUserPrincipalService userPrincipalService
            , ICacheManagementServices cacheManagementServices) : base(dasRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _userPrincipalService = userPrincipalService;
            _cacheManagementServices = cacheManagementServices;
        }
        #endregion Ctor

        #region SendArchive
        #region List & Search
        public async Task<VMIndexReportSendArchive> ReportSendArchivePaging(ReportSendArchiveCondition condition, bool isExport = false)
        {
            var model = new VMIndexReportSendArchive();
            model.ReportSendArchiveCondition = condition;

            var pagDoc = await GetReportSendArchive(condition, isExport);
            var idDocTypes = pagDoc.Select(x => x.IDDocType).Distinct();
            var docTypes = await GetDocTypes(idDocTypes);
            var docTypeFields = await GetDocTypeFields(idDocTypes);
            var docField = await GetDocFieldsByIDs(pagDoc.Select(x => x.ID));
            List<int> listUnvalid = new List<int>();
            foreach (var doc in pagDoc)
            {
                doc.VMDocType = docTypes.Where(x => x.ID == doc.IDDocType).FirstOrDefault();
                doc.VMDocTypeFields = docTypeFields.Where(x => x.IDDocType == doc.IDDocType).OrderBy(x => x.Priority).ToList();
                doc.VMDocFields = docField.Where(x => x.IDDoc == doc.ID).ToList();
                if (!docField.Where(x => x.IDDoc == doc.ID).Select(df => df.Status).Distinct().Contains((int)EnumCommon.Status.InActive))
                {
                    doc.dictCodeValue = doc.VMDocTypeFields.ToDictionary(k => k.Code, v => doc.VMDocFields.Where(x => x.IDDocTypeField == v.ID).FirstOrDefault().Value ?? "");
                }
                else
                {
                    listUnvalid.Add(doc.ID);
                }
            }
            if (listUnvalid.Count() > 0)
            {
                pagDoc.RemoveAll(x => listUnvalid.Contains(x.ID));
                pagDoc.TotalFilter = pagDoc.TotalFilter - listUnvalid.Count();
            }
            model.VMReportSendArchives = pagDoc;
            return model;
        }
        #endregion List & Search

        #region Private Func
        private async Task<PaginatedList<VMReportSendArchive>> GetReportSendArchive(ReportSendArchiveCondition condition, bool isExport = false)
        {
            var idAgency = _userPrincipalService.IDAgency;
            var temp = from p in _dasRepo.Plan.GetAll()
                       join pp in _dasRepo.PlanProfile.GetAll() on p.ID equals pp.IDPlan
                       join d in _dasRepo.Doc.GetAll() on pp.ID equals d.IDProfile
                       where (d.Status != (int)EnumDocCollect.Status.InActive)
                       && pp.Status == (int)EnumProfilePlan.Status.ArchiveApproved
                       && pp.IDAgency == idAgency
                       && (condition.Keyword.IsEmpty() || p.Name.Contains(condition.Keyword))
                       orderby p.ID descending, pp.ID descending, d.ID descending
                       select new VMReportSendArchive
                       {
                           ID = d.ID,
                           IDChannel = d.IDChannel,
                           IDFile = d.IDFile,
                           IDProfile = d.IDProfile,
                           IDDocType = d.IDDocType,
                           Status = d.Status,
                           Profile_Title = pp.Title,
                           Profile_FileCode = pp.FileCode,
                           Plan_Name = p.Name
                       };
            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMReportSendArchive>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var docs = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMReportSendArchive>(docs, (int)total, condition.PageIndex, condition.PageSize);
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
        private async Task<List<VMDocField>> GetDocFieldsByIDs(IEnumerable<int> IDDocs)
        {
            var temp = from df in _dasRepo.DocField.GetAll()
                       where /*df.Status != (int)EnumCommon.Status.InActive*/
                       //&& 
                       IDDocs.Contains(df.IDDoc)
                       select _mapper.Map<VMDocField>(df);
            return await temp.ToListAsync();
        }
        #endregion Private Func

        #endregion SendArchive

        #region ReceiveArchive

        #region List & Search
        public async Task<VMIndexReportReceiveArchive> ReportReceiveArchivePaging(ReportReceiveArchiveCondition condition, bool isExport = false)
        {
            var model = new VMIndexReportReceiveArchive();
            model.ReportReceiveArchiveCondition = condition;
            model.VMReportReceiveArchives = await GetReportReceiveArchives(condition, isExport);
            model.DictAgency = await GetDictAgency();
            return model;
        }
        #endregion List & Search

        #region Private Func
        #endregion Private Func
        private async Task<PaginatedList<VMReportReceiveArchive>> GetReportReceiveArchives(ReportReceiveArchiveCondition condition, bool isExport = false)
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            var temp1 = from organ in _dasRepo.Organ.GetAll().Where(x => x.ID == userData.IDOrgan)
                        from agency in _dasRepo.Agency.GetAll().Where(x => x.IDOrgan == organ.ID && (condition.ListAgencyStr.Count == 0 || condition.ListAgencyStr.Contains(x.ID.ToString()))).DefaultIfEmpty()
                        from profile in _dasRepo.PlanProfile.GetAll()
                        .Where(x => x.IDAgency == agency.ID ).DefaultIfEmpty()
                        from doc in _dasRepo.Doc.GetAll().Where(x => x.IDProfile == profile.ID).DefaultIfEmpty()
                        from plan in _dasRepo.Plan.GetAll().Where(x => x.ID == profile.IDPlan &&(condition.Keyword.IsEmpty() || x.Name.Contains(condition.Keyword))).DefaultIfEmpty()
                        where plan.Status == (int)EnumPlan.Status.Approved
                        && profile.Status != (int)EnumPlan.Status.InActive
                        && doc.Status != (int)EnumDocCollect.Status.InActive
                        && agency.Status != (int)EnumCommon.Status.InActive
                        group new { organ, plan, agency, IDProfile = profile.ID, IDDoc = doc.ID } by new
                        {
                            OrganID = organ.ID,
                            PlanID = plan.ID,
                            PlanName = plan.Name,
                            AgencyID = agency.ID,
                            AgencyName = agency.Name,
                            IdProfile = profile.ID
                        } into g
                        select new
                        {
                            IDPlan = g.Key.PlanID,
                            IDAgency = g.Key.AgencyID,
                            PlanName = g.Key.PlanName,
                            AgencyName = g.Key.AgencyName,
                            IdProfile = g.Key.IdProfile,
                            CountDoc = g.Count()
                        };
            var temp = from x in temp1
                       group x by new { x.IDPlan, x.IDAgency, x.PlanName, x.AgencyName }
                      into gr
                       select new VMReportReceiveArchive
                       {
                           IDPlan = gr.Key.IDPlan,
                           IDAgency = gr.Key.IDAgency,
                           PlanName = gr.Key.PlanName,
                           AgencyName = gr.Key.AgencyName,
                           CountProfile = gr.Count(),
                           CountDoc = gr.Sum(x => x.CountDoc)
                       };

            if (isExport)
            {
                var rs = await temp.ToListAsync();
                return new PaginatedList<VMReportReceiveArchive>(rs, rs.Count(), 1, rs.Count());
            }
            var total = await temp.LongCountAsync();
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            if (totalPage < condition.PageIndex)
                condition.PageIndex = 1;
            var rp = await temp.Skip((condition.PageIndex - 1) * condition.PageSize).Take(condition.PageSize).ToListAsync();
            return new PaginatedList<VMReportReceiveArchive>(rp, (int)total, condition.PageIndex, condition.PageSize);

        }

        private async Task<Dictionary<int, string>> GetDictAgency()
        {
            UserData userData = await _cacheManagementServices.GetUserDataAndSetCache();

            return (await _dasRepo.Agency.GetAllListAsync(x => x.Status != (int)EnumCommon.Status.InActive && x.IDOrgan == userData.IDOrgan)).ToDictionary(k => k.ID, v => v.Name);
        }
        #endregion ReceiveArchive
    }
}
