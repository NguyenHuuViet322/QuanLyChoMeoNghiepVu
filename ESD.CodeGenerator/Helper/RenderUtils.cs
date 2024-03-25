using DAS.CodeGenerator.Model;
using System;
using System.Linq;

namespace DAS.CodeGenerator.Helper
{
    public static class RenderUtils
    {
        public static string GetSelectSource(TbColumn column, string selected)
        {
            return $"var lst{column.RefColumn} = Model.{column.RefColumn}s.Select(n => new SelectListItem" +
                    $"\r\n            {{" +
                    $"\r\n                Text = n.Name.ToString()," +
                    $"\r\n                Value = n.ID.ToString()," +
                    $"\r\n                Selected = n.ID == {selected}" +
                    $"\r\n            }}).ToList();";
        }
        public static string GetInput(TbColumn column)
        {
            var title = string.IsNullOrEmpty(column.Title) ? column.Name : column.Title;
            string input;
            if (column.IsRefColumn)
            {
                input =
                    $"                <div class=\"form-group row\">" +
                    $"\r\n                    <label class=\"col-sm-3 col-form-label\" asp-for=\"{column.Name}\"></label>" +
                    $"\r\n                    <div class=\"col-sm-9\">" +
                    $"\r\n                        @Html.CusDropdownListSelect2(\"up{column.Name}\", \"{column.Name}\", \"--Chọn {title}--\", lst{column.RefColumn}," +
                    $"\r\n                        new {{" +
                    $"\r\n                            @class = \"select2 w-100\"," +
                    $"\r\n                            @data_placeholder = \"--Chọn {title}--\"," +
                    $"\r\n                            @data_container = \"body\"," +
                    $"\r\n                            @data_width = \"150px\"," +
                    $"\r\n                        }})" +
                    $"\r\n                        <span asp-validation-for=\"{column.Name}\" class=\"text-danger\"></span>" +
                    $"\r\n                    </div>" +
                    $"\r\n                </div>";
            }
            else if (column.DataType == "DateTime")
            {
                input =
                       $"                <div class=\"form-group row\">" +
                    $"\r\n                    <label class=\"col-sm-3 col-form-label\" asp-for=\"{column.Name}\"></label>" +
                    $"\r\n                    <div class=\"col-sm-9\">" +
                    $"\r\n                           <div class=\"input-group date_input\" id=\"dp{column.Name}\" data-target-input=\"nearest\">" +
                    $"\r\n                                @Html.CusTextBoxDate(\"\", \"{column.Name}\", Utils.DateToString(Model.{column.Name}), \"{title}\", \"{title}\", {(column.Nullable ? "false" : "true")}, new {{ data_target = \"#dp{column.Name}\", data_toggle = \"datetimepicker\" }}, 0)" +
                    $"\r\n                                <div class=\"input-group-prepend\" data-target=\"#dp{column.Name}\" data-toggle=\"datetimepicker\">" +
                    $"\r\n                                    <div class=\"input-group-text\">" +
                    $"\r\n                                        <span class=\"icon-calendar\"></span>" +
                    $"\r\n                                    </div>" +
                    $"\r\n                                </div>" +
                    $"\r\n                            </div>" +
                    $"\r\n                        <span asp-validation-for=\"{column.Name}\" class=\"text-danger\"></span>" +
                    $"\r\n                    </div>" +
                    $"\r\n                </div>";

            }
            else if (column.DataType == "bool")
            {
                input =
                    $"                <div class=\"form-group row\">" +
                    $"\r\n                    <label class=\"col-sm-3 col-form-label\" asp-for=\"{column.Name}\"></label>" +
                    $"\r\n                    <div class=\"col-sm-9 mt-2\">" +
                    $"\r\n                        <div class=\"custom-control custom-checkbox\">" +
                    $"\r\n                            <input type=\"checkbox\" class=\"checkboxes custom-control-input\" name=\"{column.Name}\" id=\"cb{column.Name}\"  value=\"1\" @(Model.{column.Name} > 0 ? \"checked\" : string.Empty)>" +
                    $"\r\n                            <label class=\"custom-control-label\" for=\"cb{column.Name}\"></label>" +
                    $"\r\n                            <span class=\"text-danger field-validation-valid\" data-valmsg-for=\"{column.Name}\" data-valmsg-replace=\"true\"></span>" +
                    $"\r\n                        </div>" +
                    $"\r\n                    </div>" +
                    $"\r\n                </div>";
            }
            else if (column.Name.EqualIgnoreCase("Body")
                || column.Name.EqualIgnoreCase("Content")
                || column.Name.EqualIgnoreCase("Description")
                || column.Name.EqualIgnoreCase("Describe")
                || column.Name.EqualIgnoreCase("MoTa"))
            {
                //Kiểu textarea
                input =
                        $"                <div class=\"form-group row\">" +
                        $"\r\n                    <label class=\"col-sm-3 col-form-label\" asp-for=\"{column.Name}\"></label>" +
                        $"\r\n                    <div class=\"col-sm-9\">" +
                        $"\r\n                        <textarea asp-for=\"{column.Name}\" class=\"form-control\" rows=\"5\">@Model.{column.Name}</textarea>" +
                        $"\r\n                        <span asp-validation-for=\"{column.Name}\" class=\"text-danger\"></span>" +
                        $"\r\n                    </div>" +
                        $"\r\n                </div>";
            }
            else
            {
                input =
                        $"                <div class=\"form-group row\">" +
                        $"\r\n                    <label class=\"col-sm-3 col-form-label required\" asp-for=\"{column.Name}\"></label>" +
                        $"\r\n                    <div class=\"col-sm-9\">" +
                        $"\r\n                        <input asp-for=\"{column.Name}\" class=\"form-control\" />" +
                        $"\r\n                        <span asp-validation-for=\"{column.Name}\" class=\"text-danger\"></span>" +
                        $"\r\n                    </div>" +
                        $"\r\n                </div>";
            }
            return input;
        }
        public static string GetModelViewProp(TbColumn column)
        {
            var propName = string.Empty;
            if (column.Name != "ID")
            {
                if (!string.IsNullOrEmpty(column.Title))
                {
                    propName += $"        [Display(Name = \"{column.Title}\", Prompt = \"{column.Title}\")]\n";
                }
                if (!column.Nullable)
                {
                    propName += $"        [Required(ErrorMessage = \"Giá trị không được để trống\")]\n";
                }

                if (column.MaxLength > 0)
                {
                    propName += $"        [MaxLength({column.MaxLength}, ErrorMessage = \"{{0}} không được quá {{1}} ký tự\")]\n";
                }
            }
            var newDataType = column.DataType;
            switch (column.DataType)
            {
                //Kiểu dữ liệu ở view model chưa hỗ trợ datetime, bool
                case "datetime":
                    newDataType = "string";
                    break;

                case "bool":
                    newDataType = "int";
                    break;
            }

            propName += $"        public {newDataType}{((newDataType != "string" && column.Nullable) ? "?" : string.Empty)} {column.Name} {{ get; set; }}\n ";

            return propName;
        }
        public static string GetModelProp(string[] ignoreUpdateFields, TbColumn column)
        {
            var prop = string.Empty;
            if (ignoreUpdateFields != null && ignoreUpdateFields.Contains(column.Name) && column.Name != "ID")
            {
                return prop;
            }

            if (column.Name == "ID")
            {
                prop += "        [Key]\r\n        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]\n";
            }


            if (!column.Nullable)
            {
                prop += "        [Required]\n";
            }
            prop += $"        public {column.DataType}{((column.DataType != "string" && column.Nullable) ? "?" : string.Empty)} {column.Name} {{ get; set; }}\n ";
            return prop;
        }
        public static string GetNewReposWrapper(string tableName, string lowerTableName)
        {
            return $"        private I{tableName}Repository _{lowerTableName};" +
                        $"\r\n        public I{tableName}Repository {tableName}" +
                        $"\r\n        {{" +
                                        $"\r\n            get" +
                                        $"\r\n            {{" +
                                        $"\r\n                if (_{lowerTableName} == null)" +
                                        $"\r\n                {{" +
                        $"\r\n                    _{lowerTableName} = new {tableName}Repository(_repoContext);\r\n                }}" +
                                        $"\r\n                return _{lowerTableName};" +
                                        $"\r\n            }}" +
                                        $"\r\n        }}";
        }

        public static string GetHeader(TbColumn column)
        {

            return $"                <th class=\"pb-2 m-w-200-px\">{column.Title.EmptyOrDefault(column.Name)}</th>";
        }

        public static string GetBody(TbColumn column)
        {
            return $"                    <td>@item.{column.Name}</td>";
        }

        #region Functions

        private static string EmptyOrDefault(this string str, string df)
        {
            if (string.IsNullOrWhiteSpace(str))
                return df;
            return str;
        }

        #endregion
    }
}
