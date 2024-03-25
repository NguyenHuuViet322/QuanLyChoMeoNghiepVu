namespace DAS.Web.Models
{
    public class LeftMenuComponentModel
    {
        /// <summary>
        /// Tên route
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// Cấu hình Pattern cho route
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// Tên menu
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tên Controller
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// Tên Action
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// Id menu cha = 0 nếu k có menu cha
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Thứ tự hiển thị menu con
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// Id menu
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Icon menu nếu có
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Pattern mặc định của menu set empty nếu có cấu hình route nhé :)
        /// </summary>
        public string Href { get; set; }
        public int Code { get; set; }
    }
}
