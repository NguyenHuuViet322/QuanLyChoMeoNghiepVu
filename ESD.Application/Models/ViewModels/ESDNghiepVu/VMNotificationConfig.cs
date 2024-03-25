namespace ESD.Application.Models.ViewModels.DasKTNN
{
    public class VMNotificationConfig
    {
        public NotificationConfigCondition Condition { get; set; }
        public PaginatedList<NotificationConfig> NotificationConfigs { get; set; }  
        public int Status { get; set; }
    }

    public class NotificationConfig
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ActiveNotification { get; set; }
    }

    public class NotificationConfigCondition
    {
        public NotificationConfigCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
