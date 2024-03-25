using ESD.Application.Models.CustomModels;
using ESD.Application.Models.ViewModels;
using ESD.Domain.Models.DAS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ESD.Application.Interfaces
{
    public interface IExcelServices
    {
        Task<ServiceResult> ExportExcel(ExportExtend excelExtend, string sheetName, bool isAdjust = true);
        Task<ServiceResult> ExportExcelCus(ExportExtend2 exportExtend, string sheetName, bool isAdjust = true);

        Task<ServiceResult> ExportExcelCus2(ExportExtend3 exportExtend, string sheetName, bool isAdjust = true);
    }
}
