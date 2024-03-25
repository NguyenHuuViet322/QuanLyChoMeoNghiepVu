using ESD.Utility.CustomClass;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESD.Utility.Helper
{
    public class CustomSelectItem : SelectListItem
    {
        public long Parent { get; set; }
        public string Parents { get; set; }
        public string Class { get; set; }
        public string SelectedValue { get; set; }

    }
    public static class HelperHtml
    {
        public const string CusClassSelect2 = "select2-cs-single form-control-sm select2-hidden-accessible";
        public const string CusClassSelect2Multy = "select2-cs";
        #region---------------Control---------------------------

        /// <summary>
        /// Tự sịnh textbox
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="displayname">Tên hiển thị</param>
        /// <param name="placeholder"></param>
        /// <param name="isNotEmpty">Bắt buộc?</param>
        /// <param name="htmlAttributes"></param>
        /// <param name="maxlen">Độ dài ký tự tối đa</param>
        /// <param name="isReadOnly">Chỉ đọc</param>
        /// <returns></returns>
        public static IHtmlContent CustTexBox(this IHtmlHelper htmlHelper, string id, string name, string value, string displayname, string placeholder, bool isNotEmpty = false, object htmlAttributes = null, bool isReadOnly = false, int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {

            var tag = new TagBuilder("input");

            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
            }
            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes, "text", "", minLen, maxLen, minVal, maxVal);
            return tag;
        }
        public static IHtmlContent CustTexBoxPassword(this IHtmlHelper htmlHelper, string id, string name, string value, string displayname, string placeholder, bool isNotEmpty = false, object htmlAttributes = null, bool isReadOnly = false, int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {

            var tag = new TagBuilder("input");

            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
            }
            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes, "password", "", minLen, maxLen, minVal, maxVal);
            return tag;
        }
        public static IHtmlContent CustTexBoxSearch(this IHtmlHelper htmlHelper, string id, string name, object value, string displayname, string placeholder, bool isNotEmpty = false, object htmlAttributes = null, bool isReadOnly = false, int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {

            var tag = new TagBuilder("input");

            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
                //ko bắt lenght, max value khi readonly
                minLen = 0; maxLen = 0; minVal = null; maxVal = null;
            }
            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes, "search", "", minLen, maxLen, minVal, maxVal);
            return tag;
        }

        public static IHtmlContent CusTextBoxDate(this IHtmlHelper html, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, int maxlen = 0, bool isReadOnly = false)
        {
            TagBuilder tag = new TagBuilder("input");
            if (maxlen > 0)
            {
                tag.MergeAttribute("data-bv-stringLength-message", displayname + string.Format(" không được vượt quá {0} ký tự", maxlen));
                tag.MergeAttribute("data-bv-stringLength-max", maxlen.ToString());
                tag.MergeAttribute("minlength", "0");
            }
            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
                tag.MergeAttribute("disabled", "disabled");
            }
            else
            {
                tag.MergeAttribute("class", "form-control datetimepicker-input");
            }
            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes);
            return tag;
        }

        public static IHtmlContent CusTextBoxDate2(this IHtmlHelper html, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, int maxlen = 0, bool isReadOnly = false)
        {
            TagBuilder tag = new TagBuilder("input");

            var dpId = "pd" + (id.IsNotEmpty() ? id : name);
            tag.MergeAttribute("data-toggle", "datetimepicker");
            tag.MergeAttribute("data-target", "#" + dpId);

            if (maxlen > 0)
            {
                tag.MergeAttribute("data-bv-stringLength-message", displayname + string.Format(" không được vượt quá {0} ký tự", maxlen));
                tag.MergeAttribute("data-bv-stringLength-max", maxlen.ToString());
                tag.MergeAttribute("minlength", "0");
            }
            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
            }
            else
            {
                tag.MergeAttribute("class", "form-control datetimepicker-input");
            }
            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes);

            TagBuilder div = new TagBuilder("div");
            div.MergeAttribute("id", dpId);
            div.MergeAttribute("class", "input-group date_input");
            div.MergeAttribute("data-target-input", "nearest");

            StringBuilder input = new StringBuilder();
            input.AppendFormat("<div class='input-group-prepend' data-target='#{0}' data-toggle='datetimepicker'>", dpId);
            input.AppendFormat("    <div class='input-group-text'><i class='far fa-calendar-alt'></i></div>");
            input.AppendFormat("</div>");
            input.Append(tag.ToStringHtml());
            div.InnerHtml.AppendHtmlLine(input.ToString());
            return div;
        }


        /// <summary>
        /// Tự sinh Dropdowlist dạng select2
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="optionLabel">Text option mặc định, ko cần > để null </param> 
        /// <param name="list">List dữ liệu</param>
        /// <param name="htmlAttributes">htmlAttributes, ko cần > để null </param>
        /// <param name="displayName">Tên dropdown</param>
        /// <param name="isNotEmpty">Cho phép để trống ko (khi dùng Bs validate)</param>
        /// <param name="isReadonly">Chỉ đọc</param>
        /// <param name="defaultVal">Giá trị  của option mặc định</param>
        /// <returns></returns>
        public static IHtmlContent CusDropdownListSelect2(this IHtmlHelper htmlHelper, string id, string name, string optionLabel, IEnumerable<SelectListItem> list, object htmlAttributes = null, string displayName = "", bool isNotEmpty = false, bool isReadonly = false, bool isMultiple = false, string defaultVal = "0", string value = null)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }
            TagBuilder dropdown = new TagBuilder("select");
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (!attributes.ContainsKey("class"))
            {
                dropdown.Attributes.Add("class", "form-control select2 " + (!isMultiple ? CusClassSelect2 : CusClassSelect2Multy));
            }
            else
            {
                dropdown.Attributes.Add("class", "form-control select2 " + (!isMultiple ? CusClassSelect2 : CusClassSelect2Multy) + " " + attributes.FirstOrDefault(x => x.Key.ToLower().Equals("class")).Value);
            }

            if (isReadonly)
            {
                dropdown.MergeAttribute("disabled", "disabled");
            }
            if (isMultiple)
            {
                dropdown.MergeAttribute("multiple", "true");
                dropdown.MergeAttribute("data-close-on-select", "false");
            }

            dropdown.Attributes.Add("data-live-search", "true");
            dropdown.Attributes.Add("data-live-search-placeholder", displayName);
            dropdown.Attributes.Add("data-actions-box", "false");
            dropdown.SetCommonDropdowList(name, id, optionLabel, list, htmlAttributes, displayName, isNotEmpty, defaultVal, value);
            return dropdown;
        }
        /// <summary>
        /// Tự sinh text box kiểu bắt buộc, hoặc số
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">Tên</param>
        /// <param name="displayname">Tên hiển thị</param>
        /// <param name="value">Giá trị</param>
        /// <param name="placeholder">placeholder</param>
        /// <param name="isNotEmpty">Bắt buộc nhập?</param>
        /// <param name="htmlAttributes">Các thuộc tính khác</param>
        /// <returns></returns>
        public static IHtmlContent CusTextBoxMoney(this IHtmlHelper html, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, bool isReadonly = false, int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {
            TagBuilder tag = new TagBuilder("input");
            if (isReadonly)
            {
                tag.MergeAttribute("readonly", "true");
            }
            tag.setCommonTextBox(id, name, value, placeholder, placeholder, isNotEmpty, htmlAttributes, "input", "isNumberInteger useMoney", minLen, maxLen, minVal, maxVal);
            return tag;
        }

        public static IHtmlContent CusTextArea(this IHtmlHelper html, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, int row = 4, int col = 20, bool isReadonly = false, int minLen = 0, int maxLen = 0)
        {
            TagBuilder tag = new TagBuilder("textarea");
            tag.MergeAttribute("id", id);
            tag.MergeAttribute("name", name);
            tag.MergeAttribute("class", "form-control");
            tag.MergeAttribute("placeholder", placeholder);
            tag.MergeAttribute("title", placeholder);
            tag.MergeAttribute("rows", row.ToString());
            tag.MergeAttribute("cols", col.ToString());
            tag.MergeAttribute("data-bv-field", name);
            if (isNotEmpty)
            {
                tag.MergeAttribute("data-bv-notempty-message", string.Format("{0} không được để trống", displayname));
                tag.MergeAttribute("data-bv-notempty", "true");
            }
            if (!Equals(value, null))
                tag.InnerHtml.SetContent(value.ToString());


            if (isReadonly)
            {
                tag.MergeAttribute("disabled", "disabled");
            }
            if (maxLen > 0 || minLen > 0)
            {
                var text = $" không được ngoài khoảng từ {minLen} đến {maxLen} ký tự";
                if (maxLen > 0 && minLen <= 0)
                    text = $" không được vượt quá {maxLen} ký tự";
                else if (maxLen <= 0 && minLen > 0)
                    text = $" không được ít hơn {maxLen} ký tự";
                tag.MergeAttribute("data-bv-stringLength-message", displayname + text);
                tag.MergeAttribute("maxlength", maxLen.ToString());
                tag.MergeAttribute("minlength", minLen.ToString());
            }

            var defaultClass = $"form-control";

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (attributes.ContainsKey("class"))
                attributes["class"] = defaultClass + attributes["class"];
            else
                attributes.Add("class", defaultClass);

            tag.MergeAttributes(attributes);
            return tag;
        }

        public static IHtmlContent CustTexBoxFloat(this IHtmlHelper html, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, bool isReadOnly = false, string separator = ".", int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {
            TagBuilder tag = new TagBuilder("input");
            tag.MergeAttribute("data-bv-numeric-message", displayname + string.Format(" phải là kiểu số"));
            if (separator.IsNotEmpty())
            {
                tag.MergeAttribute("data-bv-numeric-separator", separator);
            }
            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
            }
            if (minLen == 0 && maxLen == 0)
                maxLen = 19;

            if (maxVal == 0 && minVal == 0)
                maxVal = long.MaxValue;
            tag.setCommonTextBox(id, name, value, placeholder, placeholder, isNotEmpty, htmlAttributes, "text", "isNumber", minLen, maxLen, minVal, maxVal);
            return tag;
        }

        public static IHtmlContent CustTexBoxNumber(this IHtmlHelper htmlHelper, string id, string name, string value, string displayname, string placeholder, bool isNotEmpty = false, object htmlAttributes = null, bool isReadOnly = false, int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {
            var tag = new TagBuilder("input");
            tag.MergeAttribute("data-bv-integer-message", string.Format("{0} không phải là kiểu số nguyên", displayname));

            if (isReadOnly)
            {
                tag.MergeAttribute("readonly", string.Empty);
            }

            if (minLen == 0 && maxLen == 0)
                maxLen = 19;

            if (maxVal == 0 && minVal == 0)
                maxVal = long.MaxValue;

            tag.setCommonTextBox(id, name, value, displayname, placeholder, isNotEmpty, htmlAttributes, "text", "isNumberInteger", minLen, maxLen, minVal, maxVal);
            return tag;
        }
        #endregion----------------------------------------------

        #region Common
        private static void setCommonTextBox(this TagBuilder tag, string id, string name, object value, string displayname, string placeholder = "", bool isNotEmpty = false, object htmlAttributes = null, string type = "text", string clss = "", int minLen = 0, int maxLen = 0, long? minVal = null, long? maxVal = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            attributes.Add("type", type);
            attributes.Add("id", id);
            attributes.Add("name", name);
            attributes.Add("placeholder", placeholder);
            attributes.Add("title", displayname);

            if (maxLen > 0 || maxLen > 0)
            {
                var text = $" không được ngoài khoảng từ {minLen} đến {maxLen} ký tự";
                if (maxLen > 0 && minLen <= 0)
                    text = $" không được vượt quá {maxLen} ký tự";
                else if (maxLen <= 0 && minLen > 0)
                    text = $" không được ít hơn {maxLen} ký tự";
                tag.MergeAttribute("data-bv-stringLength-message", displayname + text);
                tag.MergeAttribute("maxlength", maxLen.ToString());
                tag.MergeAttribute("minlength", minLen.ToString());
            }

            if ((maxVal.HasValue || minVal.HasValue) && (maxVal.Value != 0 || minVal.Value != 0))
            {
                //1 trong 2 phải có value, lớn hơn 0
                var text = $"{displayname} phải trong khoảng từ {minVal} đến {maxVal}";
                if (maxVal.HasValue && !minVal.HasValue)
                {
                    if (clss.IndexOf("useMoney") > -1)
                        text = $"{displayname} không được lớn hơn {Utils.MoneyDisplay(maxVal)}";
                    else
                        text = $"{displayname} không được lớn hơn {maxVal}";
                }
                else if (!maxVal.HasValue && minVal.HasValue)
                {
                    if (clss.IndexOf("useMoney") > -1)
                        text = $"{displayname} không được lớn hơn {Utils.MoneyDisplay(minVal)}";
                    else
                        text = $"{displayname} phải lớn hơn {minVal}";
                }

                if (minVal.HasValue)
                {
                    tag.MergeAttribute("min", minVal.ToString());
                    tag.MergeAttribute("data-bv-greaterThan-message", text);

                }
                if (maxVal.HasValue)
                {
                    tag.MergeAttribute("max", maxVal.ToString());
                    tag.MergeAttribute("data-bv-lessThan-message", text);

                }
                tag.MergeAttribute("data-bv-between-inclusive", "true");

            }

            if (isNotEmpty)
            {
                attributes.Add("data-bv-field", name);
                attributes.Add("data-bv-notempty-message", displayname + string.Format(" không được để trống"));
                attributes.Add("data-bv-notempty", "true");
            }
            if (value != null)
            {
                attributes.Add("value", value.ToString());
            }

            var defaultClass = $"form-control {clss}";

            if (attributes.ContainsKey("class"))
                attributes["class"] = defaultClass + attributes["class"];
            else
                attributes.Add("class", defaultClass);
            tag.MergeAttributes(attributes);
        }

        private static void SetCommonDropdowList(this TagBuilder dropdown, string name, string id, string optionLabel, IEnumerable<SelectListItem> list, object htmlAttributes, string displayname, bool isNotEmpty, string defaultVal, string value)
        {
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", id);
            dropdown.Attributes.Add("data-container", "body");
            dropdown.Attributes.Add("data-size", "5");
            if (htmlAttributes != null)
            {
                IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var item in attributes)
                {
                    if ((item.Key.ToLower() == "disabled" || item.Key.ToLower() == "readonly") && item.Value.ToString()?.ToLower() == "false")
                    {
                        continue;
                    }

                    dropdown.MergeAttribute(item.Key, item.Value?.ToString());
                }
            }
            if (isNotEmpty)
            {
                dropdown.MergeAttribute("data-bv-field", name);//data-container="body"
                dropdown.MergeAttribute("data-bv-notempty-message", string.Format("{0} không được để trống", displayname));
                dropdown.MergeAttribute("data-bv-notempty", "true");
            }
            StringBuilder options = new StringBuilder();
            if (!string.IsNullOrEmpty(optionLabel))
            {
                options.Append("<option value='" + defaultVal + "'>" + optionLabel + "</option>");
            }
            else
            {
                options.Append("<option value=''></option>");
            }

            if (list.IsNotEmpty())
            {
                foreach (var item in list)
                {
                    var isDisabled = item.Disabled ? "disabled" : string.Empty;
                    var isSelected = item.Selected || item.Value == value ? "selected" : string.Empty;
                    if (item is SelectListItemTree)
                    {
                        options.Append("<option title='" + item.Text + "' level='" + (((SelectListItemTree)item).Level ?? 0) + "' value='" + item.Value + "'" + isDisabled + " " + isSelected + ">" + item.Text + "</span></option>");
                    }
                    else
                    {
                        options.Append("<option title='" + item.Text + "'  value='" + item.Value + "'" + isDisabled + " " + isSelected + ">" + item.Text + "</option>");
                    }
                    //options.Append("<option title='" + item.Text + "'  value='" + item.Value + "'" + isDisabled + " " + isSelected + ">" + item.Text + "</option>");
                }
            }
            dropdown.InnerHtml.AppendHtmlLine(options.ToString());
        }

        public static string ToStringHtml(this IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();
        }
        #endregion

        public static string IsSelected(this IHtmlHelper html, List<string> controller, string action = null, string cssClass = null)
        {
            bool rep = false;
            if (string.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (controller.Any())
                if (controller.Contains(currentController))
                    rep = true;

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            return rep && action == currentAction ?
                cssClass : string.Empty;
        }



        /// <summary>
        /// Tự sinh Dropdowlist dạng select2
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="optionLabel">Text option mặc định, ko cần > để null </param> 
        /// <param name="list">List dữ liệu</param>
        /// <param name="htmlAttributes">htmlAttributes, ko cần > để null </param>
        /// <param name="displayName">Tên dropdown</param>
        /// <param name="isNotEmpty">Cho phép để trống ko (khi dùng Bs validate)</param>
        /// <param name="isReadonly">Chỉ đọc</param>
        /// <param name="defaultOptionName">Text của option mặc định</param>
        /// <returns></returns>
        public static IHtmlContent CusDropdownListSelect2Ajax(this IHtmlHelper htmlHelper, string id, string name, string optionLabel, object htmlAttributes = null, string displayName = "", bool isNotEmpty = false, string url = null, string tableTarget = null, string value = null, bool isReadonly = false, bool isMultiple = false, string placeholder ="")
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }
            TagBuilder dropdown = new TagBuilder("select");
            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (!attributes.ContainsKey("class"))
            {
                dropdown.Attributes.Add("class", "select2Ajax " + (!isMultiple ? CusClassSelect2 : CusClassSelect2Multy));
            }
            else
            {
                dropdown.Attributes.Add("class", "select2Ajax " + (!isMultiple ? CusClassSelect2 : CusClassSelect2Multy) + " " + attributes.FirstOrDefault(x => x.Key.ToLower().Equals("class")).Value);
            }

            if (isReadonly)
            {
                dropdown.MergeAttribute("disabled", "disabled");
            }
            if (isMultiple)
            {
                dropdown.MergeAttribute("multiple", "true");
            }

            dropdown.Attributes.Add("data-live-search", "true");
            dropdown.Attributes.Add("data-live-search-placeholder", placeholder.IsNotEmpty() ? placeholder :displayName);
            dropdown.Attributes.Add("data-actions-box", "false");
            if (url.IsNotEmpty())
                dropdown.Attributes.Add("data-url", url);

            if (tableTarget.IsNotEmpty())
                dropdown.Attributes.Add("data-table-target", tableTarget);

            if (optionLabel.IsNotEmpty())
                dropdown.Attributes.Add("data-default-option", optionLabel);
            
            if (placeholder.IsNotEmpty())
                dropdown.Attributes.Add("data-placeholder", placeholder);

            if (value.IsNotEmpty())
                dropdown.Attributes.Add("data-selected-value", value);
            dropdown.SetCommonDropdowList(name, id, optionLabel, null, htmlAttributes, displayName, isNotEmpty, "", value);
            return dropdown;
        }
    }
}