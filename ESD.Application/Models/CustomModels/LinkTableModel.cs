namespace ESD.Application.Models.CustomModels
{
    public class LinkTableModel
    {

        public int ID { get; set; }

        public int IDSchema { get; set; }
        public string SchemaName { get; set; }
        public int IDTable { get; set; }
        public string TableName { get; set; }
        public int IDRecord { get; set; }
        public string DataLookUp { get; set; }


        public int IDSchemaRef { get; set; }
        public string SchemaNameRef { get; set; }
        public int IDTableRef { get; set; }
        public string TableNameRef { get; set; }
        public int IDRecordRef { get; set; }
        public string DataLookUpRef { get; set; }

        //public string DataName
        //{
        //    get
        //    {
        //        return Utils.GetValueDataLookups(DataLookUp, EnumTableInfo.DefaultField.Ten.ToString(), EnumTableInfo.CategoryDefaultField.TenDanhMuc.ToString());
        //    }
        //}
    }
}