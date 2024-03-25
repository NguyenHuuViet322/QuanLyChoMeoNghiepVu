using Bogus;
using Bogus.DataSets;
using ESD.Application.Enums;
using ESD.Domain.Models.DAS;
using ESD.Infrastructure.Contexts;
using ESD.Utility;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESD.Domain.Enums;
using ESD.Application.Constants;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DAS.MockData
{
    internal class Program
    {
        private const int numOfUser = 100;          // Số người dùng
        private const int numOfPlanProfile = 10000;  //200      // Số hồ sơ trung bình trong 1 kế hoạch
        private const int numOfDocInProfile = 70;  // 30        // Số tài liệu trung bình 1 hồ sơ

        private const int maxNumOfWordInTitle = 10;

        private static async Task Main(string[] args)
        {
            //await InsertPermission();
            //await InsertUser(false, 1019, 2075);
            //await InsertPlanProfile(1019, 2075, 500);
            //await InsertDoc(1019, 2075, 500);
            //await InsertCatalogingProfile(1019, 2075, 500);
            //await InsertCatalogingDoc(1019, 2075, 500);
            Console.WriteLine("Mockup data done!");
            Console.ReadLine();
        }

        /// <summary>
        /// Insert Permission, Module, ModuleChild
        /// </summary>
        private static async Task InsertPermissionTable()
        {
            await using var ct = new ESDContext();

            var temp = ct.Database.ExecuteSqlRaw("TRUNCATE TABLE PERMISSION");
            var moduleHasCode = ct.Module.Where(s => s.Code != 0).ToList();
            if (moduleHasCode.Count == 0)
                return;

            var dicPerTypeAndDes = EnumUltils.GetDescription<EnumPermission.Type>();
            var lstPermission = new List<Permission>();

            foreach (var module in moduleHasCode)
            {
                foreach (var type in dicPerTypeAndDes)
                {
                    lstPermission.Add(new Permission
                    {
                        IDModule = module.ID,
                        Name = type.Value,
                        Type = (int)type.Key,
                    });
                }

            }

            var x = lstPermission.Count;
            await ct.Permission.AddRangeAsync(lstPermission);
            await ct.SaveChangesAsync();

        }

        private static async Task InsertAdminOrgan()
        {
            await using var ct = new ESDContext();

            //1. Tạo nhóm quyền admin đơn vị nếu chưa tồn tại
            int idAdminGrp;
            string strAdmin = CommonConst.AdminOrgan.ToString();
            var adminOrgan = await ct.GroupPermission.FirstAsync(c => c.Name == strAdmin);
            if (adminOrgan == null)
            {
                var roleAdminOrgan = new GroupPermission
                {
                    IDChannel = 0,
                    Name = CommonConst.AdminOrgan.ToString(),
                    Status = (int)EnumGroupPermission.Status.Active,
                    Description = "Admin cơ quan"
                };
                ct.GroupPermission.Add(roleAdminOrgan);
                await ct.SaveChangesAsync();
                idAdminGrp = roleAdminOrgan.ID;
            }
            else
            {
                idAdminGrp = adminOrgan.ID;
            }

            if (ct.PermissionGroupPer.Any(s => s.IDGroupPermission == idAdminGrp))
            {
                Console.WriteLine("Nhóm quyền Admin cơ quan đã có các quyền tương ứng");
                return;
            }

            //2. gán quyền mặc định cho nhóm quyền admin đơn vị
            var moduleCodeForAdmin = new int[]
            {
                // (int)EnumModule.Code.QLDG // quản lý độc giả
                (int)EnumModule.Code.S9020 // người dùng
                , (int)EnumModule.Code.S9010 // nhóm người dùng
               // , (int)EnumModule.Code.M20150 //nhật ký hệ thống
                , (int)EnumModule.Code.NKND //nhật ký người dùng
            };


            var modules = ct.Module.Select(row => row).ToList();
            var permissions = ct.Permission.Select(row => row).ToList();
            var listPer = new List<Permission>();
            foreach (var item in moduleCodeForAdmin)
            {
                var moduleForItem = modules.Where(s => s.Code == item).Select(s => s.ID);
                var perForItem = permissions.Where(s => moduleForItem.Contains(s.IDModule));
                if (perForItem != null && perForItem.Count() > 0)
                {
                    listPer.AddRange(perForItem);
                }
            }
            var permissionGroupPers = new List<PermissionGroupPer>();
            var dtNow = DateTime.Now;
            foreach (var item in listPer)
            {
                permissionGroupPers.Add(new PermissionGroupPer
                {
                    CreateDate = dtNow,
                    IDGroupPermission = idAdminGrp,
                    IDPermission = item.ID,
                    Status = 1
                });
            }

            ct.PermissionGroupPer.AddRange(permissionGroupPers);
            await ct.SaveChangesAsync();
            Console.WriteLine("InsertAdminOrgan Done!");

        }

        /// <summary>
        /// Insert mockup user
        /// </summary>
        /// <param name="isResetDb"></param>
        /// <param name="oId">organId</param>
        /// <param name="aId">agencyId</param>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        private static async Task InsertUser(bool isResetDb = false, int oId = 0, int aId = 0, int bulkSize = 2000)
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = bulkSize
            };

            await using var ct = new ESDContext();
            if (isResetDb)
            {
                ct.User.RemoveRange(ct.User);
                await ct.Database.ExecuteSqlCommandAsync("DBCC CHECKIDENT('User', RESEED, 0)");
                await ct.SaveChangesAsync();
            }

            var positionIds = await GetPositionIds(ct);
            List<int> organIds;
            if (oId != 0)
            {
                organIds = new List<int>
                {
                    oId
                };
            }
            else
            {
                organIds = await GetOrganIds(ct);
            }
            var teamIds = await GetTeamIds(ct);

            // Ngày sinh từ 1/1/1965 -> 1/1/1997
            var startDob = new DateTime(1965, 1, 1);
            var endDob = new DateTime(1997, 1, 1);

            if (positionIds.Any() && organIds.Any() && teamIds.Any())
            {
                foreach (var organId in organIds)
                {
                    List<int> agenceyIds;
                    if (aId != 0)
                    {
                        agenceyIds = new List<int>
                        {
                            aId
                        };
                    }
                    else
                    {
                        agenceyIds = await GetAgencyIds(ct, organId);
                    }
                    if (agenceyIds.Any())
                    {
                        //Set the randomizer seed if you wish to generate repeatable data sets.
                        Randomizer.Seed = new Random(8675309);

                        var testUsers = new Faker<User>("vi")
                            .RuleFor(u => u.Name, (f, u) => f.Name.FullName((Name.Gender)u.Gender))
                            //.RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                            .RuleFor(u => u.AccountName, (f, u) => f.Internet.UserName())
                            .RuleFor(u => u.Password, f => StringUltils.Md5Encryption("123456"))
                            .RuleFor(u => u.IdentityNumber, f => f.Random.Replace("##########"))
                            .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
                            .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber("(###)###-####"))
                            .RuleFor(u => u.Birthday, (f, u) => f.Date.Between(startDob, endDob))
                            .RuleFor(u => u.Address, f => f.Address.FullAddress())
                            .RuleFor(u => u.Gender, f => f.Random.Number(0, 1))
                            //.RuleFor(u => u.IDOrgan, f => f.PickRandom(organIds))
                            .RuleFor(u => u.IDOrgan, f => organId)
                            .RuleFor(u => u.IDAgency, f => f.PickRandom(agenceyIds))
                            .RuleFor(u => u.IDPosition, f => f.PickRandom(positionIds))
                            .RuleFor(u => u.IDTeam, f => f.PickRandom(teamIds));

                        var users = testUsers.Generate(numOfUser);

                        await ct.BulkInsertAsync(users, bulkConfig);
                    }
                }
            }
        }

        /// <summary>
        /// Insert PlanProfile
        /// </summary>
        /// <param name="oId">organId</param>
        /// <param name="aId">agencyId</param>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        private static async Task InsertPlanProfile(int oId = 0, int aId = 0, int bulkSize = 2000)
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = bulkSize
            };

            await using var ct = new ESDContext();

            List<int> organIds;
            if (oId != 0)
            {
                organIds = new List<int>
                {
                    oId
                };
            }
            else
            {
                organIds = await GetOrganIds(ct);
            }
            var expiryDateIds = await GetExpiryDateIds(ct);
            var planProfileStatuses = new List<int>
            {
                (int)EnumProfilePlan.Status.Active,
                (int)EnumProfilePlan.Status.WaitApprove,
                (int)EnumProfilePlan.Status.WaitArchiveApproved,
                (int)EnumProfilePlan.Status.CollectComplete,
                (int)EnumProfilePlan.Status.ArchiveApproved
            };

            var planProfileRejectStatuses = new List<int>
            {
                (int)EnumProfilePlan.Status.Reject,
                (int)EnumProfilePlan.Status.ArchiveReject
            };

            if (organIds.Any() && expiryDateIds.Any())
            {
                foreach (var organId in organIds)
                {
                    List<int> agenceyIds;
                    if (aId != 0)
                    {
                        agenceyIds = new List<int>
                        {
                            aId
                        };
                    }
                    else
                    {
                        agenceyIds = await GetAgencyIds(ct, organId);
                    }
                    var planIds = await GetPlanIds(ct, organId);
                    var storageIds = await GetCategoryIds(ct, EnumCategoryType.Code.DM_Kho.ToString(), organId);
                    var profileCategoryIds = await GetCategoryIds(ct, EnumCategoryType.Code.DM_PhanLoaiHS.ToString(), organId);

                    if (agenceyIds.Any() && planIds.Any() && storageIds.Any() && profileCategoryIds.Any())
                    {
                        // Có 30% kế hoạch có hồ sơ từ chối
                        int numOfPlanReject = Convert.ToInt32(planIds.Count * 0.3);
                        var planRejectIds = planIds.Shuffle().Take(numOfPlanReject).ToList();
                        var planNonRejectIds = planIds.Where(p => planRejectIds.All(p2 => p2 != p)).ToList();

                        Randomizer.Seed = new Random(8675309);

                        // Số hồ sơ trong 1 plan từ 0.8*numOfPlanProfile -> 1.2*numOfPlanProfile
                        int minNumOfPlanProfile = Convert.ToInt32(numOfPlanProfile * 0.8);
                        int maxNumOfPlanProfile = Convert.ToInt32(numOfPlanProfile * 1.2);
                        int numOfPlanProfile1 = RandomInt(minNumOfPlanProfile, maxNumOfPlanProfile);    // Số lượng hồ sơ trong Kế hoạch ko có hồ sơ reject
                        int numOfPlanProfile2 = RandomInt(minNumOfPlanProfile, maxNumOfPlanProfile);    // Số lượng hồ sơ none reject trong Kế hoạch có hồ sơ reject & non reject
                        int numOfPlanProfile3 = RandomInt(minNumOfPlanProfile, maxNumOfPlanProfile);    // Số lượng hồ sơ reject trong Kế hoạch có hồ sơ reject & non reject

                        int minNumOfWordInTitle = Convert.ToInt32(maxNumOfWordInTitle * 0.2);
                        int numOfWord = RandomInt(minNumOfWordInTitle, maxNumOfWordInTitle);

                        // 1. Thuộc Kế hoạch ko có hồ sơ reject
                        if (planNonRejectIds.Any())
                        {
                            var testPlanProfiles1 = new Faker<PlanProfile>("vi")
                                    .RuleFor(u => u.IDPlan, (f, u) => f.PickRandom(planNonRejectIds))
                                    .RuleFor(u => u.FileCode, (f, u) => f.Random.String2(3, 30))
                                    .RuleFor(u => u.FileNotation, (f, u) => f.Random.Replace("#####.????"))
                                    .RuleFor(u => u.IDStorage, (f, u) => f.PickRandom(storageIds))
                                    .RuleFor(u => u.Title, (f, u) => f.Random.Words(numOfWord))
                                    .RuleFor(u => u.IDExpiryDate, (f, u) => f.PickRandom(expiryDateIds))
                                    .RuleFor(u => u.IDAgency, (f, u) => f.PickRandom(agenceyIds))
                                    .RuleFor(u => u.IDOrgan, (f, u) => organId)
                                    .RuleFor(u => u.IDProfileCategory, (f, u) => f.PickRandom(profileCategoryIds))
                                    .RuleFor(u => u.Status, (f, u) => f.PickRandom(planProfileStatuses))
                                ;
                            var profiles1 = testPlanProfiles1.Generate(numOfPlanProfile1);
                            await ct.BulkInsertAsync(profiles1, bulkConfig);
                        }

                        // 2. Thuộc Kế hoạch có hồ sơ reject & non reject
                        if (planRejectIds.Any())
                        {
                            // 2.0. Hồ sơ non reject
                            var testPlanProfiles2 = new Faker<PlanProfile>("vi")
                                    .RuleFor(u => u.IDPlan, (f, u) => f.PickRandom(planRejectIds))
                                    .RuleFor(u => u.FileCode, (f, u) => f.Random.String2(3, 30))
                                    .RuleFor(u => u.FileNotation, (f, u) => f.Random.Replace("#####.????"))
                                    .RuleFor(u => u.IDStorage, (f, u) => f.PickRandom(storageIds))
                                    .RuleFor(u => u.Title, (f, u) => f.Random.Words(numOfWord))
                                    .RuleFor(u => u.IDExpiryDate, (f, u) => f.PickRandom(expiryDateIds))
                                    .RuleFor(u => u.IDAgency, (f, u) => f.PickRandom(agenceyIds))
                                    .RuleFor(u => u.IDOrgan, (f, u) => organId)
                                    .RuleFor(u => u.IDProfileCategory, (f, u) => f.PickRandom(profileCategoryIds))
                                    .RuleFor(u => u.Status, (f, u) => f.PickRandom(planProfileStatuses))
                                ;

                            // 2.1. Hồ sơ bị từ chối thì có thêm trường lý do từ chối
                            var testPlanProfiles3 = new Faker<PlanProfile>("vi")
                                    .RuleFor(u => u.IDPlan, (f, u) => f.PickRandom(planRejectIds))
                                    .RuleFor(u => u.FileCode, (f, u) => f.Random.String2(3, 30))
                                    .RuleFor(u => u.FileNotation, (f, u) => f.Random.Replace("#####.????"))
                                    .RuleFor(u => u.IDStorage, (f, u) => f.PickRandom(storageIds))
                                    .RuleFor(u => u.Title, (f, u) => f.Random.Words(numOfWord))
                                    .RuleFor(u => u.IDExpiryDate, (f, u) => f.PickRandom(expiryDateIds))
                                    .RuleFor(u => u.IDAgency, (f, u) => f.PickRandom(agenceyIds))
                                    .RuleFor(u => u.IDOrgan, (f, u) => organId)
                                    .RuleFor(u => u.IDProfileCategory, (f, u) => f.PickRandom(profileCategoryIds))
                                    .RuleFor(u => u.Status, (f, u) => f.PickRandom(planProfileRejectStatuses))
                                    .RuleFor(u => u.ReasonToReject, (f, u) => f.Random.Words(numOfWord))
                                ;

                            var profiles2 = testPlanProfiles2.Generate(numOfPlanProfile2);
                            var profiles3 = testPlanProfiles3.Generate(numOfPlanProfile3);

                            await ct.BulkInsertAsync(profiles2, bulkConfig);
                            await ct.BulkInsertAsync(profiles3, bulkConfig);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Insert Doc & DocField
        /// </summary>
        /// <param name="oId">organId</param>
        /// <param name="aId">agencyId</param>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        private static async Task InsertDoc(int oId = 0, int aId = 0, int bulkSize = 2000)
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = bulkSize
            };

            await using var ct = new ESDContext();

            // 0. Insert data file
            var docFileIds = await GetDocFileIds(ct);

            List<int> organIds;
            if (oId != 0)
            {
                organIds = new List<int>
                {
                    oId
                };
            }
            else
            {
                organIds = await GetOrganIds(ct);
            }

            if (organIds.Any())
            {
                foreach (var organId in organIds)
                {
                    // 1. Insert Doc
                    List<int> agenceyIds;
                    if (aId != 0)
                    {
                        agenceyIds = new List<int>
                        {
                            aId
                        };
                    }
                    else
                    {
                        agenceyIds = await GetAgencyIds(ct, organId);
                    }
                    var planIds = await GetPlanIds(ct, organId);
                    var docTypeIds = await GetDocTypeIds(ct, organId);

                    if (agenceyIds.Any() && planIds.Any() && docTypeIds.Any())
                    {
                        foreach (var agenceyId in agenceyIds)
                        {
                            foreach (var planId in planIds)
                            {
                                var planProfiles = await GetPlanProfileIds(ct, organId, agenceyId, planId);
                                // Trạng thái hồ sơ là "Đang thu thập"
                                var planProfileActiveIds = planProfiles.Where(x => x.Status == (int)EnumProfilePlan.Status.Active).Select(x => x.ID).ToList();
                                // Trạng thái hồ sơ khác "Đang thu thập" => Thành phần hồ sơ luôn luôn là Hoàn thành
                                var planProfileNonActiveIds = planProfiles.Where(x => x.Status != (int)EnumProfilePlan.Status.Active).Select(x => x.ID).ToList();

                                Randomizer.Seed = new Random(8675309);

                                int minNumOfDocInProfile = Convert.ToInt32(numOfDocInProfile * 0.8);
                                int maxNumOfDocInProfile = Convert.ToInt32(numOfDocInProfile * 1.2);
                                int numOfDocActive = RandomInt(minNumOfDocInProfile, maxNumOfDocInProfile) * planProfileActiveIds.Count;
                                int numOfDocNonActive = RandomInt(minNumOfDocInProfile, maxNumOfDocInProfile) * planProfileNonActiveIds.Count;

                                if (planProfileActiveIds.Any())
                                {
                                    var testDocActives = new Faker<Doc>("vi")
                                            .RuleFor(u => u.IDProfile, (f, u) => f.PickRandom(planProfileActiveIds))
                                            .RuleFor(u => u.IDDocType, (f, u) => f.PickRandom(docTypeIds))
                                            .RuleFor(u => u.IDFile, (f, u) => f.PickRandom(docFileIds))
                                            .RuleFor(u => u.Status, (f, u) => (int)EnumDocCollect.Status.Active)
                                        ;
                                    var docActives = testDocActives.Generate(numOfDocActive);
                                    await ct.BulkInsertAsync(docActives, bulkConfig);
                                }

                                if (planProfileNonActiveIds.Any())
                                {
                                    var testDocNonActives = new Faker<Doc>("vi")
                                            .RuleFor(u => u.IDProfile, (f, u) => f.PickRandom(planProfileNonActiveIds))
                                            .RuleFor(u => u.IDDocType, (f, u) => f.PickRandom(docTypeIds))
                                            .RuleFor(u => u.IDFile, (f, u) => f.PickRandom(docFileIds))
                                            .RuleFor(u => u.Status, (f, u) => (int)EnumDocCollect.Status.Complete)
                                        ;

                                    var docNonActives = testDocNonActives.Generate(numOfDocNonActive);
                                    await ct.BulkInsertAsync(docNonActives, bulkConfig);
                                }
                            }
                        }
                    }

                    // 2. Insert DocField
                    var testDocFields = new List<DocField>();
                    foreach (var docTypeId in docTypeIds)
                    {
                        var docIds = await GetDocIds(ct, docTypeId);
                        var docTypeFields = await GetDocTypeFields(ct, organId, docTypeId);

                        foreach (var docTypeField in docTypeFields)
                        {
                            int min = 1;
                            int max = 100;


                            string value;
                            switch (docTypeField.InputType)
                            {
                                case (int)EnumDocType.InputType.InpNumber:
                                    value = RandomInt(min, max).ToString();
                                    break;
                                case (int)EnumDocType.InputType.InpFloat:
                                    value = RandomDouble(min, max).ToString();
                                    break;
                                case (int)EnumDocType.InputType.InpMoney:
                                    value = RandomDouble(min, max).ToString();
                                    break;
                                case (int)EnumDocType.InputType.InpDate:
                                    var startDate = new DateTime(2015, 1, 1);
                                    var endDate = new DateTime(2020, 12, 31);
                                    value = (RandomDateTime(startDate, endDate)).ToString();
                                    break;
                                case (int)EnumDocType.InputType.InpTextArea:
                                    value = RandomString();
                                    break;
                                default:
                                    value = RandomString();
                                    break;
                            }

                            foreach (var docId in docIds)
                            {
                                // add to list DocFields
                                testDocFields.Add(new DocField
                                {
                                    IDDoc = docId,
                                    IDDocTypeField = docTypeField.ID,
                                    Value = value,
                                    Status = (int)EnumCommon.Status.Active
                                });
                            }
                        }
                    }

                    await ct.BulkInsertAsync(testDocFields, bulkConfig);

                }
            }
        }

        /// <summary>
        /// Insert CatalogingProfile
        /// </summary>
        /// <param name="oId">organId</param>
        /// <param name="aId">agencyId</param>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        private static async Task InsertCatalogingProfile(int oId = 0, int aId = 0, int bulkSize = 2000)
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = bulkSize
            };

            await using var ct = new ESDContext();

            List<int> organIds;
            if (oId != 0)
            {
                organIds = new List<int>
                {
                    oId
                };
            }
            else
            {
                organIds = await GetOrganIds(ct);
            }
            if (organIds.Any())
            {
                var listCatalogingProfile = new List<CatalogingProfile>();
                foreach (var organId in organIds)
                {
                    List<int> agenceyIds;
                    if (aId != 0)
                    {
                        agenceyIds = new List<int>
                        {
                            aId
                        };
                    }
                    else
                    {
                        agenceyIds = await GetAgencyIds(ct, organId);
                    }
                    var planIds = await GetPlanIds(ct, organId);
                    if (agenceyIds.Any())
                    {
                        foreach (var agenceyId in agenceyIds)
                        {
                            foreach (var planId in planIds)
                            {
                                var planProfileArchiveApproveds = await GetPlanProfileArchiveApproveds(ct, organId, agenceyId, planId);

                                // add CatalogingProfile
                                foreach (var planProfile in planProfileArchiveApproveds)
                                {
                                    listCatalogingProfile.Add(new CatalogingProfile
                                    {
                                        IDPlanProfile = planProfile.ID,
                                        IDChannel = planProfile.IDChannel,
                                        IDPlan = planProfile.IDPlan,
                                        FileCode = planProfile.FileCode,
                                        IDStorage = planProfile.IDStorage,
                                        IDCodeBox = planProfile.IDCodeBox,
                                        IDProfileList = planProfile.IDProfileList,
                                        IDSecurityLevel = planProfile.IDSecurityLevel,
                                        IDProfileTemplate = planProfile.IDProfileTemplate,
                                        Identifier = planProfile.Identifier,
                                        FileCatalog = planProfile.FileCatalog,
                                        FileNotation = planProfile.FileNotation,
                                        Title = planProfile.Title,
                                        IDExpiryDate = planProfile.IDExpiryDate,
                                        Rights = planProfile.Rights,
                                        Language = planProfile.Language,
                                        StartDate = planProfile.StartDate,
                                        EndDate = planProfile.EndDate,
                                        TotalDoc = planProfile.TotalDoc,
                                        Description = planProfile.Description,
                                        InforSign = planProfile.InforSign,
                                        Keyword = planProfile.Keyword,
                                        Maintenance = planProfile.Maintenance,
                                        PageNumber = planProfile.PageNumber,
                                        Format = planProfile.Format,
                                        Status = planProfile.Status,
                                        IDAgency = planProfile.IDAgency,    //agenceyId
                                        IDOrgan = planProfile.IDOrgan,      //organId
                                        ReasonToReject = planProfile.ReasonToReject,
                                        ApprovedBy = planProfile.ApprovedBy,
                                        ApprovedDate = planProfile.ApprovedDate,
                                        Type = planProfile.Type,
                                        IDProfileCategory = planProfile.IDProfileCategory
                                    });
                                }
                            }
                        }
                    }
                }

                if (listCatalogingProfile.Any())
                {
                    await ct.BulkInsertAsync(listCatalogingProfile, bulkConfig);
                }
            }
        }

        /// <summary>
        /// Insert CatalogingDoc & CatalogingDocField
        /// </summary>
        /// <param name="oId">organId</param>
        /// <param name="aId">agencyId</param>
        /// <param name="bulkSize"></param>
        /// <returns></returns>
        private static async Task InsertCatalogingDoc(int oId = 0, int aId = 0, int bulkSize = 2000)
        {
            var bulkConfig = new BulkConfig
            {
                BatchSize = bulkSize
            };

            await using var ct = new ESDContext();
            ct.Database.SetCommandTimeout(99999999);

            List<int> organIds;
            if (oId != 0)
            {
                organIds = new List<int>
                {
                    oId
                };
            }
            else
            {
                organIds = await GetOrganIds(ct);
            }

            if (organIds.Any())
            {
                var listCatalogingDoc = new List<CatalogingDoc>();
                var listCatalogingDocField = new List<CatalogingDocField>();
                foreach (var organId in organIds)
                {
                    // 1. Insert CatalogingDoc
                    List<int> agenceyIds;
                    if (aId != 0)
                    {
                        agenceyIds = new List<int>
                        {
                            aId
                        };
                    }
                    else
                    {
                        agenceyIds = await GetAgencyIds(ct, organId);
                    }
                    var planIds = await GetPlanIds(ct, organId);
                    var docTypeIds = await GetDocTypeIds(ct, organId);

                    if (agenceyIds.Any() && planIds.Any() && docTypeIds.Any())
                    {
                        foreach (var agenceyId in agenceyIds)
                        {
                            foreach (var planId in planIds)
                            {
                                // get IDCatalogingProfile & IDPlanProfile
                                var catalogingProfileWithProfileIds =
                                    await GetCatalogingProfileWithProfileIds(ct, organId, agenceyId, planId);
                                foreach (var catalogingProfileWithProfileId in catalogingProfileWithProfileIds)
                                {
                                    if (catalogingProfileWithProfileId.Key > 0 && catalogingProfileWithProfileId.Value > 0)
                                    {
                                        var idCatalogingProfile = catalogingProfileWithProfileId.Key;
                                        var idPlanProfile = catalogingProfileWithProfileId.Value;
                                        var docCollect = await GetDocCompletes(ct, idPlanProfile);

                                        // add CatalogingDoc
                                        foreach (var doc in docCollect)
                                        {
                                            listCatalogingDoc.Add(new CatalogingDoc
                                            {
                                                IDChannel = doc.IDChannel,
                                                IDFile = doc.IDFile,
                                                IDDoc = doc.ID,
                                                IDCatalogingProfile = idCatalogingProfile,
                                                IDDocType = doc.IDDocType,
                                                Status = doc.Status,
                                                IsPublic = true
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (listCatalogingDoc.Any())
                    {
                        await ct.BulkInsertOrUpdateAsync(listCatalogingDoc, bulkConfig);
                    }

                    // 2. Insert CataloginDocField
                    var catalogingDocWithDocIds = await GetCatalogingDocWithDocIds(ct);
                    foreach (var catalogingDocWithDocId in catalogingDocWithDocIds)
                    {
                        if (catalogingDocWithDocId.Key > 0 && catalogingDocWithDocId.Value > 0)
                        {
                            var idCatalogingDoc = catalogingDocWithDocId.Key;
                            var idDoc = catalogingDocWithDocId.Value;

                            // add CatalogingDocField
                            var docFields = await GetDocFields(ct, idDoc);
                            foreach (var docField in docFields)
                            {
                                listCatalogingDocField.Add(new CatalogingDocField
                                {
                                    IDChannel = docField.IDChannel,
                                    IDCatalogingDoc = idCatalogingDoc,
                                    IDDocTypeField = docField.IDDocTypeField,
                                    Value = docField.Value,
                                    Status = docField.Status
                                });
                            }
                        }
                    }

                    if (listCatalogingDocField.Any())
                    {
                        await ct.BulkInsertOrUpdateAsync(listCatalogingDocField, bulkConfig);
                    }
                }
            }
        }

        #region Random function

        private static int RandomInt(int min, int max)
        {
            var rnd = new Random();
            return rnd.Next(min, max);
        }

        private static double RandomDouble(double min, double max)
        {
            var rnd = new Random();
            return rnd.NextDouble() * (max - min) + min;
        }

        private static DateTime RandomDateTime(DateTime startDate, DateTime endDate)
        {
            var rnd = new Random();
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, rnd.Next(0, (int)timeSpan.TotalMinutes), 0);
            return startDate + newSpan;
        }

        public static string RandomString(int length = 0)
        {
            var rnd = new Random();
            if (length == 0)
            {
                length = RandomInt(1, 250);
            }
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        #endregion

        private static async Task<List<int>> GetPositionIds(ESDContext ct)
        {
            return await ct.Position.Where(x => x.Status == 1).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetOrganIds(ESDContext ct)
        {
            return await ct.Organ.Where(x => x.Status == 1).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetAgencyIds(ESDContext ct, int idOrgan = 0)
        {
            return await ct.Agency.Where(x => x.Status == 1 && (x.IDOrgan == idOrgan || idOrgan == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetTeamIds(ESDContext ct)
        {
            return await ct.Team.Where(x => x.Status == 1).Select(x => x.ID).ToListAsync();
        }

        #region PlanProfile

        private static async Task<List<int>> GetPlanIds(ESDContext ct, int idOrgan = 0)
        {
            return await ct.Plan.Where(x => x.Status != (int)EnumPlan.Status.InActive && (x.IDOrgan == idOrgan || idOrgan == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetProfileTemplateIds(ESDContext ct, int idOrgan = 0)
        {
            return await ct.ProfileTemplate.Where(x => x.Status == (int)EnumProfileTemplate.Type.Open && (x.IDOrgan == idOrgan || idOrgan == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetCategoryIds(ESDContext ct, string codeType, int idOrgan = 0)
        {
            return await ct.Category.Where(x => x.Status == (int)EnumCategory.Status.Active && x.CodeType == codeType && (x.IDOrgan == idOrgan || idOrgan == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetExpiryDateIds(ESDContext ct)
        {
            return await ct.ExpiryDate.Where(x => x.Status == (int)EnumExpiryDate.Status.Active).Select(x => x.ID).ToListAsync();
        }

        #endregion

        #region Doc

        private static async Task<List<PlanProfile>> GetPlanProfileIds(ESDContext ct, int idOrgan = 0, int idAgency = 0, int idPlan = 0)
        {
            return await ct.PlanProfile
                .Where(x => x.Status != (int)EnumProfilePlan.Status.InActive && (x.IDOrgan == idOrgan || idOrgan == 0) &&
                            (x.IDAgency == idAgency || idAgency == 0) && (x.IDPlan == idPlan || idPlan == 0))
                .Select(x => new PlanProfile
                {
                    ID = x.ID,
                    Status = x.Status
                }).ToListAsync();
        }

        private static async Task<List<int>> GetDocTypeIds(ESDContext ct, int idOrgan = 0)
        {
            return await ct.DocType.Where(x => x.Status == (int)EnumExpiryDate.Status.Active && (x.IDOrgan == idOrgan || idOrgan == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<int>> GetDocIds(ESDContext ct, int idDocType = 0)
        {
            return await ct.Doc.Where(x => x.Status != (int)EnumDocCollect.Status.InActive && (x.IDDocType == idDocType || idDocType == 0)).Select(x => x.ID).ToListAsync();
        }

        private static async Task<List<DocTypeField>> GetDocTypeFields(ESDContext ct, int idOrgan = 0, int idDocType = 0)
        {
            return await ct.DocTypeField.Where(x => x.Status == (int)EnumCommon.Status.Active && (x.IDOrgan == idOrgan || idOrgan == 0) && (x.IDDocType == idDocType || idDocType == 0)).ToListAsync();
        }

        private static async Task<List<long>> GetDocFileIds(ESDContext ct, int idDocType = 0)
        {
            return await ct.Doc.Where(x => x.Status != (int)EnumDocCollect.Status.Complete && (x.IDDocType == idDocType || idDocType == 0) && x.IDFile > 0).Select(x => x.IDFile).ToListAsync();
        }

        #endregion

        #region CatalogingDoc

        private static async Task<List<PlanProfile>> GetPlanProfileArchiveApproveds(ESDContext ct, int idOrgan = 0, int idAgency = 0, int idPlan = 0)
        {
            return await ct.PlanProfile
                .Where(x => x.Status == (int)EnumProfilePlan.Status.ArchiveApproved && (x.IDOrgan == idOrgan || idOrgan == 0) &&
                            (x.IDAgency == idAgency || idAgency == 0) && (x.IDPlan == idPlan || idPlan == 0))
                .Select(x => x).ToListAsync();
        }

        private static async Task<Dictionary<int, int>> GetCatalogingProfileWithProfileIds(ESDContext ct, int idOrgan = 0, int idAgency = 0, int idPlan = 0)
        {
            return await ct.CatalogingProfile
                .Where(x => x.Status != (int)EnumCataloging.Status.InActive && x.Status != (int)EnumCataloging.Status.Approved && x.Status != (int)EnumCataloging.Status.Reject && x.Status != (int)EnumCataloging.Status.CollectComplete &&
                (x.IDOrgan == idOrgan || idOrgan == 0) && (x.IDAgency == idAgency || idAgency == 0) && (x.IDPlan == idPlan || idPlan == 0))
                .Select(x => new KeyValuePair<int, int>(x.ID, x.IDPlanProfile))
                .ToDictionaryAsync(x => x.Key, x => x.Value);
        }

        private static async Task<List<Doc>> GetDocCompletes(ESDContext ct, int idProfile = 0)
        {
            return await ct.Doc.Where(x => x.Status == (int)EnumDocCollect.Status.Complete && (x.IDDocType == idProfile || idProfile == 0)).ToListAsync();
        }

        private static async Task<Dictionary<int, int>> GetCatalogingDocWithDocIds(ESDContext ct, int idCatalogingProfile = 0)
        {
            return await ct.CatalogingDoc
                .Where(x => x.Status != (int)EnumDocCollect.Status.InActive &&
                            (x.IDCatalogingProfile == idCatalogingProfile || idCatalogingProfile == 0))
                .Select(x => new KeyValuePair<int, int>(x.ID, (int)x.IDDoc))
                .ToDictionaryAsync(x => x.Key, x => x.Value);
        }

        private static async Task<List<DocField>> GetDocFields(ESDContext ct, int idDoc = 0)
        {
            return await ct.DocField.Where(x => x.Status == (int)EnumCommon.Status.Active && (x.IDDoc == idDoc || idDoc == 0)).ToListAsync();
        }

        #endregion
    }

    #region Getting random numbers from a list 

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }

    #endregion

}
