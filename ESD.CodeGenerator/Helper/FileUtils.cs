using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DAS.CodeGenerator.Helper
{
    public static class FileUtils
    {

        /// <summary>
        /// Thay thế nội dung vào file đã có theo vị trí đánh dấu
        /// </summary>
        /// <param name="updateFile">File cần update</param>
        /// <param name="contents">Nội dung cần thêm</param>
        /// <exception cref="Exception"></exception>
        public static void ReplaceContentFile(string updateFile, params string[] contents)
        {
            UpdateContenFile(false, updateFile, contents);
        }

        /// <summary>
        /// Thêm nội dung vào file đã có theo vị trí đánh dấu
        /// </summary>
        /// <param name="updateFile">File cần update</param>
        /// <param name="contents">Nội dung cần thêm</param>
        /// <exception cref="Exception"></exception>
        public static void AppendContentFile(string updateFile, params string[] contents)
        {
            UpdateContenFile(true, updateFile, contents);
        }


        #region Functions
        /// <summary>
        /// update nội dung vào file đã có theo vị trí đánh dấu
        /// </summary>
        /// <param name="isAppend">Thêm mới nội dung?</param>
        /// <param name="updateFile">File cần update</param>
        /// <param name="contents">Nội dung cần thêm</param>
        /// <exception cref="Exception"></exception>
        private static void UpdateContenFile(bool isAppend, string updateFile, params string[] contents)
        {
            if (contents == null || contents.Length == 0)
                return;
            if (!File.Exists(updateFile))
            {
                throw new Exception($"Không tìm thấy file: {updateFile}");
            }

            var lines = File.ReadAllLines(updateFile);

            try
            {
                using (StreamWriter w = new StreamWriter(updateFile))
                {
                    var indexRender = 0;
                    //Add lại từng dòng
                    for (var i = 0; i < lines.Length; i += 1)
                    {
                        var line = lines[i];

                        if (line.Trim().Contains(Const.KeyToRender))
                        {
                            var content = contents.ElementAtOrDefault(indexRender) ?? string.Empty;
                            if (line.Trim() == Const.KeyToRender || line.Trim() == $"{Const.KeyToRender}{indexRender + 1}")
                            {
                                if (isAppend)
                                {
                                    //Tránh thêm trùng
                                    if (!lines.Any(n => n.ContainsIgnoreCase(content.Trim())))
                                        //Render nội dung mói vào vị trí
                                        w.WriteLine(content);
                                }
                                else
                                {
                                    w.WriteLine(content);
                                }
                                if (isAppend)
                                    w.WriteLine($"       {line.Trim()}"); //Thêm lại key
                            }
                            indexRender++;

                        }
                        else
                        {
                            if (i + 1 == lines.Length)
                                w.Write(line); //Dòng cuối ko xuống dòng
                            else
                                w.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }

        }

        #endregion
    }
}
