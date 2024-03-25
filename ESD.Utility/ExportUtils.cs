using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xceed.Words.NET;

namespace ESD.Utility
{

    public static class ExportUtils
    {
        public static string ExportWord(string tempFilePath, Dictionary<string, string> dic, string ftmp, Dictionary<string, Dictionary<long, Dictionary<string, string>>> dicTable = null)
        {
            string path = Directory.GetCurrentDirectory();
            tempFilePath = (path + "\\wwwroot\\templates\\" + tempFilePath);
            ftmp = DateTime.Now.ToString("ddMMyyyssmmhh");
            string file = Path.GetFileNameWithoutExtension(tempFilePath);
            string down = tempFilePath.Replace(file, file + "-" + ftmp).Replace("templates", "Download");


            Directory.CreateDirectory(Path.GetDirectoryName(down));

            DocX doc = DocX.Load(tempFilePath);
            foreach (var item in dic)
            {
                doc.ReplaceText(item.Key, item.Value ?? string.Empty);
            }

            if (Utils.IsNotEmpty(dicTable) && dicTable.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, Dictionary<long, Dictionary<string, string>>> table in dicTable)
                {
                    var placeholderTable = doc.Tables.FirstOrDefault(t => t.TableCaption == table.Key);
                    var content = new Dictionary<long, Dictionary<string, string>>();
                    if (Utils.IsNotEmpty(placeholderTable))
                    {
                        content = table.Value;
                    }

                    var baseFont = new Xceed.Document.NET.Font("Times New Roman");
                    if (Utils.IsNotEmpty(content) && content.Keys.Count > 0)
                    {
                        
                        var i = 1;
                        var rowstart = 0;
                        // check row start
                        bool isrow = true;
                        try
                        {
                            foreach (var item in placeholderTable.Rows)
                            {
                                isrow = true;
                                foreach (var cell in item.Cells)
                                {
                                    var value = cell.Paragraphs.First().Text;
                                    isrow = isrow && string.IsNullOrEmpty(value);
                                }
                                if (isrow)
                                {
                                    break;
                                }
                                rowstart++;
                            }
                        }
                        catch
                        {
                        }
                        var totalrow = placeholderTable.RowCount;
                        var isRemoveRow = true;
                        var rowPattern = placeholderTable.Rows[placeholderTable.RowCount - 1];
                        foreach (KeyValuePair<long, Dictionary<string, string>> item in content)
                        {
                            var newItem = rowPattern;
                            if (rowstart >= totalrow - 1)
                            {
                                newItem = placeholderTable.InsertRow(rowPattern, rowstart, true);
                            }
                            else
                            {
                                isRemoveRow = false;
                                newItem = placeholderTable.Rows[rowstart];
                            }
                            rowstart++;
                            var data = item.Value;
                            if (Utils.IsNotEmpty(data))
                            {
                                var j = 0;
                                foreach (KeyValuePair<string, string> rVal in data)
                                {
                                    try
                                    {
                                        newItem.Cells[j].Paragraphs.First().Append(rVal.Value).Font(baseFont).FontSize(12);
                                    }
                                    catch { }
                                    j++;
                                }
                            }
                            i++;
                        }
                        if (isRemoveRow)
                            rowPattern.Remove();
                    }
                    // placeholderTable.Remove();
                }
            }

            doc.SaveAs(down);
            return down;

        }



    }
}
