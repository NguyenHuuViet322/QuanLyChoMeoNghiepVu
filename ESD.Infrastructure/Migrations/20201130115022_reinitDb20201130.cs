using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ESD.Infrastructure.Migrations
{
    public partial class reinitDb20201130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agency",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Fax = table.Column<string>(maxLength: 20, nullable: true),
                    IsArchive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IdCategoryType = table.Column<int>(nullable: false),
                    CodeType = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    ParentPath = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CategoryField",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDCategory = table.Column<int>(nullable: false),
                    IDCategoryTypeField = table.Column<int>(nullable: false),
                    StringVal = table.Column<string>(nullable: true),
                    DateTimeVal = table.Column<DateTime>(nullable: true),
                    IntVal = table.Column<int>(nullable: true),
                    FloatVal = table.Column<float>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryField", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CategoryType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsConfig = table.Column<bool>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    ParentPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CategoryTypeField",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDCategoryType = table.Column<int>(nullable: false),
                    InputType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 250, nullable: true),
                    Format = table.Column<string>(maxLength: 30, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDCategoryTypeRelated = table.Column<int>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    IsRequire = table.Column<bool>(nullable: false),
                    IsShowGrid = table.Column<bool>(nullable: false),
                    IsSearchGrid = table.Column<bool>(nullable: false),
                    Minlenght = table.Column<int>(nullable: false),
                    Maxlenght = table.Column<int>(nullable: false),
                    MaxValue = table.Column<int>(nullable: true),
                    MinValue = table.Column<int>(nullable: true),
                    IsReadonly = table.Column<bool>(nullable: false),
                    DefaultValueType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTypeField", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CodeBox",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    FromCode = table.Column<int>(nullable: false),
                    ToCodeCode = table.Column<int>(nullable: false),
                    IDProfileList = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeBox", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DataType",
                columns: table => new
                {
                    ID = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDAgency = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Doc",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDFile = table.Column<long>(nullable: false),
                    DocCode = table.Column<string>(maxLength: 25, nullable: true),
                    IDProfile = table.Column<int>(nullable: false),
                    Identifier = table.Column<string>(maxLength: 13, nullable: true),
                    IDProfileTemplate = table.Column<int>(nullable: false),
                    FileCatalog = table.Column<int>(maxLength: 4, nullable: false),
                    FileNotation = table.Column<string>(maxLength: 20, nullable: true),
                    IDDocType = table.Column<int>(nullable: false),
                    CodeNumber = table.Column<string>(maxLength: 11, nullable: true),
                    CodeNotation = table.Column<string>(maxLength: 30, nullable: true),
                    IssuedDate = table.Column<DateTime>(nullable: false),
                    AgencyName = table.Column<string>(maxLength: 200, nullable: true),
                    Subject = table.Column<string>(maxLength: 200, nullable: true),
                    Language = table.Column<int>(nullable: false),
                    PageAmount = table.Column<int>(maxLength: 4, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    InforSign = table.Column<string>(maxLength: 30, nullable: true),
                    Keyword = table.Column<string>(maxLength: 100, nullable: true),
                    Mode = table.Column<string>(maxLength: 20, nullable: true),
                    ConfidenceLevel = table.Column<string>(maxLength: 30, nullable: true),
                    Autograph = table.Column<string>(maxLength: 2000, nullable: true),
                    Format = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doc", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DocField",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDDoc = table.Column<int>(nullable: false),
                    IDDocTypeField = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocField", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DocFieldValue",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    Value = table.Column<string>(maxLength: 1024, nullable: false),
                    IDDocField = table.Column<int>(nullable: false),
                    IDDoc = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocFieldValue", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DocType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DocTypeField",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDDocType = table.Column<int>(nullable: false),
                    InputType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 250, nullable: true),
                    Format = table.Column<string>(maxLength: 30, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDCategoryTypeRelated = table.Column<int>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    IsRequire = table.Column<bool>(nullable: false),
                    IsShowGrid = table.Column<bool>(nullable: false),
                    IsSearchGrid = table.Column<bool>(nullable: false),
                    Minlenght = table.Column<int>(nullable: false),
                    Maxlenght = table.Column<int>(nullable: false),
                    MaxValue = table.Column<int>(nullable: true),
                    MinValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocTypeField", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    FromEmail = table.Column<string>(maxLength: 50, nullable: false),
                    ToEmail = table.Column<string>(maxLength: 1000, nullable: true),
                    Content = table.Column<string>(maxLength: 1000, nullable: true),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    EmailType = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExpiryDate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiryDate", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermission",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermission", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: false),
                    ParentPath = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Url = table.Column<string>(maxLength: 250, nullable: true),
                    Icon = table.Column<string>(maxLength: 30, nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ModuleChild",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDModule = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleChild", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDModuleChild = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PermissionGroupPer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDPermission = table.Column<int>(nullable: false),
                    IDGroupPermission = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGroupPer", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Parent = table.Column<int>(nullable: false),
                    ParentPath = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Code = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    FileCode = table.Column<string>(maxLength: 13, nullable: true),
                    IDStorage = table.Column<int>(nullable: false),
                    IDCodeBox = table.Column<int>(nullable: false),
                    IDProfileList = table.Column<int>(nullable: false),
                    IDSecurityLevel = table.Column<int>(nullable: false),
                    Identifier = table.Column<string>(maxLength: 13, nullable: true),
                    IDProfileTemplate = table.Column<int>(nullable: false),
                    FileCatalog = table.Column<int>(maxLength: 4, nullable: false),
                    FileNotation = table.Column<string>(maxLength: 20, nullable: true),
                    Title = table.Column<string>(maxLength: 1000, nullable: true),
                    IDExpiryDate = table.Column<int>(nullable: false),
                    Rights = table.Column<string>(maxLength: 30, nullable: true),
                    Language = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TotalDoc = table.Column<int>(maxLength: 10, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    InforSign = table.Column<string>(maxLength: 30, nullable: true),
                    Keyword = table.Column<string>(maxLength: 100, nullable: true),
                    Maintenance = table.Column<int>(maxLength: 10, nullable: false),
                    PageNumber = table.Column<int>(maxLength: 10, nullable: false),
                    Format = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProfileList",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    IDStorage = table.Column<int>(nullable: false),
                    IDProfileTemplate = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileList", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDAgency = table.Column<int>(nullable: false),
                    IDStorage = table.Column<int>(nullable: false),
                    Identifier = table.Column<string>(maxLength: 13, nullable: true),
                    Agencyld = table.Column<string>(maxLength: 13, nullable: true),
                    FondName = table.Column<string>(maxLength: 200, nullable: false),
                    FondHistory = table.Column<string>(nullable: true),
                    ArchivesTime = table.Column<string>(maxLength: 30, nullable: true),
                    PaperTotal = table.Column<int>(maxLength: 10, nullable: false),
                    PaperDigital = table.Column<int>(maxLength: 10, nullable: false),
                    KeyGroups = table.Column<string>(maxLength: 300, nullable: true),
                    OtherTypes = table.Column<string>(maxLength: 300, nullable: true),
                    Language = table.Column<int>(nullable: false),
                    LookupTools = table.Column<string>(maxLength: 50, nullable: true),
                    CopyNumber = table.Column<int>(maxLength: 10, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    InforSign = table.Column<string>(maxLength: 30, nullable: true),
                    Keyword = table.Column<string>(maxLength: 100, nullable: true),
                    Mode = table.Column<string>(maxLength: 20, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTemplate", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ResetPassword",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 20, nullable: true),
                    ExpiredTime = table.Column<DateTime>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPassword", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoleGroupPer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDRole = table.Column<int>(nullable: false),
                    IDGroupPermission = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroupPer", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SercureLevel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SercureLevel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StgFile",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 250, nullable: true),
                    PhysicalPath = table.Column<string>(maxLength: 1000, nullable: true),
                    FileType = table.Column<int>(nullable: false),
                    Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsTemp = table.Column<bool>(nullable: false),
                    IsEncrypted = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StgFile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    PhysicalLocation = table.Column<string>(maxLength: 250, nullable: true),
                    IDAgency = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storage", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TeamGroupPer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDTeam = table.Column<int>(nullable: false),
                    IDGroupPer = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGroupPer", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TeamRole",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDTeam = table.Column<int>(nullable: false),
                    IDRole = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRole", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDPosition = table.Column<int>(nullable: true),
                    IDDepartment = table.Column<int>(nullable: true),
                    IDAgency = table.Column<int>(nullable: true),
                    AccountName = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: true),
                    IdentityNumber = table.Column<string>(maxLength: 20, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Birthday = table.Column<DateTime>(nullable: true),
                    Birthplace = table.Column<string>(maxLength: 250, nullable: true),
                    Address = table.Column<string>(maxLength: 250, nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IDTeam = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupPer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDUser = table.Column<int>(nullable: false),
                    IDGroupPer = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupPer", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChannel = table.Column<int>(nullable: false),
                    IDUser = table.Column<int>(nullable: false),
                    IDRole = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTeam",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    IDChannel = table.Column<int>(nullable: false),
                    IDTeam = table.Column<int>(nullable: false),
                    IDUser = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeam", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agency");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "CategoryField");

            migrationBuilder.DropTable(
                name: "CategoryType");

            migrationBuilder.DropTable(
                name: "CategoryTypeField");

            migrationBuilder.DropTable(
                name: "CodeBox");

            migrationBuilder.DropTable(
                name: "DataType");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Doc");

            migrationBuilder.DropTable(
                name: "DocField");

            migrationBuilder.DropTable(
                name: "DocFieldValue");

            migrationBuilder.DropTable(
                name: "DocType");

            migrationBuilder.DropTable(
                name: "DocTypeField");

            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "ExpiryDate");

            migrationBuilder.DropTable(
                name: "GroupPermission");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "ModuleChild");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "PermissionGroupPer");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "ProfileList");

            migrationBuilder.DropTable(
                name: "ProfileTemplate");

            migrationBuilder.DropTable(
                name: "ResetPassword");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "RoleGroupPer");

            migrationBuilder.DropTable(
                name: "SercureLevel");

            migrationBuilder.DropTable(
                name: "StgFile");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "TeamGroupPer");

            migrationBuilder.DropTable(
                name: "TeamRole");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserGroupPer");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserTeam");
        }
    }
}
