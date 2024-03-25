namespace ESD.Application.Models.ViewModels
{
    public class VMSchemaInfo
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public int Type  { get; set; }
    }
    public class SchemaInfoCondition
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public SchemaInfoCondition()
        {
            PageIndex = 1;
            PageSize = 10;
        }
    }
}
