using DAS.CodeGenerator.Helper;
using DAS.CodeGenerator.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DAS.CodeGenerator
{
    public partial class Form1 : Form
    {
        static string Root = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
        //Đường dẫn chính của project
        static string tempRoot = Path.Combine(Root, Const.TempFolder); //Đường dẫn chính của thư mục code mẫu
        static string TableName = "";
        static string LowerTableName = "";
        static string IDDatatype = "int"; //Kiểu dữ liệu của cột id

        static string ProjectApplication = Utils.GetStringAppSettings("ProjectApplication");
        static string ProjectDomain = Utils.GetStringAppSettings("ProjectDomain");
        static string ProjectInfrastructure = Utils.GetStringAppSettings("ProjectInfrastructure");
        static string ProjectWeb = Utils.GetStringAppSettings("ProjectWeb");
        static string ProjectUtility = Utils.GetStringAppSettings("ProjectUtility");
        static string ContextFileName = Utils.GetStringAppSettings("ContextName"); //Lấy từ appsetting
        static string[] IgnoreUpdateFields = Utils.GetStringAppSettings("IgnoreUpdateFields")?.Split(",");

        public Form1()
        {

            InitializeComponent();
            var configuration = new ConfigurationBuilder();

            configuration.SetBasePath(Directory.GetCurrentDirectory());  // errors here
            configuration.AddJsonFile(path: Path.Combine(Root, "appsettings.json"), optional: false, reloadOnChange: true); // errors here
            configuration.Build();

            //rtbInput.Text = "ID\tID\tint\tfalse\r\n";
            rtbInput.Focus();

            //Set thưc mục gốc là mặc định 
            txtOutput.Text = Directory.GetParent(Root).Parent.FullName; //Utils.GetStringAppSettings("OutputDir"); //lấy từ appsetting
            //txtName.Text = "User";
            //txtDisplayname.Text = "Người dùng";
            txtGroup.Text = Utils.GetStringAppSettings("GroupName");
            txtRepos.Text = Utils.GetStringAppSettings("ReposName");
        }

        #region Events
        private void btnGenModel_Click(object sender, EventArgs e)
        {
            try
            {
                //Wrapper service: DAS.Application.Services.DIServiceWrapper  
                TableName = txtName.Text.Trim();
                LowerTableName = Utils.FirstCharToLowerCase(txtName.Text.Trim());

                if (cbIsController.Checked == false
                    && cbIsModel.Checked == false && cbIsService.Checked == false)
                {
                    AddLog("Hãy chọn ít nhất 1 tùy chọn");
                    return;
                }

                if (string.IsNullOrWhiteSpace(rtbInput.Text))
                {
                    rtbInput.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtOutput.Text))
                {
                    txtOutput.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    txtName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDisplayname.Text))
                {
                    txtDisplayname.Focus();
                    return;
                }

                var isGenerated = CheckIsGenerated();
                if (isGenerated)
                {
                    DialogResult dialogResult = MessageBox.Show($"Đã tồn tại code của bảng {TableName}, bạn có muốn ghi đè không?", "Xác nhận ghi đè", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        GenCode();
                    }
                    else
                    {
                        //Ignore
                    }
                }
                else
                {
                    GenCode();
                }

            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        private void btnClearOutput_Click(object sender, EventArgs e)
        {
            rtbOutput.Text = "";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //Get File
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);
                    txtOutput.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnCopySqlServerScript_Click(object sender, EventArgs e)
        {
            try
            {
                var fileScript = Path.Combine(Root, "Scripts", "SqlServer.sql");
                if (!File.Exists(fileScript))
                {
                    AddLog($"Không tìm thấy file {fileScript}");
                    return;
                }
                Clipboard.SetText(File.ReadAllText(fileScript));
                if (!string.IsNullOrEmpty(fileScript))
                    MessageBox.Show("Đã copy script SqlServer", "Thông báo", MessageBoxButtons.OK);
                else
                    AddLog("Script SQL Server trống, vui lòng kiểm tra lại");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        private void btnCopyOracleScript_Click(object sender, EventArgs e)
        {
            try
            {
                var fileScript = Path.Combine(Root, "Scripts", "Oracle.sql");
                if (!File.Exists(fileScript))
                {
                    AddLog($"Không tìm thấy file {fileScript}");
                    return;
                }

                Clipboard.SetText(File.ReadAllText(fileScript));
                if (!string.IsNullOrEmpty(fileScript))
                    MessageBox.Show("Đã copy script Oracle", "Thông báo", MessageBoxButtons.OK);
                else
                    AddLog("Script SQL Server trống, vui lòng kiểm tra lại");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }
        }

        #endregion

        #region Functions  
        private bool CheckIsGenerated()
        {
            var fileLog = Path.Combine(txtOutput.Text, "_nlog.txt");
            var itemLog = $"{txtGroup.Text}.{TableName}";
            if (File.Exists(fileLog))
            {
                var logs = File.ReadAllLines(fileLog);
                if (logs.Contains(itemLog))
                {
                    return true; //Đã gen
                }
            }
            return false;
        }
        private bool LogOutput()
        {
            var fileLog = Path.Combine(txtOutput.Text, "_nlog.txt");
            var itemLog = $"{txtGroup.Text}.{TableName}";
            if (File.Exists(fileLog))
            {
                using (var sw = new StreamWriter(fileLog, true))
                {
                    sw.WriteLine(itemLog);
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(fileLog))
                {
                    sw.WriteLine(itemLog);
                }
                File.SetAttributes(fileLog, File.GetAttributes(fileLog) | FileAttributes.Hidden);
            }

            return false;
        }
        private void GenCode()
        {
            var columns = Utils.GetTableColumns(rtbInput.Text);
            
            if(columns.Any(n=>n.Name.EqualIgnoreCase("ID")))
            {
                //set kiểu dữ liệu của cột id
                var idCol = columns.FirstOrDefault(n => n.Name.EqualIgnoreCase("ID"));
                IDDatatype = idCol.DataType; 
            }    
            if (cbIsModel.Checked)
            {
                DIRepositoryWrapper();

                var modelFile = CloneFromFile(true, ProjectDomain, "Models", "Temp.cs");
                GenModelToFile(modelFile, columns);

                CloneFromFile(true, ProjectDomain, "Interfaces", "ITempRepository.cs");
                CloneFromFile(true, ProjectInfrastructure, "Repositories", "TempRepository.cs");
            }
            if (cbIsService.Checked)
            {

                var file = CloneFromFile(true, ProjectApplication, "Models", "Param", "TempCondition.cs");
                GenParamToFile(file, columns);

                file = CloneFromFile(true, ProjectApplication, "Models", "ViewModels", "VMTemp.cs");
                GenModelViewToFile(file, columns);

                file = CloneFromFile(true, ProjectApplication, "Models", "ViewModels", "VMIndexTemp.cs");
                GenModelRefViewToFile(file, columns);

                file = CloneFromFile(true, ProjectApplication, "Models", "ViewModels", "VMUpdateTemp.cs");
                GenModelRefViewToFile(file, columns);

                CloneFromFile(true, ProjectApplication, "Interfaces", "ITempServices.cs");

                CloneFromFile(true, ProjectApplication, "Interfaces", "ITempServices.cs");
                CloneFromFile(true, ProjectApplication, "Services", "TempService.cs");


                file = CloneFromFile(true, ProjectApplication, "Services", "TempService.cs");
                GenToService(file, columns);

                AddMapping();
                DIServiceWrapper();
            }
            if (cbIsController.Checked)
            {
                CloneFromFile(true, ProjectWeb, "wwwroot", "js", "controller", "TempController.js");

                CloneFromFile(false, ProjectWeb, "Controllers", "TempController.cs");
                CloneFromFile(false, ProjectWeb, "Views", "Temp", "Index.cshtml");
                CloneFromFile(false, ProjectWeb, "Views", "Temp", "Index_Temps.cshtml");

                var file = CloneFromFile(false, ProjectWeb, "Views", "Temp", "Index_Update.cshtml");
                GenWebUpdateViewToFile(file, columns);

                file = CloneFromFile(false, ProjectWeb, "Views", "Temp", "Index_Temps.cshtml");
                GenWebTableViewToFile(file, columns);

            }
            AddLog($"Gen code {txtDisplayname.Text} ({TableName}) thành công");
            LogOutput();
        }
       
        private void AddLog(string strLog)
        {
            rtbOutput.AppendText(DateTime.Now.ToString("HH:mm:ss") + string.Format(": {0}\n", strLog));
            rtbOutput.ScrollToCaret();
        }
        private string ReplaceContent(string input)
        {
            try
            {
                return input
                    .Replace("{idDatatype}", IDDatatype)
                    .Replace("Temp.Application", ProjectApplication)
                    .Replace("Temp.Domain", ProjectDomain)
                    .Replace("Temp.Infrastructure", ProjectInfrastructure)
                    .Replace("Temp.Web", ProjectWeb)
                    .Replace("Temp.Utility", ProjectUtility)

                    .Replace("temp-link", ConvertToUrl(txtName.Text))
                    .Replace("temp-name", txtDisplayname.Text.ToLower())
                    .Replace("Temp-name", txtDisplayname.Text)
                    .Replace("Temp", txtName.Text.Trim())

                    .Replace("{.Group}", $".{txtGroup.Text.Trim()}")
                    .Replace("{Title}", txtDisplayname.Text.Trim())
                    .Replace("{title}", txtDisplayname.Text.Trim().ToLower())
                    .Replace("{Group}", txtGroup.Text.Trim())
                    .Replace("{Context}", ContextFileName)
                    .Replace("{Table}", TableName)
                    .Replace("{table}", LowerTableName)

                    .Replace("{_repo}", txtRepos.Text.Trim());
            }
            catch (Exception)
            {
                return input;
            }
        }
        private string GetNewFilePath(string oldFile)
        {
            try
            {
                return oldFile
                    //.Replace("TempGroup", txtName.Text.Trim())
                    .Replace("Temp", txtName.Text.Trim());
            }
            catch (Exception)
            {
                return oldFile;
            }
        }
        private string GetTempFilePath(string path)
        {
            try
            {
                return path
                    .Replace(ProjectApplication, "Temp.Application")
                    .Replace(ProjectDomain, "Temp.Domain")
                    .Replace(ProjectInfrastructure, "Temp.Infrastructure")
                    .Replace(ProjectWeb, "Temp.Web")
                    + ".txt";
            }
            catch (Exception)
            {
                return path;
            }
        }
        private static string ConvertToUrl(string input)
        {
            try
            {
                var sb = new StringBuilder();
                char previousChar = char.MinValue; // Unicode '\0'

                foreach (char c in input)
                {
                    if (char.IsUpper(c))
                    {
                        // If not the first character and previous character is not a space, insert a space before uppercase

                        if (sb.Length != 0 && previousChar != ' ')
                        {
                            sb.Append('-');
                        }
                    }
                    sb.Append(c.ToString().ToLower());
                    previousChar = c;
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        #endregion


        #region Renders
        private string CloneFromFile(bool isGroup, params string[] inputFiles)
        {
            var inputFile = Path.Combine(inputFiles);

            var inputPath = Path.GetDirectoryName(inputFile);

            var tempPath = GetTempFilePath(Path.Combine(tempRoot, inputFile));
            if (!File.Exists(tempPath))
            {
                throw new Exception($"Không tìm thấy file mẫu: {tempPath}");
            }

            string strCtl = File.ReadAllText(tempPath);

            var outputPath = Path.Combine(txtOutput.Text, inputPath, isGroup ? txtGroup.Text : string.Empty);
            outputPath = GetNewFilePath(outputPath);

            var fileName = GetNewFilePath(Path.GetFileName(inputFile));

            var outputFilePath = Path.Combine(outputPath, fileName);

            //if folder did not exist create one
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            using (StreamWriter w = new StreamWriter(outputFilePath))
            {
                w.Write(ReplaceContent(strCtl));
            }
            return outputFilePath;
        }

        /// <summary>
        /// Thêm các cột dl vào model
        /// </summary>
        /// <param name="modelFile"></param>
        /// <param name="columns"></param>
        private void GenModelToFile(string modelFile, IEnumerable<TbColumn> columns)
        {
            var cols = new List<string>();
            foreach (var column in columns)
            {
                var propName = RenderUtils.GetModelProp(IgnoreUpdateFields, column);
                if (!string.IsNullOrEmpty(propName))
                    cols.Add(propName);
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", cols));
        }
        private void GenParamToFile(string modelFile, IEnumerable<TbColumn> columns)
        {
            var cols = new List<string>();
            var refCols = columns.Where(n => n.IsRefColumn);
            foreach (var column in refCols)
            {
                var propName = $"        public {column.DataType}? {column.Name} {{ get; set; }}";
                cols.Add(propName);
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", cols));
        }
        private void GenModelViewToFile(string modelFile, IEnumerable<TbColumn> columns)
        {
            var cols = new List<string>();
            foreach (var column in columns)
            {
                var propName = RenderUtils.GetModelViewProp(column);
                cols.Add(propName);
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", cols));
        }
        private void GenModelRefViewToFile(string modelFile, IEnumerable<TbColumn> columns)
        {
            var cols = new List<string>();
            var refCols = columns.Where(n => n.IsRefColumn);
            foreach (var column in refCols)
            {
                var propName = $"        public IEnumerable<{column.RefColumn}> {column.RefColumn}s {{ get; set; }} = new List<{column.RefColumn}>();";
                cols.Add(propName);
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", cols));
        }
        private void GenToService(string modelFile, IEnumerable<TbColumn> columns)
        {
            var cols = new List<string>();
            var refCols = columns.Where(n => n.IsRefColumn);
            foreach (var column in refCols)
            {
                var propName = $"            model.{column.RefColumn}s = await {txtRepos.Text}.{column.RefColumn}.GetAllListAsync();";
                cols.Add(propName);
            }
            var render = string.Join("\n", cols);
            FileUtils.ReplaceContentFile(modelFile, render, render, render);
        }
        private void GenWebUpdateViewToFile(string modelFile, IEnumerable<TbColumn> columns)
        {

            var refSources = new List<string>();
            var refCols = columns.Where(n => n.IsRefColumn);
            foreach (var column in refCols)
            {
                var refSource = RenderUtils.GetSelectSource(column, $"Model.{column.Name}");
                refSources.Add(refSource);
            }

            var inputs = new List<string>();
            foreach (var column in columns)
            {
                if (IgnoreUpdateFields != null && IgnoreUpdateFields.Contains(column.Name))
                {
                    continue;
                }
                var propName = RenderUtils.GetInput(column);
                inputs.Add(propName);
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", refSources), string.Join("\n", inputs));
        }
        private void GenWebTableViewToFile(string modelFile, IEnumerable<TbColumn> columns)
        {
            var headers = new List<string>();
            var bodys = new List<string>();

            foreach (var column in columns.Where(x => !IgnoreUpdateFields.Contains(x.Name)).Take(2))
	    {
                headers.Add(RenderUtils.GetHeader(column));
                bodys.Add(RenderUtils.GetBody(column));
            }
            FileUtils.ReplaceContentFile(modelFile, string.Join("\n", headers), string.Join("\n", bodys));
        }

        /// <summary>
        /// DI Repository
        /// </summary>
        private void DIRepositoryWrapper()
        {
            var contextFileName = ContextFileName;
            if (string.IsNullOrWhiteSpace(contextFileName))
                contextFileName = $"{txtGroup.Text}Context"; //gán theo format 

            var reposWrapperFileName = Utils.GetStringAppSettings("ReposWrapper"); //Lấy từ appsetting
            if (string.IsNullOrWhiteSpace(reposWrapperFileName))
                reposWrapperFileName = $"{txtGroup.Text}RepositoryWrapper"; //gán theo format 

            var iReposWrapperFileName = Utils.GetStringAppSettings("InterfaceReposWrapper"); //Lấy từ appsetting
            if (string.IsNullOrWhiteSpace(iReposWrapperFileName))
                iReposWrapperFileName = $"I{txtGroup.Text}RepositoryWrapper";//gán theo format 

            //Check valid
            var contextFile = Path.Combine(txtOutput.Text, ProjectInfrastructure, "Contexts", contextFileName + ".cs");
            CheckFileUpdate(contextFile);

            var reposWrapperFile = Path.Combine(txtOutput.Text, ProjectInfrastructure, "Repositories", txtGroup.Text, reposWrapperFileName + ".cs");
            CheckFileUpdate(reposWrapperFile);

            var iReposWrapperFile = Path.Combine(txtOutput.Text, ProjectDomain, "Interfaces", txtGroup.Text, iReposWrapperFileName + ".cs");
            CheckFileUpdate(iReposWrapperFile);

            //Update file
            FileUtils.AppendContentFile(contextFile, $"        public DbSet<{TableName}> {TableName} {{ get; set; }}\r\n");

            FileUtils.AppendContentFile(iReposWrapperFile, $"        I{TableName}Repository {TableName} {{ get; }}");

            FileUtils.AppendContentFile(reposWrapperFile, RenderUtils.GetNewReposWrapper(TableName, LowerTableName));

        }

        /// <summary>
        /// DI Service
        /// </summary>
        private void DIServiceWrapper()
        {
            var diServiceFileName = Utils.GetStringAppSettings("DIServiceWrapper") + ".cs"; //Lấy từ appsetting

            //Check valid
            var diServiceWrapperFile = Path.Combine(txtOutput.Text, ProjectApplication, "Services", diServiceFileName);
            CheckFileUpdate(diServiceWrapperFile);

            //Update file
            FileUtils.AppendContentFile(diServiceWrapperFile, $"            services.AddScoped<I{TableName}Services, {TableName}Service>();\r\n");
        }

        /// <summary>
        /// AddMapping
        /// </summary>
        private void AddMapping()
        {
            var mappingProfile = Utils.GetStringAppSettings("MappingProfile") + ".cs"; //Lấy từ appsetting

            //Check valid
            var diServiceWrapperFile = Path.Combine(txtOutput.Text, ProjectApplication, "AutoMapper", mappingProfile);
            CheckFileUpdate(diServiceWrapperFile);

            //Update file
            FileUtils.AppendContentFile(diServiceWrapperFile,
                $"            CreateMap<{TableName}, VM{TableName}>();" +
                $"\r\n            CreateMap<VM{TableName}, {TableName}>();" +
                $"\r\n            CreateMap<{TableName}, VMUpdate{TableName}>();" +
                $"\r\n            CreateMap<VMUpdate{TableName}, {TableName}>();" +
                $"\r\n");
        }

        /// <summary>
        /// Kiểm tra file cần update hợp lệ ko
        /// </summary>
        /// <param name="updateFile"></param>
        /// <exception cref="Exception"></exception>
        private void CheckFileUpdate(string updateFile)
        {
            if (!File.Exists(updateFile))
            {
                //Kiểm tra tồn tại
                throw new Exception($"Không tìm thấy file: {updateFile}");
            }
            var lines = File.ReadAllText(updateFile);
            if (!lines.Contains(Const.KeyToRender))
            {
                //Kiểm tra có key xác định vị trí để render ko
                throw new Exception($"Vui lòng thêm {Const.KeyToRender} (Vị trí render) vào file: {updateFile}");
            }
        }
        #endregion
    }
}
