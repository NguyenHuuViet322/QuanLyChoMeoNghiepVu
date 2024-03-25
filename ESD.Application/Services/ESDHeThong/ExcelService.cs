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
using Newtonsoft.Json;
using ESD.Domain.Enums;
using ClosedXML.Excel;
using System.Collections;
using System.Globalization;
using System.Dynamic;
using System.IO;
using ESD.Utility;
using DocumentFormat.OpenXml.Bibliography;

namespace ESD.Application.Services
{
    public class ExcelService : IExcelServices
    {
        #region Export
        public async Task<ServiceResult> ExportExcel(ExportExtend exportExtend, string sheetName, bool isAdjust = true)
        {
            var myWorkBook = new XLWorkbook();
            myWorkBook.AddWorksheet();
            var myWorkSheet = myWorkBook.Worksheet(1);
            try
            {
                myWorkSheet.Name = sheetName;
                using (var ms = new MemoryStream())
                {
                    int rowstart = 1;
                    int colstart = 1;
                    int index = 0;

                    //Header
                    var headers = exportExtend.Headers;
                    foreach (var item in headers)
                    {
                        //Name
                        myWorkSheet.Cell(rowstart, colstart + index).Value = string.IsNullOrEmpty(item.Name) ? "" : item.Name;
                        //Background
                        if (!string.IsNullOrEmpty(item.BackGround))
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(item.BackGround);
                        //Color
                        if (!string.IsNullOrEmpty(item.Color))
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(item.Color);
                        //Font
                        if (!string.IsNullOrEmpty(item.Font))
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = item.Font;
                        //IsBold
                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = item.IsBold;
                        //IsItalic
                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = item.IsItalic;
                        //Size
                        if (!string.IsNullOrEmpty(item.Size.ToString()))
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = item.Size;
                        if (item.IsBorder)
                        {
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        myWorkSheet.Column(colstart + index).Width = item.Width;
                        index++;
                    }
                    //Data
                    rowstart = 2;
                    colstart = 1;
                    int stt = 1;
                    IList list = exportExtend.Data;
                    foreach (var item in list)
                    {
                        index = 0;
                        var type = item.GetType();
                        if (type.Name != "ExpandoObject") //Danh mục tĩnh
                        {
                            //DM Thương                        
                            foreach (var col in exportExtend.Cols)
                            {
                                var columnName = col.Field;
                                if (type.GetProperty(columnName ?? "") != null)
                                {
                                    var value = type.GetProperty(columnName).GetValue(item, null);
                                    //1.Value
                                    if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                        myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                    }
                                    else
                                    {
                                        if (col.DataType == (int)XLDataType.Boolean)
                                        {
                                            if (value is bool || value is bool?)
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                            }
                                            else
                                            {
                                                if (col.DefineEnum != null) //Có define Enum
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                }
                                                else
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                }

                                            }
                                        }
                                        else
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                        }
                                    }
                                }
                                else
                                {
                                    if (col.DataType == 5)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                        myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                        stt++;
                                    }
                                    else
                                    {
                                        var value = Utils.GetString(Utils.KeyValue(item), columnName);
                                        myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                        myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                    }
                                }
                                //2.Format
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                if (!string.IsNullOrEmpty(col.Color))
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                if (!string.IsNullOrEmpty(col.BackGround))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                }
                                if (!string.IsNullOrEmpty(col.Font))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                }
                                if (!string.IsNullOrEmpty(col.Size.ToString()))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                }
                                if (col.IsBorder)
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                }
                                index++;
                            }
                            rowstart++;
                        }
                        else //Danh mục động
                        {

                            foreach (var col in exportExtend.Cols)
                            {
                                var columnName = col.Field;
                                if (!string.IsNullOrEmpty(columnName))
                                {
                                    var value = ((IDictionary<string, object>)item)[columnName];
                                    if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                        myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                    }
                                    else
                                    {
                                        if (col.DataType == (int)XLDataType.Boolean)
                                        {
                                            if (value is bool || value is bool?)
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                            }
                                            else
                                            {
                                                if (col.DefineEnum != null) //Có define Enum
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                }
                                                else
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                        }
                                    }
                                }
                                else
                                {
                                    if (col.DataType == 5)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                        myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                        stt++;
                                    }
                                }
                                //2.Format
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                if (!string.IsNullOrEmpty(col.Color))
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                if (!string.IsNullOrEmpty(col.BackGround))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                }
                                if (!string.IsNullOrEmpty(col.Font))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                }
                                if (!string.IsNullOrEmpty(col.Size.ToString()))
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                }
                                if (col.IsBorder)
                                {
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                }
                                index++;
                            }
                            rowstart++;
                        }


                    }
                    if (isAdjust)
                    {
                        myWorkSheet.Columns().AdjustToContents();
                    }
                    myWorkBook.SaveAs(ms);
                    var workBookBytes = ms.ToArray();

                    var rs = new ServiceResultSuccess("Export Successful");
                    rs.Data = workBookBytes;

                    return rs;
                }


            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> ExportExcelCus(ExportExtend2 exportExtend, string sheetName, bool isAdjust = true)
        {
            int rowstart = 1;
            int colstart = 1;
            var myWorkBook = new XLWorkbook();
            if (File.Exists(exportExtend.Template))
            {
                myWorkBook = new XLWorkbook(exportExtend.Template);
            }
            else
            {
                myWorkBook.AddWorksheet();
            }
            var myWorkSheet = myWorkBook.Worksheet(1);
            try
            {
                myWorkSheet.Name = sheetName;
                using (var ms = new MemoryStream())
                {
                    int index = 0;

                    //Header
                    if (exportExtend.IsCreateHeader)
                    {

                        var headers = exportExtend.Headers;
                        foreach (var item in headers)
                        {
                            //Name
                            myWorkSheet.Cell(rowstart, colstart + index).Value = string.IsNullOrEmpty(item.Name) ? "" : item.Name;
                            //Background
                            if (!string.IsNullOrEmpty(item.BackGround))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(item.BackGround);
                            }
                            //Color
                            if (!string.IsNullOrEmpty(item.Color))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(item.Color);
                            }
                            //Font
                            if (!string.IsNullOrEmpty(item.Font))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = item.Font;
                            }
                            //IsBold
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = item.IsBold;
                            //IsItalic
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = item.IsItalic;
                            //Size
                            if (!string.IsNullOrEmpty(item.Size.ToString()))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = item.Size;
                            }

                            if (item.IsBorder)
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            myWorkSheet.Column(colstart + index).Width = item.Width;
                            index++;
                        }
                    }

                    //Data
                    rowstart = exportExtend.RowStart == 0 ? 2 : exportExtend.RowStart;
                    colstart = exportExtend.ColStart == 0 ? 1 : exportExtend.ColStart;
                    int stt = 1;
                    var limit = 5000; // tạm thòi giói hạn xuất 5000 bàn ghi cho 1 lần xuất;

                    IList list = exportExtend.Data;
                    var count = list.Count;
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (stt > limit)
                                break;
                            index = 0;
                            var type = item.GetType();
                            if (type.Name != "ExpandoObject") //Danh mục tĩnh
                            {
                                //DM Thương                        
                                foreach (var col in exportExtend.Cols)
                                {
                                    var columnName = col.Field;
                                    if (type.GetProperty(columnName ?? "") != null)
                                    {
                                        var value = type.GetProperty(columnName).GetValue(item, null);
                                        //1.Value
                                        if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            if (col.DataType == (int)XLDataType.Boolean)
                                            {
                                                if (value is bool || value is bool?)
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                                }
                                                else
                                                {
                                                    if (col.DefineEnum != null) //Có define Enum
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                    }
                                                    else
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                                myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (col.DataType == 5)
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                            myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            stt++;
                                        }
                                    }
                                    //2.Format
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                    if (!string.IsNullOrEmpty(col.Color))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                    }

                                    if (!string.IsNullOrEmpty(col.BackGround))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                    }
                                    if (!string.IsNullOrEmpty(col.Font))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                    }
                                    if (!string.IsNullOrEmpty(col.Size.ToString()))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                    }
                                    if (col.IsBorder)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    }
                                    index++;
                                }
                                rowstart++;
                            }
                            else //Danh mục động
                            {

                                foreach (var col in exportExtend.Cols)
                                {
                                    var columnName = col.Field;
                                    if (!string.IsNullOrEmpty(columnName))
                                    {
                                        var value = ((IDictionary<string, object>)item)[columnName];
                                        if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            if (col.DataType == (int)XLDataType.Boolean)
                                            {
                                                if (value is bool || value is bool?)
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                                }
                                                else
                                                {
                                                    if (col.DefineEnum != null) //Có define Enum
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                    }
                                                    else
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                                myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (col.DataType == 5)
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                            myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            stt++;
                                        }
                                    }
                                    //2.Format
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                    if (!string.IsNullOrEmpty(col.Color))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                    }

                                    if (!string.IsNullOrEmpty(col.BackGround))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                    }
                                    if (!string.IsNullOrEmpty(col.Font))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                    }
                                    if (!string.IsNullOrEmpty(col.Size.ToString()))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                    }
                                    if (col.IsBorder)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    }
                                    index++;
                                }
                                rowstart++;
                            }


                        }
                    }

                    if (isAdjust)
                    {
                        myWorkSheet.Columns().AdjustToContents();
                    }
                    myWorkBook.SaveAs(ms);
                    var workBookBytes = ms.ToArray();

                    var rs = new ServiceResultSuccess("Export Successful");
                    if (count > limit)
                    {
                        rs = new ServiceResultSuccess($"Dũ liệu xuất giói hạn  {0} bảng chi đầu, xuất dũ liệu excel thành công", limit);
                    }
                    rs.Data = workBookBytes;

                    return rs;
                }


            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }

        public async Task<ServiceResult> ExportExcelCus2(ExportExtend3 exportExtend, string sheetName, bool isAdjust = true)
        {
            int rowstart = exportExtend.RowStart;
            int colstart = exportExtend.ColStart;
            var myWorkBook = new XLWorkbook();
            if (File.Exists(exportExtend.Template))
            {
                myWorkBook = new XLWorkbook(exportExtend.Template);
            }
            else
            {
                myWorkBook.AddWorksheet();
            }
            var myWorkSheet = myWorkBook.Worksheet(1);
            try
            {
                myWorkSheet.Name = sheetName;
                using (var ms = new MemoryStream())
                {
                    int index = 0;

                    myWorkSheet.Range(myWorkSheet.Cell(rowstart, colstart), myWorkSheet.Cell(rowstart, colstart + 4)).Merge();
                    myWorkSheet.Cell(rowstart, colstart).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    myWorkSheet.Cell(rowstart, colstart).Value = exportExtend.title;
                    myWorkSheet.Cell(rowstart, colstart).Style.Font.Bold = true;
                    myWorkSheet.Cell(rowstart, colstart).Style.Font.FontSize = 12;
                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = "Times New Roman";

                    rowstart += 2;

                    myWorkSheet.Cell(rowstart, colstart).Value = "Đơn vị";
                    myWorkSheet.Cell(rowstart, colstart).Style.Font.Bold = true;
                    myWorkSheet.Cell(rowstart, colstart).Style.Font.FontSize = 12;
                    myWorkSheet.Cell(rowstart, colstart).Style.Font.FontName = "Times New Roman";


                    if (exportExtend.description.Count() > 0)
                        foreach (var item in exportExtend.description)
                        {
                            myWorkSheet.Cell(rowstart, colstart + 1).Value = item.Ten;
                            myWorkSheet.Cell(rowstart, colstart + 1).Style.Font.Bold = true;
                            myWorkSheet.Cell(rowstart, colstart + 1).Style.Font.FontSize = 12;
                            myWorkSheet.Cell(rowstart, colstart + 1).Style.Font.FontName = "Times New Roman";
                            rowstart++;
                        }

                    rowstart++;

                    //Header
                    if (exportExtend.IsCreateHeader)
                    {

                        var headers = exportExtend.Headers;
                        foreach (var item in headers)
                        {
                            //Name
                            myWorkSheet.Cell(rowstart, colstart + index).Value = string.IsNullOrEmpty(item.Name) ? "" : item.Name;
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            //Background
                            if (!string.IsNullOrEmpty(item.BackGround))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(item.BackGround);
                            }
                            //Color
                            if (!string.IsNullOrEmpty(item.Color))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(item.Color);
                            }
                            //Font
                            if (!string.IsNullOrEmpty(item.Font))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = item.Font;
                            }
                            //IsBold
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = item.IsBold;
                            //IsItalic
                            myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = item.IsItalic;
                            //Size
                            if (!string.IsNullOrEmpty(item.Size.ToString()))
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = item.Size;
                            }

                            if (item.IsBorder)
                            {
                                myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            index++;
                        }
                    }

                    //Data
                    rowstart++;
                    int stt = 1;
                    var limit = 5000; // tạm thòi giói hạn xuất 5000 bàn ghi cho 1 lần xuất;

                    IList list = exportExtend.Data;
                    var count = list.Count;
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (stt > limit)
                                break;
                            index = 0;
                            var type = item.GetType();

                            if (type.Name != "ExpandoObject") //Danh mục tĩnh
                            {
                                //DM Thương                        
                                foreach (var col in exportExtend.Cols)
                                {
                                    var columnName = col.Field;
                                    if (col.DataType != 5)
                                    {
                                        var test = type.GetProperty(columnName);
                                        var value = type.GetProperty(columnName).GetValue(item, null);

                                        //1.Value
                                        if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            if (col.DataType == (int)XLDataType.Boolean)
                                            {
                                                if (value is bool || value is bool?)
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                                }
                                                else
                                                {
                                                    if (col.DefineEnum != null) //Có define Enum
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                                    }
                                                    else
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                                myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (col.DataType == 5)
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                            myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            stt++;
                                        }
                                    }
                                    //2.Format
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                    if (!string.IsNullOrEmpty(col.Color))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                    }

                                    if (!string.IsNullOrEmpty(col.BackGround))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                    }
                                    if (!string.IsNullOrEmpty(col.Font))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                    }
                                    if (!string.IsNullOrEmpty(col.Size.ToString()))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                    }
                                    if (col.IsBorder)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    }
                                    index++;
                                }
                                rowstart++;
                            }
                            else //Danh mục động
                            {

                                foreach (var col in exportExtend.Cols)
                                {
                                    var columnName = col.Field;
                                    if (!string.IsNullOrEmpty(columnName))
                                    {
                                        var value = ((IDictionary<string, object>)item)[columnName];
                                        if (col.DataType == (int)XLDataType.DateTime && (value is DateTime || value is DateTime?))
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                        }
                                        else
                                        {
                                            if (col.DataType == (int)XLDataType.Boolean)
                                            {
                                                if (value is bool || value is bool?)
                                                {
                                                    myWorkSheet.Cell(rowstart, colstart + index).Value = (bool)value ? "Kích hoạt" : "Không kích hoạt";
                                                }
                                                else
                                                {
                                                    if (col.DefineEnum != null) //Có define Enum
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (string)col.DefineEnum[(int)value];
                                                    }
                                                    else
                                                    {
                                                        myWorkSheet.Cell(rowstart, colstart + index).Value = (int)value != 0 ? "Kích hoạt" : "Không kích hoạt";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                                myWorkSheet.Cell(rowstart, colstart + index).SetValue<string>(Convert.ToString(value));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (col.DataType == 5)
                                        {
                                            myWorkSheet.Cell(rowstart, colstart + index).DataType = XLDataType.Text;
                                            myWorkSheet.Cell(rowstart, colstart + index).Value = stt.ToString();
                                            myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            stt++;
                                        }
                                    }
                                    //2.Format
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Alignment.WrapText = col.isWrapText;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Bold = col.IsBold;
                                    myWorkSheet.Cell(rowstart, colstart + index).Style.Font.Italic = col.IsItalic;
                                    if (!string.IsNullOrEmpty(col.Color))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontColor = XLColor.FromName(col.Color);
                                    }

                                    if (!string.IsNullOrEmpty(col.BackGround))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Fill.BackgroundColor = XLColor.FromName(col.BackGround);
                                    }
                                    if (!string.IsNullOrEmpty(col.Font))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontName = col.Font;
                                    }
                                    if (!string.IsNullOrEmpty(col.Size.ToString()))
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Font.FontSize = col.Size;
                                    }
                                    if (col.IsBorder)
                                    {
                                        myWorkSheet.Cell(rowstart, colstart + index).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    }
                                    index++;
                                }
                                rowstart++;
                            }


                        }
                    }

                    if (isAdjust)
                    {
                        myWorkSheet.Columns().AdjustToContents();
                    }
                    colstart = 1; index = 0;
                    foreach (var item in exportExtend.Headers)
                    {
                        //Name
                        myWorkSheet.Column(colstart + index).Width = item.Width;
                        index++;
                    }
                    myWorkBook.SaveAs(ms);
                    var workBookBytes = ms.ToArray();

                    var rs = new ServiceResultSuccess("Export Successful");
                    if (count > limit)
                    {
                        rs = new ServiceResultSuccess($"Dũ liệu xuất giói hạn  {0} bảng chi đầu, xuất dũ liệu excel thành công", limit);
                    }
                    rs.Data = workBookBytes;

                    return rs;
                }


            }
            catch (Exception ex)
            {
                return new ServiceResultError(ex.Message);
            }
        }
        #endregion
    }
}
